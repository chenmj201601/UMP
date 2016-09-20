using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace Wcf00000
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service00000 : IService00000
    {
        public OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            try
            {
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(); }
                if (AIntOperationID == 2) { LOperationReturn = OperationA02(); }
                if (AIntOperationID == 3) { LOperationReturn = OperationA03(); }
                if (AIntOperationID == 4) { LOperationReturn = OperationA04(); }
                if (AIntOperationID == 5) { LOperationReturn = OperationA05(); }
                if (AIntOperationID == 6) { LOperationReturn = OperationA06(); }
                if (AIntOperationID == 7) { LOperationReturn = OperationA07(AListStringArgs); }
                if (AIntOperationID == 8) { LOperationReturn = OperationA08(AListStringArgs); }
                if (AIntOperationID == 9) { LOperationReturn = OperationA09(AListStringArgs); }
                if (AIntOperationID == 10) { LOperationReturn = OperationA10(AListStringArgs); }
                if (AIntOperationID == 11) { LOperationReturn = OperationA11(AListStringArgs); }
                if (AIntOperationID == 12) { LOperationReturn = OperationA12(AListStringArgs, "SignOut"); }
                if (AIntOperationID == 13) { LOperationReturn = OperationA12(AListStringArgs, "Online"); }
                if (AIntOperationID == 14) { LOperationReturn = OperationA14(AListStringArgs); }
                if (AIntOperationID == 15) { LOperationReturn = OperationA15(AListStringArgs); }
                if (AIntOperationID == 16) { LOperationReturn = OperationA16(AListStringArgs); }
                if (AIntOperationID == 17) { LOperationReturn = OperationA17(AListStringArgs); }
                if (AIntOperationID == 18) { LOperationReturn = OperationA18(AListStringArgs); }
                if (AIntOperationID == 19) { LOperationReturn = OperationA19(AListStringArgs); }

                if (AIntOperationID == 21) { LOperationReturn = OperationA21(AListStringArgs); }
                if (AIntOperationID == 22) { LOperationReturn = OperationA22(AListStringArgs); }

                if (AIntOperationID == 26) { LOperationReturn = OperationA26(AListStringArgs); }
                if (AIntOperationID == 27) { LOperationReturn = OperationA27(AListStringArgs); }

                if (AIntOperationID == 31) { LOperationReturn = OperationA31(AListStringArgs); }
                if (AIntOperationID == 32) { LOperationReturn = OperationA32(AListStringArgs); }
                if (AIntOperationID == 33) { LOperationReturn = OperationA33(AListStringArgs); }

                if (AIntOperationID == 41) { LOperationReturn = OperationA41(AListStringArgs); }
                if (AIntOperationID == 42) { LOperationReturn = OperationA42(AListStringArgs, "SignOut"); }
                if (AIntOperationID == 43) { LOperationReturn = OperationA42(AListStringArgs, "Online"); }

                if (AIntOperationID == 44) { LOperationReturn = OperationA44(AListStringArgs); }
                if (AIntOperationID == 45) { LOperationReturn = OperationA45(AListStringArgs); }

                if (AIntOperationID == 51) { LOperationReturn = OperationA51(AListStringArgs); }
                if (AIntOperationID == 52) { LOperationReturn = OperationA52(AListStringArgs); }
                if (AIntOperationID == 53) { LOperationReturn = OperationA53(AListStringArgs); }
                if (AIntOperationID == 54) { LOperationReturn = OperationA54(AListStringArgs); }
                if (AIntOperationID == 55) { LOperationReturn = OperationA55(AListStringArgs); }

                if (AIntOperationID == 61) { LOperationReturn = OperationA61(AListStringArgs); }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

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

        private void GetFolderSubAllDirectoriesAndFiles(DirectoryInfo ADirInformation, ref List<string> AListStrFiles)
        {

            FileInfo[] LFileInfoGetFiles = ADirInformation.GetFiles();
            foreach (FileInfo LFileInfoSingle in LFileInfoGetFiles)
            {
                if (LFileInfoSingle.Name.Contains("Thumbs.db")) { continue; }
                AListStrFiles.Add(LFileInfoSingle.FullName);
            }

            DirectoryInfo[] LDirInfoGetDirectories = ADirInformation.GetDirectories();
            foreach (DirectoryInfo LDirectoryInfoSingle in LDirInfoGetDirectories)
            {
                GetFolderSubAllDirectoriesAndFiles(LDirectoryInfoSingle, ref AListStrFiles);
            }
        }

        /// <summary>
        /// 获取UMP的简称
        /// </summary>
        /// <returns>返回结果保存在 OperationDataArgs.StringReturn 中</returns>
        private OperationDataArgs OperationA01()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("UMPApplication");
                LOperationReturn.StringReturn = LXMLNodeSection.Attributes["Attribute01"].Value;
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取系统支持的 TrustZones
        /// </summary>
        /// <returns>WinVersion、SourceType、InstallType、RootRegistryKey、SubRegistryKey，以DataSet形式返回</returns>
        private OperationDataArgs OperationA02()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                DataTable LDataTableTrustZones = new DataTable();
                LDataTableTrustZones.Columns.Add("WinVersion", typeof(string));
                LDataTableTrustZones.Columns.Add("SourceType", typeof(string));
                LDataTableTrustZones.Columns.Add("InstallType", typeof(string));
                LDataTableTrustZones.Columns.Add("RootRegistryKey", typeof(string));
                LDataTableTrustZones.Columns.Add("SubRegistryKey", typeof(string));
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("TrustZones");
                XmlNodeList LXmlNodeTrustZones = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleTrustZone in LXmlNodeTrustZones)
                {
                    DataRow LDataRow = LDataTableTrustZones.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["WinVersion"] = LXmlNodeSingleTrustZone.Attributes["WinVersion"].Value;
                    LDataRow["SourceType"] = LXmlNodeSingleTrustZone.Attributes["SourceType"].Value;
                    LDataRow["InstallType"] = LXmlNodeSingleTrustZone.Attributes["InstallType"].Value;
                    LDataRow["RootRegistryKey"] = LXmlNodeSingleTrustZone.Attributes["RootRegistryKey"].Value;
                    LDataRow["SubRegistryKey"] = LXmlNodeSingleTrustZone.Attributes["SubRegistryKey"].Value;
                    LDataRow.EndEdit();
                    LDataTableTrustZones.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableTrustZones);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 取系统当前支持的语言列表XML
        /// </summary>
        /// <returns>返回结果保存在 OperationDataArgs.ListStringReturn 中，如 1033、2052、1028、1041等</returns>
        private OperationDataArgs OperationA03()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("SuportLanguages");
                XmlNodeList LXmlNodeSuportLanguages = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguage in LXmlNodeSuportLanguages)
                {
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingleLanguage.Attributes["ID"].Value);
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 取系统当前支持的协议、及相关参数
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

        /// <summary>
        /// 取数据库连接参数
        /// </summary>
        /// <returns>DBID、DBType、ServerHost、ServerPort、NameService、LoginID、LoginPwd、OtherArgs、Describer， 以DataSet形式返回</returns>
        private OperationDataArgs OperationA05()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

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

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrP03 = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrP03 != "1") { continue; }

                    DataRow LDataRow = LDataTableDatabaseList.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["DBID"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P01"].Value);
                    LDataRow["DBType"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P02"].Value);
                    LDataRow["ServerHost"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P04"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["ServerPort"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P05"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["NameService"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P06"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginID"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P07"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginPwd"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P08"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["OtherArgs"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P09"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["Describer"] = LXmlNodeSingleDatabase.Attributes["P10"].Value;
                    LDataRow.EndEdit();
                    LDataTableDatabaseList.Rows.Add(LDataRow);
                }

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
        /// 取当前的所有Style样式的xml文件，以供下载到本地
        /// </summary>
        /// <returns>返回结果保存在 OperationDataArgs.ListStringReturn 中</returns>
        private OperationDataArgs OperationA06()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrSiteBaseDirectory = string.Empty;
            string LStrThemesRoot = string.Empty;
            string LStrReplacedFile = string.Empty;

            List<string> LListStrEnumFiles = new List<string>();

            try
            {
                LOperationReturn.ListStringReturn.Clear();
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrThemesRoot = System.IO.Path.Combine(LStrSiteBaseDirectory, "Themes");
                DirectoryInfo LDirectoryInfo = new DirectoryInfo(LStrThemesRoot);
                GetFolderSubAllDirectoriesAndFiles(LDirectoryInfo, ref LListStrEnumFiles);
                foreach (string LStrSingleFile in LListStrEnumFiles)
                {
                    LStrReplacedFile = LStrSingleFile.Replace(LStrSiteBaseDirectory, "");
                    LOperationReturn.ListStringReturn.Add(LStrReplacedFile);
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 取系统当前支持的语言列表(数据库)
        /// </summary>
        /// <param name="AListStringArgs">0：数据库类型；1：数据库连接串</param>
        /// <returns></returns>
        private OperationDataArgs OperationA07(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrDynamicSQL = "SELECT C001, C003 FROM T_00_004 WHERE C006 = '1' ORDER BY C002";
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    foreach (DataRow LDataRowSingleLanguage in LOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleLanguage["C003"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleLanguage["C003"] = LStrTemp;
                    }
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取语言包
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：语言编码；
        /// 3：获取方法（M1:根据Page、UserObject、...的名称，M2：根据模块编码，M3：根据消息内容编码，M21：根据模块编码和根据Page、UserObject、...的名称，M22：根据模块编码和子模块编码， M23：根据模块编码和子模块编码和根据Page、UserObject、...的名称）
        /// 4 ~ n:根据获取方法填写不同的参数 </param>
        /// <returns></returns>
        private OperationDataArgs OperationA08(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;

            try
            {
                LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE C001 = " + AListStringArgs[2] + " ";
                if (AListStringArgs[3] == "M1")
                {
                    LStrDynamicSQL += " AND C011 = '" + AListStringArgs[4] + "'";
                }
                else if (AListStringArgs[3] == "M2")
                {
                    LStrDynamicSQL += " AND C009 = " + AListStringArgs[4];
                }
                else if (AListStringArgs[3] == "M3")
                {
                    LStrDynamicSQL += " AND C002 = '" + AListStringArgs[4] + "'";
                }
                else if (AListStringArgs[3] == "M21")
                {
                    LStrDynamicSQL += " AND C009 = " + AListStringArgs[4] + " AND C011 = '" + AListStringArgs[5] + "'";
                }
                else if (AListStringArgs[3] == "M22")
                {
                    LStrDynamicSQL += " AND C009 = " + AListStringArgs[4] + " AND C010 = " + AListStringArgs[5];
                }
                else if (AListStringArgs[3] == "M23")
                {
                    LStrDynamicSQL += " AND C009 = " + AListStringArgs[4] + " AND C010 = " + AListStringArgs[5] + " AND C011 = '" + AListStringArgs[6] + "'";
                }
                else
                {
                    LStrDynamicSQL += " AND 1 = 2";
                }
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取已经设置逻辑分区表的信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0:数据库类型
        /// 1:数据库连接串
        /// 2:租户Token
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA09(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrDynamicSQL = string.Empty;
            string LStr00000001 = string.Empty;
            string LStrDepentColumn = string.Empty;
            string LStrTableName = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + AListStringArgs[2] + "' AND C003 = 'LP' AND C004 = '1'";
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDatabaseOperationReturn.BoolReturn;
                if (!LOperationReturn.BoolReturn)
                {
                    LOperationReturn.StringReturn = LDatabaseOperationReturn.StrReturn;
                    return LOperationReturn;
                }
                foreach (DataRow LDataRowSingleRent in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStr00000001 = LDataRowSingleRent["C001"].ToString();
                    LStrTableName = "T" + LStr00000001.Substring(2, 7);
                    LStrDepentColumn = LDataRowSingleRent["C008"].ToString();
                    LOperationReturn.ListStringReturn.Add(LStrTableName + AscCodeToChr(27) + LStrDepentColumn + AscCodeToChr(27) + "1");
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 判断该域是否自动登录
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0:数据库类型
        /// 1:数据库连接串
        /// 2:域名
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA10(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStr00012003 = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStr00012003 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "SELECT * FROM T_00_012 WHERE C003 = '" + LStr00012003 + "' AND C008 = '1' AND C009 = '0' AND C010 = '1'";
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDatabaseOperationReturn.BoolReturn;
                LOperationReturn.StringReturn = LDatabaseOperationReturn.StrReturn;
                if (LOperationReturn.StringReturn == "1")
                {
                    LOperationReturn.ListStringReturn.Add(LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows[0]["C002"].ToString());
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        #region 用户登录、退出、设置在线状态、修改密码
        /// <summary>
        /// 用户登录,List<string>中的数据必须是M004加密传递
        /// </summary>
        /// <param name="AListStringArgs">0：用户帐号；1：登录密码；2：登录方式(F,N)；3：客户端机器名</param>
        /// <returns></returns>
        private OperationDataArgs OperationA11(List<string> AListStringArgs)
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

            try
            {
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;

                #region 获取客户端IP地址
                OperationContext LOperationContext = OperationContext.Current;
                MessageProperties LMessageProperties = LOperationContext.IncomingMessageProperties;
                RemoteEndpointMessageProperty LRemoteEndpointMessageProperty = LMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                LStrRemoteIPAddress = LRemoteEndpointMessageProperty.Address.ToString();
                #endregion

                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[0] + AscCodeToChr(27) + AListStringArgs[1] + AscCodeToChr(27) + AListStringArgs[2] + AscCodeToChr(27);
                LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("11000", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[3] + AscCodeToChr(27) + LStrRemoteIPAddress;

                LTcpClient = new TcpClient();
                LTcpClient.SendTimeout = 10000;
                LTcpClient.Connect("127.0.0.1", LIntService01Port);
                //LTcpClient = new TcpClient("127.0.0.1", LIntService01Port);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (!ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    LOperationReturn.BoolReturn = false;
                }
                LOperationReturn.StringReturn = LStrReadMessage;
                
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
        /// 用户退出、设置在线状态，List<string>中的数据必须是M004加密传递
        /// </summary>
        /// <param name="AListStringArgs">0：租户编号（5位）；1、用户编码（19位）；2：登录后分配的SessionID</param>
        /// <returns></returns>
        private OperationDataArgs OperationA12(List<string> AListStringArgs, string AStrMethod)
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

            try
            {
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;

                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrMethod == "SignOut")
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[0] + AscCodeToChr(27) + AListStringArgs[1] + AscCodeToChr(27) + AListStringArgs[2];
                
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
        /// 用户修改密码
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：租户编码5位
        /// 1：用户编码19位
        /// 2：原密码
        /// 3：新密码
        /// 4：登录成功后分配的SessionID
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA14(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            int LIntHttpBindingPort = 0;
            int LIntService01Port = 0;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            string LStrCallReturn = string.Empty;

            try
            {
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;
                
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A04", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[0] + AscCodeToChr(27) + AListStringArgs[1] + AscCodeToChr(27) + AListStringArgs[2];
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[3] + AscCodeToChr(27) + AListStringArgs[4];

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

            #region 原来的修改密码实现方法。2015-04-23改写成通过服务实现
            //OperationDataArgs LOperationReturn = new OperationDataArgs();
            //string LStrDynamicSQL = string.Empty;
            //List<string> LListStrArgs = new List<string>();
            //string LStrVerificationCode001 = string.Empty;
            //string LStrVerificationCode002 = string.Empty;
            //string LStrVerificationCode104 = string.Empty;
            //string LStrOldPasswordDB = string.Empty;
            //string LStrOldPasswordIn = string.Empty;
            //string LStrUpdDBPassword = string.Empty;
            //string LStrUtcNow = string.Empty;
            //string LStrIsNew = string.Empty;

            //try
            //{
            //    #region 局部变量初始化
            //    LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
            //    LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
            //    LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            //    LListStrArgs.Add(AListStringArgs[0]);
            //    LListStrArgs.Add(AListStringArgs[1]);
            //    LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
            //    LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
            //    LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
            //    LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));

            //    DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
            //    DataOperations01 LDataOperations01 = new DataOperations01();
                
            //    GlobalParametersOperations LGlobalParametersOperation = new GlobalParametersOperations();
            //    OperationsReturn LGlobalOperationReturn = new OperationsReturn();
            //    #endregion

            //    LStrUpdDBPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);

            //    #region 获取当前用户的基本信息
            //    LStrDynamicSQL = "SELECT * FROM T_11_005_" + LListStrArgs[2] + " WHERE C001 = " + LListStrArgs[3];
            //    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
            //    if (!LDatabaseOperation01Return.BoolReturn)
            //    {
            //        LOperationReturn.BoolReturn = false;
            //        LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
            //        return LOperationReturn;
            //    }
            //    #endregion
            //    DataTable LDataTable11005 = LDatabaseOperation01Return.DataSetReturn.Tables[0];
            //    LStrIsNew = LDataTable11005.Rows[0]["C025"].ToString();

            //    #region 获取当前用户所有的历史修改密码信息
            //    LStrDynamicSQL = "SELECT * FROM T_00_002 WHERE C000 = '" + LListStrArgs[2] + "' AND C001 = " + LListStrArgs[3] + " AND C003 = 'C004' ORDER BY C004 DESC";
            //    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
            //    if (!LDatabaseOperation01Return.BoolReturn)
            //    {
            //        LOperationReturn.BoolReturn = false;
            //        LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
            //        return LOperationReturn;
            //    }
            //    #endregion
            //    DataTable LDataTable00002 = LDatabaseOperation01Return.DataSetReturn.Tables[0];
                
            //    List<string> LListStr11001 = new List<string>();
            //    //0：复杂性要求；1：最短密码长度；2：最短密码使用期限；3：最长使用期限；4：强制密码历史（个数）；5：强制密码历史（天数）
            //    #region 获取需要用到的安全策略信息
            //    List<string> LListStr11001Source = new List<string>();
            //    LListStr11001Source.Add("11010101");
            //    LListStr11001Source.Add("11010102");
            //    LListStr11001Source.Add("11010201");
            //    LListStr11001Source.Add("11010202");
            //    LListStr11001Source.Add("11010301");
            //    LListStr11001Source.Add("11010302");
            //    foreach (string LStrSingle11001 in LListStr11001Source)
            //    {
            //        LGlobalOperationReturn = LGlobalParametersOperation.GetParameterSettedValue(int.Parse(LListStrArgs[0]), LListStrArgs[1], LListStrArgs[2], LStrSingle11001);
            //        if (!LGlobalOperationReturn.BoolReturn)
            //        {
            //            LOperationReturn.BoolReturn = false;
            //            LOperationReturn.StringReturn = LGlobalOperationReturn.StrReturn;
            //            return LOperationReturn;
            //        }
            //        LListStr11001.Add(LGlobalOperationReturn.StrReturn);
            //    }
            //    #endregion

            //    #region 判断原密码是否正确
            //    LStrOldPasswordDB = LDataTable11005.Rows[0]["C004"].ToString();
            //    LStrOldPasswordIn = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[4], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
            //    if (LStrOldPasswordDB != LStrOldPasswordIn)
            //    {
            //        LOperationReturn.StringReturn = "W000E01";      //原密码错误
            //        return LOperationReturn;
            //    }
            //    #endregion

            //    #region 判断最短密码使用期限
            //    if (LStrIsNew != "1")
            //    {
            //        DateTime LDateTimeLastChanged = DateTime.Parse(LDataTable11005.Rows[0]["C023"].ToString());
            //        DateTime LDateTimeCanChange = LDateTimeLastChanged.AddDays(int.Parse(LListStr11001[2]));
            //        if (LDateTimeCanChange > DateTime.UtcNow)
            //        {
            //            LOperationReturn.StringReturn = "W000E02";      //不满足密码策略中的“密码最短使用期限”
            //            LOperationReturn.ListStringReturn.Add(LListStr11001[2]);
            //            return LOperationReturn;
            //        }
            //    }
            //    #endregion

            //    #region 判断新密码长度
            //    if (LListStrArgs[5].Length < int.Parse(LListStr11001[1]))
            //    {
            //        LOperationReturn.StringReturn = "W000E03";      //不满足密码策略中的“最短密码长度”
            //        LOperationReturn.ListStringReturn.Add(LListStr11001[1]);
            //        return LOperationReturn;
            //    }
            //    #endregion

            //    #region 判断强制密码历史（个数）
            //    bool LBoolExistEqual = false;
            //    int LIntCheckedCount = 0;
            //    string LStr00002006 = string.Empty;

            //    if (LListStr11001[4] != "0" && LStrIsNew != "1")
            //    {
            //        foreach (DataRow LDataRowSingleChange in LDataTable00002.Rows)
            //        {
            //            LIntCheckedCount += 1;
            //            LStr00002006 = LDataRowSingleChange["C006"].ToString();
            //            if (LStr00002006 == LStrUpdDBPassword)
            //            {
            //                LBoolExistEqual = true; break;
            //            }
            //        }
            //        if (LBoolExistEqual)
            //        {
            //            if (LIntCheckedCount <= int.Parse(LListStr11001[4]))
            //            {
            //                LOperationReturn.StringReturn = "W000E04";      //不满足密码策略中的“强制密码历史（个数）”
            //                LOperationReturn.ListStringReturn.Add(LListStr11001[4]);
            //                return LOperationReturn;
            //            }
            //        }
            //    }
            //    #endregion

            //    #region 判断强制密码历史（天数）
            //    if (LListStr11001[5] != "0" && LStrIsNew != "1")
            //    {
            //        DateTime LDateTimeInspectionPeriod = DateTime.UtcNow.AddDays(int.Parse(LListStr11001[5]) * -1);

            //        foreach (DataRow LDataRowSingleChange in LDataTable00002.Rows)
            //        {
            //            DateTime LDateTime00002004 = DateTime.Parse(LDataRowSingleChange["C004"].ToString());
            //            if (LDateTime00002004 < LDateTimeInspectionPeriod) { continue; }
            //            LStr00002006 = LDataRowSingleChange["C006"].ToString();
            //            if (LStr00002006 == LStrUpdDBPassword)
            //            {
            //                LBoolExistEqual = true; break;
            //            }
            //        }
            //        if (LBoolExistEqual)
            //        {
            //            LOperationReturn.StringReturn = "W000E05";      //不满足密码策略中的“强制密码历史（天数）”
            //            LOperationReturn.ListStringReturn.Add(LListStr11001[5]);
            //            return LOperationReturn;
            //        }
            //    }
            //    #endregion

            //    #region 判断复杂性要求
            //    if (LListStr11001[0] == "1")
            //    {
            //        string LStrFalseReturn = string.Empty;
            //        bool LBoolMeetComplexity = PasswordVerifyOptions.MeetComplexityRequirements(LListStrArgs[5], int.Parse(LListStr11001[1]), 64, "", ref LStrFalseReturn);

            //        if (!LBoolMeetComplexity)
            //        {
            //            LOperationReturn.StringReturn = "W000E06";      //不满足密码策略中的“密码必须符合复杂性要求”
            //            LOperationReturn.ListStringReturn.Add(LListStr11001[0]);
            //            return LOperationReturn;
            //        }

            //    }
            //    #endregion

            //    #region 将修改后的密码保存到 T_11_005

            //    LStrUtcNow = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
            //    LStrDynamicSQL = "UPDATE T_11_005_" + LListStrArgs[2] + " SET C004 = '" + LStrUpdDBPassword + "', C025 = '0', ";
            //    if (LListStrArgs[0] == "3")
            //    {
            //        LStrDynamicSQL += "C023 = TO_DATE('" + LStrUtcNow + "', 'yyyy/MM/dd HH24:mi:ss') ";
            //    }
            //    else
            //    {
            //        LStrDynamicSQL += "C023 = CONVERT(DATETIME, '" + LStrUtcNow + "') ";
            //    }
            //    LStrDynamicSQL += "WHERE C001 = " + LListStrArgs[3];
            //    LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
            //    if (!LDatabaseOperation01Return.BoolReturn)
            //    {
            //        LOperationReturn.BoolReturn = false;
            //        LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
            //        return LOperationReturn;
            //    }
            //    #endregion

            //    #region 修改XML文件的administrator的密码
            //    string LStrXmlFileName = string.Empty;
            //    string LStrA01 = string.Empty;
            //    string LStrSAPassword = string.Empty;

            //    LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //    LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
            //    XmlDocument LXmlDocArgs02 = new XmlDocument();
            //    LXmlDocArgs02.Load(LStrXmlFileName);
            //    XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
            //    foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
            //    {
            //        LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
            //        if (LStrA01 != LListStrArgs[3]) { continue; }
            //        LStrSAPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
            //        LXmlNodeSingleUser.Attributes["A03"].Value = LStrSAPassword;
            //        break;
            //    }
            //    LXmlDocArgs02.Save(LStrXmlFileName);
            //    #endregion

            //    #region 将修改记录保存到T_00_002中
            //    LStrDynamicSQL = "INSERT INTO T_00_002 VALUES('" + LListStrArgs[2] + "', " + LListStrArgs[3] + ", 102, 'C004', ";
            //    if (LListStrArgs[0] == "2")
            //    {
            //        LStrDynamicSQL += " CONVERT(DATETIME, '" + LStrUtcNow + "'), '" + AscCodeToChr(27) + "', '" + LStrUpdDBPassword + "' , 2002, " + LListStrArgs[3] + ")";
            //    }
            //    else
            //    {
            //        LStrDynamicSQL += " TO_DATE('" + LStrUtcNow + "', 'yyyy/MM/dd HH24:mi:ss'), " + AscCodeToChr(27) + "', '" + LStrUpdDBPassword + "' , 2002, " + LListStrArgs[3] + ")";
            //    }
            //    LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    LOperationReturn.BoolReturn = false;
            //    LOperationReturn.StringReturn = ex.ToString();
            //}

            //return LOperationReturn;
            #endregion
        }

        #endregion

        #region 座席登录、退出、设置在线状态、修改密码
        /// <summary>
        /// 用户登录,List<string>中的数据必须是M004加密传递
        /// </summary>
        /// <param name="AListStringArgs">0：用户帐号；1：登录密码；2：登录方式(F,N)；3：客户端机器名</param>
        /// <returns></returns>
        private OperationDataArgs OperationA41(List<string> AListStringArgs)
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

            try
            {
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;

                #region 获取客户端IP地址
                OperationContext LOperationContext = OperationContext.Current;
                MessageProperties LMessageProperties = LOperationContext.IncomingMessageProperties;
                RemoteEndpointMessageProperty LRemoteEndpointMessageProperty = LMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                LStrRemoteIPAddress = LRemoteEndpointMessageProperty.Address.ToString();
                #endregion

                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A11", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[0] + AscCodeToChr(27) + AListStringArgs[1] + AscCodeToChr(27) + AListStringArgs[2] + AscCodeToChr(27);
                LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("11000", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[3] + AscCodeToChr(27) + LStrRemoteIPAddress;

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
        /// 座席退出\设置在线状态，List<string>中的数据必须是M004加密传递
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <param name="AStrMethod">0：租户编号（5位）；1、用户编码（19位）；2：登录后分配的SessionID</param>
        /// <returns></returns>
        private OperationDataArgs OperationA42(List<string> AListStringArgs, string AStrMethod)
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

            try
            {
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;

                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrMethod == "SignOut")
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A12", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                else
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A13", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                }
                LStrSendMessage += AscCodeToChr(27) + AListStringArgs[0] + AscCodeToChr(27) + AListStringArgs[1] + AscCodeToChr(27) + AListStringArgs[2];

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
        /// 座席修改密码
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码5位
        /// 3：用户编码19位
        /// 4：原密码
        /// 5：新密码
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA44(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            List<string> LListStrArgs = new List<string>();
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrOldPasswordDB = string.Empty;
            string LStrOldPasswordIn = string.Empty;
            string LStrUpdDBPassword = string.Empty;
            string LStrUtcNow = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrArgs.Add(AListStringArgs[0]);
                LListStrArgs.Add(AListStringArgs[1]);
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));

                LStrDynamicSQL = "SELECT * FROM T_11_101_" + LListStrArgs[2] + " WHERE C001 = " + LListStrArgs[3] + " ORDER BY C002 ASC";
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                LStrOldPasswordDB = LDatabaseOperation01Return.DataSetReturn.Tables[0].Rows[0]["C020"].ToString();
                LStrOldPasswordIn = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[4], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                if (LStrOldPasswordDB != LStrOldPasswordIn)
                {
                    LOperationReturn.StringReturn = "W000E01";      //原密码错误
                    return LOperationReturn;
                }

                LStrUpdDBPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[5], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrUtcNow = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                LStrDynamicSQL = "UPDATE T_11_101_" + LListStrArgs[2] + " SET C020 = '" + LStrUpdDBPassword + "', C013 = '0' WHERE C001 = " + LListStrArgs[3] + " AND C002 = 1";
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                LStrDynamicSQL = "UPDATE T_11_101_" + LListStrArgs[2] + " SET C011 = '" + LStrUtcNow + "', C013 = '0' WHERE C001 = " + LListStrArgs[3] + " AND C002 = 2";
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 座席强制修改密码
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码5位
        /// 3：用户编码19位
        /// 4：新密码
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA45(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            List<string> LListStrArgs = new List<string>();
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrOldPasswordDB = string.Empty;
            string LStrOldPasswordIn = string.Empty;
            string LStrUpdDBPassword = string.Empty;
            string LStrUtcNow = string.Empty;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrArgs.Add(AListStringArgs[0]);
                LListStrArgs.Add(AListStringArgs[1]);
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrArgs.Add(EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));

                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                
                LStrUpdDBPassword = EncryptionAndDecryption.EncryptStringSHA512(LListStrArgs[3] + LListStrArgs[4], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrUtcNow = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                LStrDynamicSQL = "UPDATE T_11_101_" + LListStrArgs[2] + " SET C020 = '" + LStrUpdDBPassword + "', C013 = '0' WHERE C001 = " + LListStrArgs[3] + " AND C002 = 1";
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                LStrDynamicSQL = "UPDATE T_11_101_" + LListStrArgs[2] + " SET C011 = '" + LStrUtcNow + "', C013 = '0' WHERE C001 = " + LListStrArgs[3] + " AND C002 = 2";
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(LListStrArgs[0]), LListStrArgs[1], LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }
        #endregion

        #region 与 Service 01 通讯用
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
        #endregion

        /// <summary>
        /// 获取用户所拥有的角色
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：UserID（19位）</param>
        /// <returns></returns>
        private OperationDataArgs OperationA15(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrC008 = string.Empty, LStrC009 = string.Empty, LStrC004 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            
            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                LStrDynamicSQL = "SELECT C001, C004, C008, C009 FROM T_11_004_" + AListStringArgs[2] + " WHERE C005 = '1' AND C006 = '0' AND C001 IN ";
                LStrDynamicSQL += "(SELECT C003 FROM T_11_201_" + AListStringArgs[2] + " WHERE C004 = " + AListStringArgs[3] + " AND C003 > 1060000000000000000 AND C003 <1070000000000000000)";

                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListStringReturn.Clear();
                    foreach (DataRow LDataRowSingleLanguage in LDatabaseOperation01Return.DataSetReturn.Tables[0].Rows)
                    {
                        LStrC008 = LDataRowSingleLanguage["C008"].ToString();
                        LStrC009 = LDataRowSingleLanguage["C009"].ToString();
                        LStrC008 = EncryptionAndDecryption.EncryptDecryptString(LStrC008, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrC009 = EncryptionAndDecryption.EncryptDecryptString(LStrC009, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        if (DateTime.Parse(LStrC008) <= DateTime.UtcNow && DateTime.Parse(LStrC009) >= DateTime.UtcNow)
                        {
                            LStrC004 = LDataRowSingleLanguage["C004"].ToString();
                            LStrC004 = EncryptionAndDecryption.EncryptDecryptString(LStrC004, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            LOperationReturn.ListStringReturn.Add(LDataRowSingleLanguage["C001"].ToString() + AscCodeToChr(27) + LStrC004);
                        }
                    }
                    
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }

            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据用户编码、角色编码获取功能、操作信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：UserID（19位）
        /// 4：RoleID（19位）
        /// 5：ModuleID（2位）
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA16(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrBeginTime = string.Empty, LStrEndTime = string.Empty;
            string LStrSiteBaseDirectory = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            Int64 LInt11003C002 = 0;
            string LStr11003C006 = string.Empty, LStr11003C007 = string.Empty, LStr11003C008 = string.Empty;
            string LStr11003005 = string.Empty;
            string LStr11003C008Hash8 = string.Empty;
            string LStrAfterEncrypto = string.Empty;
            string LStrAfterDecrypto = string.Empty;
            string LStrLastReadedDogNumber = string.Empty;
            bool LBoolExist = false;

            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                #region 获取最后一次读取的狗号
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                string[] LStrArrayReadLines = System.IO.File.ReadAllLines(System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Young.01"));
                LStrLastReadedDogNumber = LStrArrayReadLines[0];
                LStrLastReadedDogNumber = EncryptionAndDecryption.EncryptDecryptString(LStrLastReadedDogNumber, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                #endregion

                #region 获取角色拥有的操作权限
                if (AListStringArgs.Count == 5)
                {
                    LStrDynamicSQL = "SELECT A.*,B.C008 AS CBT,B.C009 AS CET ";
                    LStrDynamicSQL += "FROM T_11_003_" + AListStringArgs[2] + " A, T_11_202_" + AListStringArgs[2] + " B ";
                    LStrDynamicSQL += "WHERE B.C001 = " + AListStringArgs[4] + " AND A.C002 = B.C002 AND B.C003 = '1' AND A.C005 <> '0' ORDER BY A.C005 ASC";
                }
                else
                {
                    LStrDynamicSQL = "SELECT A.*,B.C008 AS CBT,B.C009 AS CET ";
                    LStrDynamicSQL += "FROM T_11_003_" + AListStringArgs[2] + " A, T_11_202_" + AListStringArgs[2] + " B ";
                    LStrDynamicSQL += "WHERE A.C001 = " + AListStringArgs[5] + " AND B.C001 = " + AListStringArgs[4] + " AND A.C002 = B.C002 AND B.C003 = '1' ORDER BY A.C005 ASC";
                }
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                #endregion

                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    for (int LIntLoopRow = LOperationReturn.DataSetReturn.Tables[0].Rows.Count - 1; LIntLoopRow >= 0; LIntLoopRow--)
                    {
                        DataRow LDataRowSingle = LOperationReturn.DataSetReturn.Tables[0].Rows[LIntLoopRow];
                        
                        #region 是否在允许的时间范围内
                        LStrBeginTime = LDataRowSingle["CBT"].ToString();
                        LStrEndTime = LDataRowSingle["CET"].ToString();
                        if (DateTime.Parse(LStrBeginTime) > DateTime.UtcNow || DateTime.Parse(LStrEndTime) <= DateTime.UtcNow)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow); continue;
                        }
                        #endregion

                        #region License许可
                        LStr11003C006 = LDataRowSingle["C006"].ToString().Trim();
                        LStr11003C007 = LDataRowSingle["C007"].ToString().Trim();
                        LStr11003C008 = LDataRowSingle["C008"].ToString().Trim();
                        if (string.IsNullOrEmpty(LStr11003C006))
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        LStrAfterDecrypto = EncryptionAndDecryption.EncryptDecryptString(LStr11003C006, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        if (LStrAfterDecrypto == LStr11003C006)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        string[] LStrArrayC006 = LStrAfterDecrypto.Split(AscCodeToChr(27).ToArray());
                        if (LStrArrayC006.Length != 3)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        LInt11003C002 = Int64.Parse(LDataRowSingle["C002"].ToString());
                        if ((LInt11003C002 + 1000000000).ToString() != LStrArrayC006[0])
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        if (LStrArrayC006[2] == "N") { continue; }
                        LStrAfterEncrypto = EncryptionAndDecryption.EncryptDecryptString((LInt11003C002 + 1000000000).ToString() + LStrLastReadedDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        if (LStrAfterEncrypto != LStr11003C008)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                        LStrAfterDecrypto = EncryptionAndDecryption.DecryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);
                        if(LStrAfterDecrypto == LStr11003C007)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        string[] LStrArrayC007 = LStrAfterDecrypto.Split(AscCodeToChr(27).ToArray());
                        if (LStrArrayC007.Length != 2)
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        if (LStrArrayC007[1] == "Y") { continue; }
                        else
                        {
                            LOperationReturn.DataSetReturn.Tables[0].Rows.RemoveAt(LIntLoopRow);
                            continue;
                        }
                        #endregion
                    }
                    LOperationReturn.StringReturn = LOperationReturn.DataSetReturn.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }

                foreach (DataRow LDataRowSingleFeature in LOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStr11003005 = LDataRowSingleFeature["C005"].ToString();
                    LBoolExist = false;
                    foreach (string LStrGroupID in LOperationReturn.ListStringReturn)
                    {
                        if (LStrGroupID == LStr11003005) { LBoolExist = true; break; }
                    }
                    if (!LBoolExist) { LOperationReturn.ListStringReturn.Add(LStr11003005); }
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据租户编码获取安全策略\全局参数信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA17(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrDynamicSQL = "SELECT * FROM T_11_001_" + AListStringArgs[2] + " WHERE (C003 >= 11010000 AND C003 <= 11039999) OR (C003 >= 12010000 AND C003 <= 12019999)";
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleArgument["C006"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C006"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                LStrDynamicSQL = "SELECT * FROM T_00_003 WHERE C001 >= 1106000100 AND C001 <= 1106009900";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据租户编码获取机构类型、技能组信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA18(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2];
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleArgument["C006"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C006"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取LDAP用户在UMP中的密码
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户ID（19位）
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA19(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrDynamicSQL = string.Empty;
            string LStrVerificationCode103 = string.Empty;

            try
            {
                LStrVerificationCode103 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M103);
                LStrDynamicSQL = "SELECT * FROM T_11_005_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[3];
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDatabaseOperation01Return.BoolReturn;
                if (LOperationReturn.BoolReturn)
                {
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.DataSetReturn.Tables[0].Rows[0]["C004"].ToString();
                    LOperationReturn.StringReturn = EncryptionAndDecryption.EncryptDecryptString(LOperationReturn.StringReturn, LStrVerificationCode103, EncryptionAndDecryption.UMPKeyAndIVType.M103);
                    LOperationReturn.StringReturn = LOperationReturn.StringReturn.Replace(AListStringArgs[3], "");
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 修改安全策略或全局参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：参数编码
        /// 4：参数值
        /// 5：修改人ID（19位）</param>
        /// <returns></returns>
        private OperationDataArgs OperationA21(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrDataTimeNow = string.Empty;

            try
            {
                if (AListStringArgs[3] == "11020401")
                {
                    LOperationReturn = RenameAdministratorAccount(AListStringArgs);
                    if (!LOperationReturn.BoolReturn) { return LOperationReturn; }
                }
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrDataTimeNow = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                LStrTemp = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrTemp = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[3] + LStrTemp, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "UPDATE T_11_001_" + AListStringArgs[2] + " SET C006 = '" + LStrTemp + "', C019 = " + AListStringArgs[5] + ", C018 = '" + LStrDataTimeNow + "' ";
                LStrDynamicSQL += "WHERE C002 = 11 AND C003 = " + AListStringArgs[3];
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                if (!LDatabaseOperation01Return.BoolReturn) { LOperationReturn.BoolReturn = false; }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 保存机构类型、技能组信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：数据类别
        /// 4：数据ID
        /// 5：状态
        /// 6:机构类型名称/技能组编码
        /// 7:增加人
        /// 8:机构类型/技能组描述
        /// 9:技能组名称
        /// 10:排序序号
        /// 11:C011
        /// 12：方法('A增加'、'E编辑'、'D删除')</param>
        /// <returns></returns>
        private OperationDataArgs OperationA22(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            //string LStrVerificationCode104 = string.Empty;
            string LStrDataTimeNow = string.Empty;
            string LStrDataID = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDataID = AListStringArgs[4];
                //if (AListStringArgs[12] == "A")
                //{
                //    if (ExistOrgSkillInDatabase(AListStringArgs))
                //    {
                //        LOperationReturn.BoolReturn = false;
                //        LOperationReturn.StringReturn = string.Empty;
                //        if (AListStringArgs[3] == "1") { LOperationReturn.StringReturn = "S905E01"; }
                //        else { LOperationReturn.StringReturn = "S906E01"; }
                //        return LOperationReturn;
                //    }
                //    LStrDataID = GetSerialIDOrgSkill(AListStringArgs).ToString();
                //}
                if (AListStringArgs[12] == "D")
                {
                    LStrDynamicSQL = "DELETE FROM T_11_009_" + AListStringArgs[2] + " WHERE C000 = " + AListStringArgs[3] + " AND C001 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    if (!LDatabaseOperation01Return.BoolReturn) { LOperationReturn.BoolReturn = false; }
                    LStrDynamicSQL = "UPDATE T_11_009_" + AListStringArgs[2] + " SET C002 = C002 - 1 WHERE C000 = " + AListStringArgs[3] + " AND C002 > " + AListStringArgs[10];
                    LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LStrDataID;
                }
                else
                {
                    if (ExistOrgSkillInDatabase(AListStringArgs))
                    {
                        LOperationReturn.BoolReturn = false;
                        LOperationReturn.StringReturn = string.Empty;
                        if (AListStringArgs[3] == "1") { LOperationReturn.StringReturn = "S905E01"; }
                        else { LOperationReturn.StringReturn = "S906E01"; }
                        return LOperationReturn;
                    }
                    if (AListStringArgs[12] == "A") { LStrDataID = GetSerialIDOrgSkill(AListStringArgs).ToString(); }
                    LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C000 = " + AListStringArgs[3] + " AND C001 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DB = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DB.Tables[0].TableName = "T_11_009_" + AListStringArgs[2];
                    if (AListStringArgs[12] == "A")
                    {
                        DataRow LDataRowNewData = LDataSetSave2DB.Tables[0].NewRow();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewData);
                    }
                    LDataSetSave2DB.Tables[0].Rows[0]["C000"] = Int16.Parse(AListStringArgs[3]);
                    LDataSetSave2DB.Tables[0].Rows[0]["C001"] = long.Parse(LStrDataID);
                    LDataSetSave2DB.Tables[0].Rows[0]["C002"] = Int16.Parse(AListStringArgs[10]);
                    LDataSetSave2DB.Tables[0].Rows[0]["C003"] = 0;
                    LDataSetSave2DB.Tables[0].Rows[0]["C004"] = AListStringArgs[5];
                    LDataSetSave2DB.Tables[0].Rows[0]["C005"] = 2;
                    LDataSetSave2DB.Tables[0].Rows[0]["C006"] = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[6], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LDataSetSave2DB.Tables[0].Rows[0]["C007"] = AListStringArgs[7];
                    LDataSetSave2DB.Tables[0].Rows[0]["C008"] = AListStringArgs[9];
                    LDataSetSave2DB.Tables[0].Rows[0]["C009"] = AListStringArgs[8];
                    LDataSetSave2DB.Tables[0].Rows[0]["C010"] = "";
                    LDataSetSave2DB.Tables[0].Rows[0]["C011"] = "";

                    #region 将数据写入MSSQL数据库
                    if (AListStringArgs[0] == "2")
                    {
                        LSqlConnection = new SqlConnection(AListStringArgs[1]);
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();

                        LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder.SetAllValues = false;
                        LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                        LSqlDataAdapter.Update(LDataSetSave2DB, "T_11_009_" + AListStringArgs[2]);
                        LDataSetSave2DB.AcceptChanges();
                        LSqlCommandBuilder.Dispose();
                        LSqlDataAdapter.Dispose();
                    }
                    #endregion

                    #region 将数据写入Oracle数据库
                    if (AListStringArgs[0] == "3")
                    {
                        LOracleConnection = new OracleConnection(AListStringArgs[1]);
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                        LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder.SetAllValues = false;
                        LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                        LOracleDataAdapter.Update(LDataSetSave2DB, "T_11_009_" + AListStringArgs[2]);
                        LDataSetSave2DB.AcceptChanges();
                        LOracleCommandBuilder.Dispose();
                        LOracleDataAdapter.Dispose();
                    }
                    #endregion

                    LOperationReturn.StringReturn = LStrDataID;
                }
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

        private bool ExistOrgSkillInDatabase(List<string> AListStringArgs)
        {
            bool LBoolReturn = false;
            string LStrCheckTarget = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrCheckTarget = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[6], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                if (AListStringArgs[3] == "1")
                {
                    if (AListStringArgs[12] == "E")
                    {
                        LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C001 >= 9050000000000000000 AND C001 < 9060000000000000000 AND C006 = '" + LStrCheckTarget + "' AND C001 <> " + AListStringArgs[4];
                    }
                    else
                    {
                        LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C001 >= 9050000000000000000 AND C001 < 9060000000000000000 AND C006 = '" + LStrCheckTarget + "'";
                    }
                }
                else
                {
                    if (AListStringArgs[12] == "E")
                    {
                        LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C001 >= 9060000000000000000 AND C001 < 9070000000000000000 AND C006 = '" + LStrCheckTarget + "' AND C001 <> " + AListStringArgs[4];
                    }
                    else
                    {
                        LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C001 >= 9060000000000000000 AND C001 < 9070000000000000000 AND C006 = '" + LStrCheckTarget + "'";
                    }
                }
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.StrReturn != "0") { LBoolReturn = true; }
            }
            catch { }

            return LBoolReturn;
        }

        private long GetSerialIDOrgSkill(List<string> AListStringArgs)
        {
            long LLongSerialID = 0;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                if (AListStringArgs[3] == "1")
                {
                    LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(int.Parse(AListStringArgs[0]), AListStringArgs[1], 11, 905, AListStringArgs[2], "20140101000000");
                }
                if (AListStringArgs[3] == "2")
                {
                    LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(int.Parse(AListStringArgs[0]), AListStringArgs[1], 11, 906, AListStringArgs[2], "20140101000000");
                }
                if (!LDatabaseOperation01Return.BoolReturn) { LLongSerialID = 0; }
                else { LLongSerialID = long.Parse(LDatabaseOperation01Return.StrReturn); }
            }
            catch { LLongSerialID = 0; }
            return LLongSerialID;
        }

        /// <summary>
        /// 获取 所有机构 \ 座席 \ 技能组 \ 技能组包含的座席 \ 用户管理的座席 信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户编码（19位）
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA26(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                #region 0:所有用户信息
                LStrDynamicSQL = "SELECT C001, C002, C003, C006, C010, C011 FROM T_11_005_" + AListStringArgs[2];
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.ListDataSetReturn[0].Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleArgument["C002"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C002"] = LStrTemp;
                        LStrTemp = LDataRowSingleArgument["C003"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C003"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 1:获取机构信息
                LStrDynamicSQL = "SELECT C001, C002, C003, C004 FROM T_11_006_" + AListStringArgs[2] + " WHERE C005 = '1' AND C006 = '0'";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.ListDataSetReturn[1].Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleArgument["C002"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C002"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 2:获取座席信息
                LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 > 1030000000000000000 AND C001 < 1040000000000000000";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.ListDataSetReturn[2].Tables[0].Rows)
                    {
                        if (LDataRowSingleArgument["C002"].ToString() != "1") { continue; }
                        LStrTemp = LDataRowSingleArgument["C017"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C017"] = LStrTemp;
                        LStrTemp = LDataRowSingleArgument["C018"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C018"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 3:获取技能组信息
                LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStringArgs[2] + " WHERE C000 = 2 AND C004 = '1' ORDER BY C002";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    foreach (DataRow LDataRowSingleArgument in LOperationReturn.ListDataSetReturn[3].Tables[0].Rows)
                    {
                        LStrTemp = LDataRowSingleArgument["C006"].ToString();
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LDataRowSingleArgument["C006"] = LStrTemp;
                    }
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 4:获取技能组包含的座席
                LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 > 1030000000000000000 AND C004 < 1040000000000000000";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 5:获取用户管理的座席
                LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 > 1030000000000000000 AND C004 < 1040000000000000000";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 6:获取用户管理的部门
                LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[3] + " AND C004 > 1010000000000000000 AND C004 < 1020000000000000000";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 7:获取用户管理的用户
                LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[3] + " AND C004 > 1020000000000000000 AND C004 < 1030000000000000000";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.ListDataSetReturn.Add(LDatabaseOperation01Return.DataSetReturn);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            return LOperationReturn;
        }

        /// <summary>
        /// 保存座席修改信息（修改、增加、删除）
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户编码（19位）
        /// 4：方法（E：修改；D：删除；A：增加）
        /// 5：座席编码（19位。如果方法 = ‘A’，该值为 0或String.Empty）
        /// 6：座席号
        /// 7～N：BXX，属性ID("00"格式) + char(27) + 实际数据；S00 + char(27) + 技能组ID； U00 + char(27) + 用户ID
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA27(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrUserDefaultPwd = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrDataTimeNow = string.Empty;
            string LStrDataID = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                if (AListStringArgs[4] == "D")
                {
                    #region 删除座席信息
                    LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[5] + " AND C002 = 2 AND C013 <> '0'";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetExistUsed = LDatabaseOperation01Return.DataSetReturn;
                    if (LDataSetExistUsed.Tables[0].Rows.Count > 0)
                    {
                        LOperationReturn.BoolReturn = false;
                        LOperationReturn.StringReturn = "W000E02";          //已经存在录音记录，不能被删除
                        return LOperationReturn;
                    }
                    LStrDynamicSQL = "DELETE FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[5];
                    LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    if (!LDatabaseOperation01Return.BoolReturn) { LOperationReturn.BoolReturn = false; }
                    LStrDynamicSQL = "DELETE FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[5] + " OR C004 = " + AListStringArgs[5];
                    LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LStrDataID;
                    #endregion
                }
                else
                {
                    LStrDataID = AListStringArgs[5];

                    #region 如果是增加座席，判断座席是否已经存在
                    if (AListStringArgs[4] == "A")
                    {
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[6], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 >= 1030000000000000001 AND C001 < 1040000000000000000 AND C002 = 1 AND C017 = '" + LStrTemp + "'";
                        LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                        DataSet LDataSetExistData = LDatabaseOperation01Return.DataSetReturn;
                        if (LDataSetExistData.Tables[0].Rows.Count > 0)
                        {
                            LOperationReturn.BoolReturn = false;
                            LOperationReturn.StringReturn = "W000E03";          //座席已经存在，不能增加
                            LOperationReturn.ListStringReturn.Add(LDataSetExistData.Tables[0].Rows[0]["C001"].ToString());
                            return LOperationReturn;
                        }
                    }
                    #endregion
                    
                    //新增座席从系统中获取新的流水号
                    if (AListStringArgs[4] == "A") { LStrDataID = GetSerialIDByType(AListStringArgs, 11, 103).ToString(); }

                    #region 读取座席基本信息
                    LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBBI = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBBI.Tables[0].TableName = "T_11_101_" + AListStringArgs[2];
                    #endregion

                    #region 获取用户管理的用户
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[3] + " AND C004 > 1020000000000000000 AND C004 < 1030000000000000000";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetMyControlUser = LDatabaseOperation01Return.DataSetReturn;
                    #endregion

                    #region 获取管理我的用户
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + AListStringArgs[3];
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetControlMyUser = LDatabaseOperation01Return.DataSetReturn;
                    #endregion

                    #region 获取用户管理的座席
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBUA = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBUA.Tables[0].TableName = "T_11_201_" + AListStringArgs[2];
                    #endregion

                    #region 获取技能组包含的座席
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBSA = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBSA.Tables[0].TableName = "T_11_201_" + AListStringArgs[2];
                    #endregion

                    #region 如果是新增座席，初始化2行数据
                    if (AListStringArgs[4] == "A")
                    {
                        #region 获取新用户默认密码
                        LStrDynamicSQL = "SELECT C006 FROM T_11_001_" + AListStringArgs[2] + " WHERE C003 = 11010501";
                        LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                        DataSet LDataSet11010501 = LDatabaseOperation01Return.DataSetReturn;
                        LStrUserDefaultPwd = LDataSet11010501.Tables[0].Rows[0][0].ToString();
                        LStrUserDefaultPwd = EncryptionAndDecryption.EncryptDecryptString(LStrUserDefaultPwd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrUserDefaultPwd = LStrUserDefaultPwd.Substring(8);
                        LStrUserDefaultPwd = EncryptionAndDecryption.EncryptStringSHA512(LStrDataID + LStrUserDefaultPwd, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        #endregion

                        for (int LIntAddRow = 1; LIntAddRow <= 2; LIntAddRow++)
                        {
                            DataRow LDataRowNewData = LDataSetSave2DBBI.Tables[0].NewRow();
                            LDataRowNewData.BeginEdit();
                            LDataRowNewData["C001"] = long.Parse(LStrDataID);
                            LDataRowNewData["C002"] = LIntAddRow;
                            if (LIntAddRow == 1)
                            {
                                LDataRowNewData["C013"] = "1";
                                LDataRowNewData["C014"] = "0";
                                LDataRowNewData["C016"] = "00";
                                LDataRowNewData["C020"] = LStrUserDefaultPwd;
                            }
                            if (LIntAddRow == 2)
                            {
                                LDataRowNewData["C011"] = "2014/01/01 00:00:00";
                                LDataRowNewData["C012"] = "0";
                                LDataRowNewData["C013"] = "0";
                            }
                            LDataRowNewData.EndEdit();
                            LDataSetSave2DBBI.Tables[0].Rows.Add(LDataRowNewData);
                        }
                    }
                    #endregion

                    int LIntAllSaveItem = AListStringArgs.Count;

                    #region 更新座席基本信息
                    int LIntPropertyID = 0, LIntPropertyRow = 0;
                    string LStrColumnName = string.Empty;
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "B") { continue; }
                        LIntPropertyID = int.Parse(LStrDataSpliter[0].Substring(1));
                        if (LIntPropertyID > 0 && LIntPropertyID <= 10) { LIntPropertyRow = 0; LStrColumnName = "C" + (LIntPropertyID + 10).ToString("000"); }
                        if (LIntPropertyID > 10 && LIntPropertyID <= 20) { LIntPropertyRow = 1; LStrColumnName = "C" + LIntPropertyID.ToString("000"); }
                        if (LIntPropertyID == 7 || LIntPropertyID == 8)
                        {
                            LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrDataSpliter[1], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                            LDataSetSave2DBBI.Tables[0].Rows[LIntPropertyRow][LStrColumnName] = LStrTemp;
                        }
                        else
                        {
                            LDataSetSave2DBBI.Tables[0].Rows[LIntPropertyRow][LStrColumnName] = LStrDataSpliter[1];
                        }
                    }
                    #endregion

                    #region 更新管理人员信息
                    int LIntAllOldManager = 0;
                    string LStrManager = string.Empty;
                    LIntAllOldManager = LDataSetSave2DBUA.Tables[0].Rows.Count;
                    for (int LIntLoopOldManager = LIntAllOldManager - 1; LIntLoopOldManager >= 0; LIntLoopOldManager--)
                    {
                        LStrManager = LDataSetSave2DBUA.Tables[0].Rows[LIntLoopOldManager]["C003"].ToString();
                        DataRow[] LDataRowIsMyCtrl = LDataSetMyControlUser.Tables[0].Select("C004 = " + LStrManager);
                        if (LDataRowIsMyCtrl.Length > 0) { LDataSetSave2DBUA.Tables[0].Rows[LIntLoopOldManager].Delete(); }
                    }
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "U") { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBUA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = long.Parse(LStrDataSpliter[1]);
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBUA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    foreach (DataRow LDataRowSingleControlMy in LDataSetControlMyUser.Tables[0].Rows)
                    {
                        LStrManager = LDataRowSingleControlMy["C003"].ToString();
                        DataRow[] LDataRowExistCtrlAgent = LDataSetSave2DBUA.Tables[0].Select("C003 = " + LStrManager);
                        if (LDataRowExistCtrlAgent.Length > 0) { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBUA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = LStrManager;
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBUA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    #endregion

                    #region 更新座席所属技能组信息
                    int LIntAllOldSkillGroup = 0;
                    LIntAllOldSkillGroup = LDataSetSave2DBSA.Tables[0].Rows.Count;
                    for (int LIntLoopSkillGroup = LIntAllOldSkillGroup - 1; LIntLoopSkillGroup >= 0; LIntLoopSkillGroup--)
                    {
                        LDataSetSave2DBSA.Tables[0].Rows[LIntLoopSkillGroup].Delete();
                    }
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "S") { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBSA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = long.Parse(LStrDataSpliter[1]);
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBSA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    #endregion

                    #region 将数据保存到MSSQL数据库
                    if (AListStringArgs[0] == "2")
                    {
                        LSqlConnection = new SqlConnection(AListStringArgs[1]);

                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                        SqlDataAdapter LSqlDataAdapter1 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder1 = new SqlCommandBuilder();

                        LSqlCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder1.SetAllValues = false;
                        LSqlCommandBuilder1.DataAdapter = LSqlDataAdapter1;
                        LSqlDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + AListStringArgs[2]);
                        LDataSetSave2DBBI.AcceptChanges();
                        LSqlCommandBuilder1.Dispose();
                        LSqlDataAdapter1.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                        SqlDataAdapter LSqlDataAdapter2 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder2 = new SqlCommandBuilder();

                        LSqlCommandBuilder2.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder2.SetAllValues = false;
                        LSqlCommandBuilder2.DataAdapter = LSqlDataAdapter2;
                        LSqlDataAdapter2.Update(LDataSetSave2DBUA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBUA.AcceptChanges();
                        LSqlCommandBuilder2.Dispose();
                        LSqlDataAdapter2.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                        SqlDataAdapter LSqlDataAdapter3 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder3 = new SqlCommandBuilder();

                        LSqlCommandBuilder3.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder3.SetAllValues = false;
                        LSqlCommandBuilder3.DataAdapter = LSqlDataAdapter3;
                        LSqlDataAdapter3.Update(LDataSetSave2DBSA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBSA.AcceptChanges();
                        LSqlCommandBuilder3.Dispose();
                        LSqlDataAdapter3.Dispose();
                    }
                    #endregion

                    #region 将数据保存到Oracle数据库
                    if (AListStringArgs[0] == "3")
                    {
                        LOracleConnection = new OracleConnection(AListStringArgs[1]);

                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                        OracleDataAdapter LOracleDataAdapter1 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder1 = new OracleCommandBuilder();

                        LOracleCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder1.SetAllValues = false;
                        LOracleCommandBuilder1.DataAdapter = LOracleDataAdapter1;
                        LOracleDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + AListStringArgs[2]);
                        LDataSetSave2DBBI.AcceptChanges();
                        LOracleCommandBuilder1.Dispose();
                        LOracleDataAdapter1.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                        OracleDataAdapter LOracleDataAdapter2 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder2 = new OracleCommandBuilder();

                        LOracleCommandBuilder2.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder2.SetAllValues = false;
                        LOracleCommandBuilder2.DataAdapter = LOracleDataAdapter2;
                        LOracleDataAdapter2.Update(LDataSetSave2DBUA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBUA.AcceptChanges();
                        LOracleCommandBuilder2.Dispose();
                        LOracleDataAdapter2.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                        OracleDataAdapter LOracleDataAdapter3 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder3 = new OracleCommandBuilder();

                        LOracleCommandBuilder3.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder3.SetAllValues = false;
                        LOracleCommandBuilder3.DataAdapter = LOracleDataAdapter3;
                        LOracleDataAdapter3.Update(LDataSetSave2DBSA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBSA.AcceptChanges();
                        LOracleCommandBuilder3.Dispose();
                        LOracleDataAdapter3.Dispose();
                    }
                    #endregion

                    LOperationReturn.StringReturn = LStrDataID;
                }
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

        /// <summary>
        /// 未连接上数据库，获取当前系统支持的语言、Style名称
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：语言ID
        /// </param>
        /// <returns>
        /// OperationDataArgs.StringReturn实际语言编码
        /// OperationDataArgs.ListDataSetReturn[0] 支持的语言列表
        /// OperationDataArgs.ListDataSetReturn[1] 支持的样式
        /// OperationDataArgs.ListDataSetReturn[2] 部分必须要的语言包
        /// </returns>
        private OperationDataArgs OperationA31(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrIISBaseFolder = string.Empty;
            string LStrXMLFileFolder = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrIISBaseFolder = GetIISBaseDirectory();

                #region 初始化表
                DataTable LDataTableSupportLanguages = new DataTable();
                DataTable LDataTableSupportStyle = new DataTable();
                DataTable LDataTableLanguagePackage = new DataTable();

                LDataTableSupportLanguages.Columns.Add("C001", typeof(string));
                LDataTableSupportLanguages.Columns.Add("C002", typeof(string));
                LDataTableSupportLanguages.Columns.Add("C003", typeof(int));
                LDataTableSupportLanguages.Columns.Add("C004", typeof(string));
                LDataTableSupportLanguages.Columns.Add("C005", typeof(string));

                LDataTableSupportStyle.Columns.Add("C001", typeof(string));
                LDataTableSupportStyle.Columns.Add("C002", typeof(string));

                LDataTableLanguagePackage.Columns.Add("C001", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C002", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C003", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C004", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C005", typeof(string));
                #endregion

                #region 加载配置文件
                LStrXMLFileFolder = System.IO.Path.Combine(LStrIISBaseFolder, @"MAMT\Languages");
                LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S" + AListStringArgs[0] + ".xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S2052.xml");
                    LOperationReturn.StringReturn = "2052";
                }
                else { LOperationReturn.StringReturn = AListStringArgs[0]; }

                XmlDocument LXmlDocument = new XmlDocument();
                LXmlDocument.Load(LStrXmlFileName);
                #endregion

                #region 读取支持的语言列表
                XmlNode LXMLNodeSupportLanguages = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("SupportLanguages");
                XmlNodeList LXmlNodeSupportLanguages = LXMLNodeSupportLanguages.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguage in LXmlNodeSupportLanguages)
                {
                    DataRow LDataRow = LDataTableSupportLanguages.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleLanguage.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleLanguage.Attributes["C002"].Value;
                    LDataRow["C003"] = int.Parse(LXmlNodeSingleLanguage.Attributes["C003"].Value);
                    LDataRow["C004"] = LXmlNodeSingleLanguage.Attributes["C004"].Value;
                    LDataRow["C005"] = LXmlNodeSingleLanguage.Attributes["C005"].Value;
                    LDataRow.EndEdit();
                    LDataTableSupportLanguages.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableSupportLanguages);
                #endregion

                #region 读取支持的Style
                XmlNode LXMLNodeSupportStyle = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("SupportStyle");
                XmlNodeList LXmlNodeSupportStyles = LXMLNodeSupportStyle.ChildNodes;
                foreach (XmlNode LXmlNodeSingleStyle in LXmlNodeSupportStyles)
                {
                    DataRow LDataRow = LDataTableSupportStyle.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleStyle.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleStyle.Attributes["C002"].Value;
                    LDataRow.EndEdit();
                    LDataTableSupportStyle.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableSupportStyle);
                #endregion

                #region 读取语言包
                XmlNode LXMLNodeLanguages = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("Languages");
                XmlNodeList LXmlNodeLanguages = LXMLNodeLanguages.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguageItem in LXmlNodeLanguages)
                {
                    DataRow LDataRow = LDataTableLanguagePackage.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleLanguageItem.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleLanguageItem.Attributes["C002"].Value;
                    LDataRow["C003"] = LXmlNodeSingleLanguageItem.Attributes["C003"].Value;
                    LDataRow["C004"] = LXmlNodeSingleLanguageItem.Attributes["C004"].Value;
                    LDataRow["C005"] = LXmlNodeSingleLanguageItem.Attributes["C005"].Value;
                    LDataRow.EndEdit();
                    LDataTableLanguagePackage.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableLanguagePackage);
                #endregion

            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// MAMT用户登录
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：用户帐号；
        /// 1：登录密码；
        /// 2：客户端机器名
        /// </param>
        /// <returns>
        /// M01S00:登录成功
        /// M01E00：登录失败，用户不存在或密码错误
        /// M01E01：已经在别的机器上登录
        /// </returns>
        private OperationDataArgs OperationA32(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrRemoteIPAddress = string.Empty;

            string LStrLoginUserID = string.Empty;
            string LStrLoginAccount = string.Empty;
            string LStrLoginPassword = string.Empty;
            string LStrHostName = string.Empty;

            string LStrA01 = string.Empty, LStrA02 = string.Empty, LStrA03 = string.Empty, LStrA04 = string.Empty, LStrA05 = string.Empty, LStrA06 = string.Empty, LStrA07 = string.Empty;
            string LStrEncryptionPwd = string.Empty;

            try
            {
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");

                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrLoginAccount = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginPassword = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrHostName = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA02 = LXmlNodeSingleUser.Attributes["A02"].Value;
                    LStrA02 = EncryptionAndDecryption.EncryptDecryptString(LStrA02, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    if (LStrA02 == LStrLoginAccount)
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
                    LOperationReturn.BoolReturn = true;
                    LOperationReturn.StringReturn = EncryptionAndDecryption.EncryptDecryptString("M01E00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return LOperationReturn;
                }

                LStrEncryptionPwd = EncryptionAndDecryption.EncryptStringSHA512(LStrA01 + LStrLoginPassword, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                if (LStrEncryptionPwd != LStrA03)
                {
                    LOperationReturn.BoolReturn = true;
                    LOperationReturn.StringReturn = EncryptionAndDecryption.EncryptDecryptString("M01E00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    return LOperationReturn;
                }

                #region 获取客户端IP地址
                OperationContext LOperationContext = OperationContext.Current;
                MessageProperties LMessageProperties = LOperationContext.IncomingMessageProperties;
                RemoteEndpointMessageProperty LRemoteEndpointMessageProperty = LMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                LStrRemoteIPAddress = LRemoteEndpointMessageProperty.Address.ToString();
                #endregion

                LStrA04 = "1"; LStrA05 = LStrHostName; LStrA06 = LStrRemoteIPAddress; LStrA07 = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA02 = LXmlNodeSingleUser.Attributes["A02"].Value;
                    LStrA02 = EncryptionAndDecryption.EncryptDecryptString(LStrA02, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    if (LStrA02 == LStrLoginAccount)
                    {
                        LXmlNodeSingleUser.Attributes["A04"].Value = LStrA04;
                        LXmlNodeSingleUser.Attributes["A05"].Value = LStrA05;
                        LXmlNodeSingleUser.Attributes["A06"].Value = LStrA06;
                        LXmlNodeSingleUser.Attributes["A07"].Value = LStrA07;
                        break;
                    }
                }
                LXmlDocArgs02.Save(LStrXmlFileName);
                LOperationReturn.BoolReturn = true;
                LOperationReturn.StringReturn = EncryptionAndDecryption.EncryptDecryptString("M01S00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString(LStrA01, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString(LStrA07, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取MAMT指定的语言包
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：语言ID
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA33(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrIISBaseFolder = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrIISBaseFolder = GetIISBaseDirectory();

                DataTable LDataTableLanguagePackage = new DataTable();
                LDataTableLanguagePackage.Columns.Add("C001", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C002", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C003", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C004", typeof(string));
                LDataTableLanguagePackage.Columns.Add("C005", typeof(string));

                LStrXmlFileName = System.IO.Path.Combine(LStrIISBaseFolder, @"MAMT\Languages", "L" + AListStringArgs[0] + ".xml");
                XmlDocument LXmlDocument = new XmlDocument();
                LXmlDocument.Load(LStrXmlFileName);

                XmlNode LXMLNodeLanguages = LXmlDocument.SelectSingleNode("UMPMamtLanguages");
                XmlNodeList LXmlNodeLanguages = LXMLNodeLanguages.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguageItem in LXmlNodeLanguages)
                {
                    DataRow LDataRow = LDataTableLanguagePackage.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleLanguageItem.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleLanguageItem.Attributes["C002"].Value;
                    LDataRow["C003"] = LXmlNodeSingleLanguageItem.Attributes["C003"].Value;
                    LDataRow["C004"] = LXmlNodeSingleLanguageItem.Attributes["C004"].Value;
                    LDataRow["C005"] = LXmlNodeSingleLanguageItem.Attributes["C005"].Value;
                    LDataRow.EndEdit();
                    LDataTableLanguagePackage.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableLanguagePackage);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取安全策略信息、全局参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3～N：参数编码
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA51(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            int LIntCount = 0;

            try
            {
                GlobalParametersOperations LGlobalParametersOperation = new GlobalParametersOperations();
                OperationsReturn LGlobalOperationReturn = new OperationsReturn();

                LIntCount = AListStringArgs.Count;
                for (int LIntLoop = 3; LIntLoop < LIntCount; LIntLoop++)
                {
                    LGlobalOperationReturn = LGlobalParametersOperation.GetParameterSettedValue(int.Parse(AListStringArgs[0]), AListStringArgs[1], AListStringArgs[2], AListStringArgs[LIntLoop]);
                    LOperationReturn.ListStringReturn.Add(LGlobalOperationReturn.StrReturn);
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据登录后分配的流水号检测当前状态
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：流水号
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA52(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDBDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_11_002_" + AListStringArgs[2] + " WHERE C006 = " + AListStringArgs[3] + " AND C008 = '0'";
                LDBOperationReturn = LDBDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDBOperationReturn.BoolReturn;
                LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据用户ID获取个人参数或设置参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户编码（19位）
        /// 4：参数编码
        /// 5：读取、或设置 'R'/'W'
        /// 6: 设置值
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA53(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDBDataOperations = new DataOperations01();

                if (AListStringArgs[5] == "R")
                {
                    LStrDynamicSQL = "SELECT * FROM T_11_011_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[3] + " AND C002 = " + AListStringArgs[4];
                    LDBOperationReturn = LDBDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.BoolReturn = LDBOperationReturn.BoolReturn;
                    LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                    if (LDBOperationReturn.StrReturn != "1")
                    {
                        LOperationReturn.BoolReturn = false;
                        if (AListStringArgs[4] == "1100101")
                        {
                            LStrDynamicSQL = "INSERT INTO T_11_011_" + AListStringArgs[2] + "(C001, C002, C003, C004, C005, C006, C008, C009) VALUES(" + AListStringArgs[3] + ", " + AListStringArgs[4] + ", 11001, 1, '" + AListStringArgs[6] + "', 2, 0, 0)";
                            LDBDataOperations.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                        }
                    }
                    else
                    {
                        LOperationReturn.DataSetReturn = LDBOperationReturn.DataSetReturn;
                    }
                }
                if (AListStringArgs[5] == "W")
                {
                    LStrDynamicSQL = "UPDATE T_11_011_" + AListStringArgs[2] + " SET C005 = '" + AListStringArgs[6] + "' WHERE C001 = " + AListStringArgs[3] + " AND C002 = " + AListStringArgs[4];
                    LDBOperationReturn = LDBDataOperations.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.BoolReturn = LDBOperationReturn.BoolReturn;
                    LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据用户ID获取个人基本参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户编码（19位）
        /// 4：字段名称 如 C006-所属机构等
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA54(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDBDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_11_005_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[3];
                LDBOperationReturn = LDBDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDBOperationReturn.BoolReturn;
                LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                if (LDBOperationReturn.StrReturn != "1")
                {
                    LOperationReturn.BoolReturn = false;
                }
                else
                {
                    LOperationReturn.StringReturn = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0][AListStringArgs[4]].ToString();
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据用户ID获取个人参数 11_011表中的参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码（5位）
        /// 3：用户编码（19位）
        /// 4：参数编码 C002的值
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA55(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrVerificationCode102 = string.Empty;

            try
            {
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDBDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_11_011_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[3] + " AND C002 = " + AListStringArgs[4];
                LDBOperationReturn = LDBDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.BoolReturn = LDBOperationReturn.BoolReturn;
                LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                if (LDBOperationReturn.StrReturn != "1")
                {
                    LOperationReturn.BoolReturn = false;
                }
                else
                {
                    LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LOperationReturn.StringReturn = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0]["C005"].ToString();
                    LOperationReturn.StringReturn = EncryptionAndDecryption.EncryptDecryptString(LOperationReturn.StringReturn, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取第三方应用参数信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：第三方应用简称
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA61(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.07.xml");
                XmlDocument LXmlDocServer07 = new XmlDocument();
                LXmlDocServer07.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer07.SelectSingleNode("UMPSetted").SelectSingleNode("ThirdPartyApplications");
                foreach (XmlNode LXmlNodeSingle in LXMLNodeSection.ChildNodes)
                {
                    if (LXmlNodeSingle.Attributes["Attribute00"].Value != AListStringArgs[0]) { continue; }
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute01"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute02"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute03"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute04"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute05"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute06"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute07"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute08"].Value);
                    LOperationReturn.ListStringReturn.Add(LXmlNodeSingle.Attributes["Attribute09"].Value);
                    break;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            return LOperationReturn;
        }

        private long GetSerialIDByType(List<string> AListStringArgs, int AIntModule, int AIntType)
        {
            long LLongSerialID = 0;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(int.Parse(AListStringArgs[0]), AListStringArgs[1], AIntModule, AIntType, AListStringArgs[2], "20140101000000");
                if (!LDatabaseOperation01Return.BoolReturn) { LLongSerialID = 0; }
                else { LLongSerialID = long.Parse(LDatabaseOperation01Return.StrReturn); }
            }
            catch { LLongSerialID = 0; }
            return LLongSerialID;
        }

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
            catch(Exception ex)
            {
                LIntHttpBindingPort = 0;
                AStrErrorReturn = ex.ToString();
            }
            return LIntHttpBindingPort;
        }

        #endregion

        #region 重新设置系统管理员账户
        private OperationDataArgs RenameAdministratorAccount(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStr11005002 = string.Empty;
            string LStrXmlSAUser = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            string LStrUserID19 = string.Empty;

            string LStrXmlFileName = string.Empty;
            string LStrA01 = string.Empty;
            /// <summary>
            /// 修改安全策略或全局参数
            /// </summary>
            /// <param name="AListStringArgs">
            /// 0：数据库类型
            /// 1：数据库连接串
            /// 2：租户编码（5位）
            /// 3：参数编码
            /// 4：参数值
            /// 5：修改人ID（19位）</param>
            /// <returns></returns>
            /// 
            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrUserID19 = "102" + AListStringArgs[2] + "00000000001";
                LStrTemp = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11005002 = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrXmlSAUser = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);

                #region 判断用户是否已经存在
                LStrDynamicSQL = "SELECT * FROM T_11_005_" + AListStringArgs[2] + " WHERE C001 <> " + LStrUserID19 + " AND C002 = '" + LStr11005002 + "'";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false; return LOperationReturn;
                }
                if (LOperationReturn.StringReturn != "0")
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.ListStringReturn.Add("11020401");
                    return LOperationReturn;
                }
                #endregion

                #region 更新T_11_005
                LStrDynamicSQL = "UPDATE T_11_005_" + AListStringArgs[2] + " SET C002 = '" + LStr11005002 + "' WHERE C001 = " + LStrUserID19;
                LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false; return LOperationReturn;
                }
                #endregion

                #region 更新 UMP.Server下的Args02.UMP.xml
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                    if (LStrA01 != LStrUserID19) { continue; }
                    LXmlNodeSingleUser.Attributes["A02"].Value = LStrXmlSAUser;
                    break;
                }
                LXmlDocArgs02.Save(LStrXmlFileName);
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }
        #endregion

        #region 生成 MD5 Hash 字符串
        /// <summary>
        /// 生成 MD5 Hash 字符串
        /// </summary>
        /// <param name="AStrSource"></param>
        /// <returns></returns>
        private string CreateMD5HashString(string AStrSource)
        {
            string LStrHashPassword = string.Empty;
            try
            {
                MD5CryptoServiceProvider LMD5Crypto = new MD5CryptoServiceProvider();
                byte[] LByteArray = Encoding.Unicode.GetBytes(AStrSource);
                LByteArray = LMD5Crypto.ComputeHash(LByteArray);
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LByteArray) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrHashPassword = LStrBuilder.ToString();
            }
            catch { LStrHashPassword = "YoungPassword"; }
            return LStrHashPassword;
        }
        #endregion
    }
}
