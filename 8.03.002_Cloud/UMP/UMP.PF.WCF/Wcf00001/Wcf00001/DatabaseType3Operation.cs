using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Wcf00001
{
    public class DatabaseType3Operation
    {
        /// <summary>
        /// 连接到指定的ServiceName 并获取当前数据库对象的版本信息
        /// </summary>
        /// <param name="AListStrArguments">
        /// 0-数据库服务器
        /// 1-端口
        /// 2-LogingID
        /// 3-Login Password
        /// 4-服务名
        /// </param>
        /// <returns></returns>
        public static OperationDataArgs Connect2SpecifiedServiceObtainCurrentVersion(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            string LStrConnectParam = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            string LStrDBServer = string.Empty;
            string LStrDBPort = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPwd = string.Empty;
            string LStrServiceName = string.Empty;

            string LStrDynamicSQL = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                DataOperations01 LDataOperations = new DataOperations01();

                #region 创建数据库连接串
                LStrDBServer = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrDBPort = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginID = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginPwd = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServiceName = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrDBServer, LStrDBPort, LStrServiceName, LStrLoginID, LStrLoginPwd);
                #endregion

                LOperationDataArgsReturn.ListStringReturn.Add("0.00.000");

                #region 判断表 T_00_000 是否存在
                LStrDynamicSQL = "SELECT * FROM USER_TABLES WHERE TABLE_NAME = 'T_00_000'";

                DatabaseOperation01Return LDatabaseOperation01Return1 = LDataOperations.SelectDataByDynamicSQL(3, LStrConnectParam, LStrDynamicSQL);
                if (!LDatabaseOperation01Return1.BoolReturn)
                {
                    LOperationDataArgsReturn.ListStringReturn.Clear();
                    LOperationDataArgsReturn.BoolReturn = false;
                    LOperationDataArgsReturn.StringReturn = LDatabaseOperation01Return1.StrReturn;
                    return LOperationDataArgsReturn;
                }
                if (LDatabaseOperation01Return1.StrReturn != "1") { return LOperationDataArgsReturn; }

                #endregion

                #region 表 T_00_000 存在，获取已经创建的版本
                string LStrSelectC000 = CreateSpliterCharater() + CreateSpliterCharater() + CreateSpliterCharater() + CreateSpliterCharater() + "1";
                LStrDynamicSQL = "SELECT C002 FROM T_00_000 WHERE C000 = '" + LStrSelectC000 + "'";
                DatabaseOperation01Return LDatabaseOperation01Return2 = LDataOperations.SelectDataByDynamicSQL(3, LStrConnectParam, LStrDynamicSQL);
                if (!LDatabaseOperation01Return2.BoolReturn)
                {
                    LOperationDataArgsReturn.ListStringReturn.Clear();
                    LOperationDataArgsReturn.BoolReturn = false;
                    LOperationDataArgsReturn.StringReturn = LDatabaseOperation01Return2.StrReturn;
                    return LOperationDataArgsReturn;
                }
                if (LDatabaseOperation01Return2.StrReturn == "1")
                {
                    LOperationDataArgsReturn.ListStringReturn[0] = LDatabaseOperation01Return2.DataSetReturn.Tables[0].Rows[0][0].ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.ListStringReturn.Clear();
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "Connect2SpecifiedServiceObtainCurrentVersion()\n" + ex.ToString();
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs CreateObjectTable(List<string> AListStrArguments, string AStrIISBaseFolder)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();

            #region 局部变量定义
            string LStrDataTypeConvertFile = string.Empty;
            string LStrTableObjectFolder = string.Empty;
            string LStrTableObjectFile = string.Empty;

            string LStrTableName = string.Empty;
            string LStrCreateTableName = string.Empty;
            List<string> LListStrCreateType = new List<string>();
            string LStrTabelVersion = string.Empty;
            string LStrCreateSynonym = string.Empty;
            string LStrWithRent = string.Empty;
            string LStrCanPartition = string.Empty;
            string LStrTableDescribe = string.Empty;

            string LStrDDLSql = string.Empty;
            List<string> LStrCreateOrAlterSQL = new List<string>();

            string LStrColumnName = string.Empty;
            string LStrDataType = string.Empty;
            string LStrDataWidth = string.Empty;
            string LStrDataScale = string.Empty;
            string LStrCanNull = string.Empty;
            string LStrDefaultValue = string.Empty;
            string LStrDataConverted = string.Empty;

            string LStrDefValueName = string.Empty;

            string LStrIndexName = string.Empty;
            string LStrIndexType = string.Empty;
            string LStrCreateIndexName = string.Empty;
            string LStrIsPrimaryKey = string.Empty;
            string LStrIsUnique = string.Empty;
            string LStrIndexColumn = string.Empty;
            string LStrOrderType = string.Empty;

            string LStr00000DynamicSQL = string.Empty;
            #endregion

            try
            {
                LStrDataTypeConvertFile = System.IO.Path.Combine(AStrIISBaseFolder, @"MAMT\Configrations\DBDataTypeConvert.XML");
                LStrTableObjectFolder = System.IO.Path.Combine(AStrIISBaseFolder, @"MAMT\DBObjects");

                DataSet LDataSetDataTypeDefine = new DataSet();
                StreamReader LStreamReader = new StreamReader(LStrDataTypeConvertFile, Encoding.UTF8);
                LDataSetDataTypeDefine.ReadXml(LStreamReader);
                LStreamReader.Close();

                LStrTableObjectFile = System.IO.Path.Combine(LStrTableObjectFolder, AListStrArguments[8]);
                XmlDocument LXmlDocumentTable = new XmlDocument();
                LXmlDocumentTable.Load(LStrTableObjectFile);


                XmlNode LXMLNodeTableDefine = LXmlDocumentTable.SelectSingleNode("TableDefine");

                LStrTableName = LXMLNodeTableDefine.Attributes["P01"].Value;
                LStrTabelVersion = LXMLNodeTableDefine.Attributes["P02"].Value;
                LStrCreateSynonym = LXMLNodeTableDefine.Attributes["P03"].Value;
                LStrWithRent = LXMLNodeTableDefine.Attributes["P04"].Value;
                LStrCanPartition = LXMLNodeTableDefine.Attributes["P05"].Value;
                LStrTableDescribe = LXMLNodeTableDefine.Attributes["P06"].Value;

                if (LStrWithRent == "0")
                {
                    LListStrCreateType.Add("0");
                }
                if (LStrWithRent == "1")
                {
                    LListStrCreateType.Add("1");
                }
                if (LStrWithRent == "2")
                {
                    LListStrCreateType.Add("0");
                    LListStrCreateType.Add("1");
                }

                foreach (string LStrSingleCreateType in LListStrCreateType)
                {
                    #region 创建表
                    if (LStrSingleCreateType == "0") { LStrCreateTableName = LStrTableName; }
                    if (LStrSingleCreateType == "1") { LStrCreateTableName = LStrTableName + "_" + AListStrArguments[9]; }

                    LStrDDLSql = "CREATE TABLE " + LStrCreateTableName + "(\n";
                    XmlNodeList LXmlNodeListTableColumns = LXMLNodeTableDefine.SelectSingleNode("ColumnDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;
                        LStrDataType = LXmlNodeSingleColumn.Attributes["P02"].Value;
                        LStrDataWidth = LXmlNodeSingleColumn.Attributes["P03"].Value;
                        LStrDataScale = LXmlNodeSingleColumn.Attributes["P04"].Value;
                        LStrCanNull = LXmlNodeSingleColumn.Attributes["P05"].Value;

                        LStrDDLSql += " " + LStrColumnName;
                        DataRow[] LDataRowDataConvert = LDataSetDataTypeDefine.Tables[0].Select("Type0 = '" + LStrDataType + "'");
                        LStrDataConverted = LDataRowDataConvert[0]["Type" + AListStrArguments[1]].ToString();
                        if (LStrDataType == "01" || LStrDataType == "02" || LStrDataType == "03" || LStrDataType == "21")
                        {
                            LStrDDLSql += "     " + LStrDataConverted;
                        }
                        else if (LStrDataType == "04")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + "," + LStrDataScale + ")";
                        }
                        else if (LStrDataType == "11" || LStrDataType == "12" || LStrDataType == "13" || LStrDataType == "14")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + ")";
                        }
                        if (LStrCanNull == "0") { LStrDDLSql += "       NOT NULL"; } else { LStrDDLSql += "       NULL"; }
                        if (LXmlNodeSingleColumn.NextSibling != null) { LStrDDLSql += ",\n"; }
                    }
                    LStrDDLSql += "\n)";
                    LStrCreateOrAlterSQL.Add(LStrDDLSql);

                    LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrCreateTableName + "', '" + LStrTabelVersion + "', 'TB', '1', SYSDATE, '" + LStrCanPartition + "', 'NULL')";
                    LStrCreateOrAlterSQL.Add(LStr00000DynamicSQL);
                    #endregion

                    #region 给表中字段加 Default Value
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrDefaultValue = LXmlNodeSingleColumn.Attributes["P06"].Value.Trim();
                        if (string.IsNullOrEmpty(LStrDefaultValue)) { continue; }
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;

                        LStrDefValueName = "DF_" + LStrCreateTableName.Substring(2) + "_" + LStrColumnName;
                        LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " MODIFY (" + LStrColumnName + " DEFAULT " + LStrDefaultValue + ")";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                        LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrDefValueName + "', '" + LStrTabelVersion + "', 'DF', '1', SYSDATE, '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStr00000DynamicSQL);
                    }
                    #endregion

                    #region 创建主键、索引
                    XmlNodeList LXMLNodeIndexDefine = LXMLNodeTableDefine.SelectSingleNode("IndexDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleIndex in LXMLNodeIndexDefine)
                    {
                        LStrIndexName = LXmlNodeSingleIndex.Attributes["P01"].Value;
                        if (LStrSingleCreateType == "0") { LStrCreateIndexName = LStrIndexName; }
                        if (LStrSingleCreateType == "1") { LStrCreateIndexName = LStrIndexName + "_" + AListStrArguments[9]; }

                        LStrIsPrimaryKey = LXmlNodeSingleIndex.Attributes["P02"].Value;
                        LStrIsUnique = LXmlNodeSingleIndex.Attributes["P03"].Value;
                        if (LStrIsPrimaryKey == "1")
                        {
                            LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD ( CONSTRAINT " + LStrCreateIndexName + " PRIMARY KEY ("; LStrIndexType = "PK";
                            foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                            {
                                LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                                LStrDDLSql += LStrIndexColumn;
                                if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                            }
                            LStrDDLSql += "))";
                        }
                        else
                        {
                            if (LStrIsUnique == "1") { LStrDDLSql = "CREATE UNIQUE INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + " ("; }
                            else { LStrDDLSql = "CREATE INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + " ("; }
                            LStrIndexType = "IX";
                            foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                            {
                                LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                                LStrOrderType = LXmlNodeSingleIndexColumn.Attributes["P02"].Value;
                                if (LStrOrderType == "A") { LStrDDLSql += LStrIndexColumn + " ASC"; } else { LStrDDLSql += LStrIndexColumn + " DESC"; }
                                if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                            }
                            LStrDDLSql += ")";
                        }

                        LStrCreateOrAlterSQL.Add(LStrDDLSql);

                        LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrCreateIndexName + "', '" + LStrTabelVersion + "', '" + LStrIndexType + "', '1', SYSDATE, '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStr00000DynamicSQL);
                    }
                    #endregion

                }
                LOperationDataArgsReturn = ExecuteDynamicSql(AListStrArguments, LStrCreateOrAlterSQL);
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "CreateObjectTable()\n" + ex.Message;
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs CreateObjectBySQL(List<string> AListStrArguments, string AStrIISBaseFolder)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            string LStrTableObjectFolder = string.Empty;
            string LStrTableObjectFile = string.Empty;
            List<string> LStrCreateOrAlterSQL = new List<string>();

            try
            {
                LStrTableObjectFolder = System.IO.Path.Combine(AStrIISBaseFolder, @"MAMT\DBObjects");
                LStrTableObjectFile = System.IO.Path.Combine(LStrTableObjectFolder, AListStrArguments[8]);

                string[] LStrArrayCreateSQL = File.ReadAllLines(LStrTableObjectFile, Encoding.Default);
                string LStrObjectCreateSQL = string.Empty;
                foreach (string LStrSingleLine in LStrArrayCreateSQL)
                {
                    LStrObjectCreateSQL += LStrSingleLine + "\n";
                }
                //string LStrObjectCreateSQL = File.ReadAllText(LStrTableObjectFile, Encoding.Default);
                LStrCreateOrAlterSQL.Add(LStrObjectCreateSQL);
                LOperationDataArgsReturn = ExecuteDynamicSql(AListStrArguments, LStrCreateOrAlterSQL);


                //LOperationDataArgsReturn = 
                //StreamReader sr = File.OpenText(@"C:\temp\ascii.txt"); 
                //string restOfStream = sr.ReadToEnd();

                //使用完StreamReader之后，不要忘记关闭它： sr.Closee();

                //它们都一次将文本内容全部读完，并返回一个包含全部文本内容的字符串
                //string str = File.ReadAllText(@"c:\temp\ascii.txt");
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "CreateObjectBySQL()\n" + ex.Message;
            }

            return LOperationDataArgsReturn;
        }

        private static OperationDataArgs ExecuteDynamicSql(List<string> AListStrArguments, List<string> AListStrDDLSql)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            List<string> LListStrDatabaseProfile = new List<string>();

            string LStrVerificationCode104 = string.Empty;

            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            string LStrConnectParam = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LListStrDatabaseProfile.Add(AListStrArguments[2]);
                LListStrDatabaseProfile.Add(AListStrArguments[3]);
                LListStrDatabaseProfile.Add(AListStrArguments[4]);
                LListStrDatabaseProfile.Add(EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrDatabaseProfile.Add(AListStrArguments[6]);

                LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDatabaseProfile[0], LListStrDatabaseProfile[1], LListStrDatabaseProfile[4], LListStrDatabaseProfile[2], LListStrDatabaseProfile[3]);

                LOracleConnection = new OracleConnection(LStrConnectParam);
                LOracleConnection.Open();
                foreach (string LStrSingleDynamicSQL in AListStrDDLSql)
                {
                    LOperationDataArgsReturn.ListStringReturn.Add(LStrSingleDynamicSQL);
                    LOracleCommand = new OracleCommand(LStrSingleDynamicSQL, LOracleConnection);
                    LOracleCommand.ExecuteNonQuery();
                }
            }
            catch (OracleException LOracleException)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ExecuteDynamicSql()\n" + LOracleException.Message;
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ExecuteDynamicSql()\n" + ex.Message;
            }
            finally
            {
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs InitTablesData(List<string> AListStrArguments, string AStrIISBaseFolder)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();

            string LStrTableDataFolder = string.Empty;
            string LStrTableDataFile = string.Empty;

            string LStrInitSourceTable = string.Empty;
            string LStrInitTargetTable = string.Empty;
            string LStrInitLocalTable = string.Empty;
            string LStrInitRentTable = string.Empty;
            string LStrInitMethod = string.Empty;

            try
            {
                LStrTableDataFolder = System.IO.Path.Combine(AStrIISBaseFolder, @"MAMT\DBObjects");
                LStrTableDataFile = System.IO.Path.Combine(LStrTableDataFolder, AListStrArguments[8]);
                XmlDocument LXmlDocumentTableData = new XmlDocument();
                LXmlDocumentTableData.Load(LStrTableDataFile);

                XmlNode LXMLNodeTableData = LXmlDocumentTableData.SelectSingleNode("TableData");
                LStrInitSourceTable = LXMLNodeTableData.Attributes["P01"].Value;
                LStrInitLocalTable = LXMLNodeTableData.Attributes["P02"].Value;
                LStrInitRentTable = LXMLNodeTableData.Attributes["P03"].Value;
                LStrInitMethod = LXMLNodeTableData.Attributes["P04"].Value;

                if (LStrInitMethod == "I")
                {
                    if (LStrInitLocalTable == "1")
                    {
                        LStrInitTargetTable = LStrInitSourceTable;
                        LOperationDataArgsReturn = InitTablesDataMethodI(AListStrArguments, LXmlDocumentTableData, LStrInitTargetTable);
                    }
                    if (LStrInitRentTable == "1")
                    {
                        LStrInitTargetTable = LStrInitSourceTable + "_" + AListStrArguments[9];
                        LOperationDataArgsReturn = InitTablesDataMethodI(AListStrArguments, LXmlDocumentTableData, LStrInitTargetTable);
                    }
                }
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "InitTablesData()\n" + ex.Message;
            }

            return LOperationDataArgsReturn;
        }

        /// <summary>
        /// 根据租户信息，更改表中的初始化数据
        /// </summary>
        /// <param name="AListStrDatabaseProfile"></param>
        /// <param name="AListStrRentDataSetted"></param>
        /// <returns></returns>
        public static OperationDataArgs InitTableByRentInfo(List<string> AListStrDatabaseProfile, List<string> AListStrRentDataSetted)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode003 = string.Empty;

            string LStrConnectParam = string.Empty;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            string LStrLoginPassword = string.Empty;
            string LStrDynamicSQL = string.Empty;

            string LStrAdminLoginPassword = string.Empty;

            string LStrStep = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode003 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M003);

                #region 连接到数据库
                LStrStep = "Step 00";
                LStrLoginPassword = EncryptionAndDecryption.EncryptDecryptString(AListStrDatabaseProfile[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", AListStrDatabaseProfile[1], AListStrDatabaseProfile[2], AListStrDatabaseProfile[5], AListStrDatabaseProfile[3], LStrLoginPassword);
                LOracleConnection = new OracleConnection(LStrConnectParam);
                LOracleConnection.Open();
                #endregion

                #region 插入租户
                LStrStep = "Step 01";
                string LStrRentName = EncryptionAndDecryption.EncryptDecryptString(AListStrRentDataSetted[0], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                string LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(AListStrRentDataSetted[1], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "INSERT INTO T_00_121 VALUES(1000000000000000000, '" + LStrRentName + "', '62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2', '9A5898067968489B93CEB7AB3D0D7D1629EABA4BF65117E54BCA8BD252A40EAAEE7F688C8659128943EB17C72C658538', '" + LStrRentToken + "', '2BE3BD86F5887DD2358AF32D148569EB')";
                LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                LOracleCommand.ExecuteNonQuery();
                LOracleCommand.Dispose();
                LOracleCommand = null;
                #endregion

                #region 插入机构
                LStrStep = "Step 02";
                string LStrRootOrgID = "101" + AListStrRentDataSetted[1] + "00000000001";
                LStrDynamicSQL = "INSERT INTO T_11_006_" + AListStrRentDataSetted[1] + " VALUES(" + LStrRootOrgID + ", '" + LStrRentName + "', 0, 0, '1', '0', '11111111111111111111111111111111', '62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2', '6EB61DB7AC715A60440B4938B6F07C91B35A20BB4248DBC8D61ACA150F380F2F', 1020000000000000001, F_00_004(), N'')";
                LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                LOracleCommand.ExecuteNonQuery();
                LOracleCommand.Dispose();
                LOracleCommand = null;
                #endregion

                #region 插入系统管理员用户
                LStrStep = "Step 03";
                string LStrAdminID = "102" + AListStrRentDataSetted[1] + "00000000001";
                DataSet LDataSet11005 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_11_005_" + AListStrRentDataSetted[1] + " WHERE C001 = " + LStrAdminID;
                OracleDataAdapter LOracleDataAdapter1 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                LOracleDataAdapter1.Fill(LDataSet11005, "T_11_005_" + AListStrRentDataSetted[1]);
                LOracleDataAdapter1.Dispose();

                DataRow LDataRowNewData = LDataSet11005.Tables[0].NewRow();
                LDataRowNewData.BeginEdit();
                LDataRowNewData["C001"] = long.Parse(LStrAdminID);
                LDataRowNewData["C002"] = "E53C39BA5A8F9F567F04301860EB1212F685650B15867C77405A988CD8AA7383";
                LDataRowNewData["C003"] = "CAD502458057F534DFFFDE1DB38E124BDA282E41C42C6242EEB80ED92DD21367";
                LStrAdminLoginPassword = EncryptionAndDecryption.EncryptDecryptString(AListStrRentDataSetted[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LDataRowNewData["C004"] = EncryptionAndDecryption.EncryptStringSHA512(LStrAdminID + LStrAdminLoginPassword, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDataRowNewData["C005"] = 20002;
                LDataRowNewData["C006"] = 1010000000000000001;
                LDataRowNewData["C007"] = "S";
                LDataRowNewData["C008"] = "0";
                LDataRowNewData["C009"] = "N";
                LDataRowNewData["C010"] = "1";
                LDataRowNewData["C011"] = "0";
                LDataRowNewData["C012"] = "11111111111111111111111111111111";
                LDataRowNewData["C013"] = EncryptionAndDecryption.EncryptDecryptString("2015/01/01 00:00:00", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002); ;
                LDataRowNewData["C014"] = ""; LDataRowNewData["C015"] = ""; LDataRowNewData["C016"] = 0;
                LDataRowNewData["C017"] = "62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2";
                LDataRowNewData["C018"] = "6EB61DB7AC715A60440B4938B6F07C91B35A20BB4248DBC8D61ACA150F380F2F";
                LDataRowNewData["C019"] = long.Parse(LStrAdminID);
                LDataRowNewData["C020"] = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDataRowNewData["C023"] = (DateTime.Parse("2014/01/01 00:00:00")).ToUniversalTime();
                LDataRowNewData["C024"] = 0;
                LDataRowNewData["C025"] = "1";
                LDataRowNewData.EndEdit();
                LDataSet11005.Tables[0].Rows.Add(LDataRowNewData);

                DataRow LDataRowNewData02 = LDataSet11005.Tables[0].NewRow();
                LDataRowNewData02.BeginEdit();
                LDataRowNewData02["C001"] = long.Parse(LStrAdminID) + 1;
                LDataRowNewData02["C002"] = EncryptionAndDecryption.EncryptDecryptString("UMP.Upload", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002); ;
                LDataRowNewData02["C003"] = EncryptionAndDecryption.EncryptDecryptString("UMP Upload Account", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002); ;
                LDataRowNewData02["C004"] = EncryptionAndDecryption.EncryptDecryptString("Upload.User.123", LStrVerificationCode003, EncryptionAndDecryption.UMPKeyAndIVType.M003);
                LDataRowNewData02["C005"] = 20002;
                LDataRowNewData02["C006"] = 1010000000000000002;
                LDataRowNewData02["C007"] = "H";
                LDataRowNewData02["C008"] = "0";
                LDataRowNewData02["C009"] = "N";
                LDataRowNewData02["C010"] = "1";
                LDataRowNewData02["C011"] = "0";
                LDataRowNewData02["C012"] = "11111111111111111111111111111111";
                LDataRowNewData02["C013"] = EncryptionAndDecryption.EncryptDecryptString("2015/01/01 00:00:00", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002); ;
                LDataRowNewData02["C014"] = ""; LDataRowNewData["C015"] = ""; LDataRowNewData["C016"] = 0;
                LDataRowNewData02["C017"] = "62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2";
                LDataRowNewData02["C018"] = "6EB61DB7AC715A60440B4938B6F07C91B35A20BB4248DBC8D61ACA150F380F2F";
                LDataRowNewData02["C019"] = long.Parse(LStrAdminID);
                LDataRowNewData02["C020"] = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDataRowNewData02["C023"] = (DateTime.Parse("2014/01/01 00:00:00")).ToUniversalTime();
                LDataRowNewData02["C024"] = 0;
                LDataRowNewData02["C025"] = "1";
                LDataRowNewData02.EndEdit();
                LDataSet11005.Tables[0].Rows.Add(LDataRowNewData02);

                OracleDataAdapter LOracleDataAdapter2 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();
                LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                LOracleCommandBuilder.SetAllValues = false;
                LOracleCommandBuilder.DataAdapter = LOracleDataAdapter2;
                LOracleDataAdapter2.Update(LDataSet11005, "T_11_005_" + AListStrRentDataSetted[1]);
                LDataSet11005.AcceptChanges();
                LOracleCommandBuilder.Dispose();
                LOracleDataAdapter2.Dispose();
                #endregion

                #region 插入默认机构类型
                LStrStep = "Step 04";
                string LStrOrgRootID = "905" + AListStrRentDataSetted[1] + "00000000000";
                DataSet LDataSet11009 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStrRentDataSetted[1] + " WHERE C001 = " + LStrAdminID;
                OracleDataAdapter LOracleDataAdapterOld = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                LOracleDataAdapterOld.Fill(LDataSet11009, "T_11_009_" + AListStrRentDataSetted[1]);
                LOracleDataAdapterOld.Dispose();

                DataRow LDataRowNew905 = LDataSet11009.Tables[0].NewRow();
                LDataRowNew905.BeginEdit();
                LDataRowNew905["C000"] = 1;
                LDataRowNew905["C001"] = long.Parse(LStrOrgRootID);
                LDataRowNew905["C002"] = 1;
                LDataRowNew905["C003"] = 0;
                LDataRowNew905["C004"] = "1";
                LDataRowNew905["C005"] = 2;
                LDataRowNew905["C006"] = EncryptionAndDecryption.EncryptDecryptString("InitData", LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDataRowNew905["C007"] = LStrAdminID;
                LDataRowNew905["C008"] = "";
                LDataRowNew905["C009"] = "";
                LDataRowNew905["C010"] = "";
                LDataRowNew905["C011"] = "";
                LDataRowNew905.EndEdit();
                LDataSet11009.Tables[0].Rows.Add(LDataRowNew905);

                OracleDataAdapter LOracleDataAdapter11009New = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                OracleCommandBuilder LOracleCommandBuilder11009 = new OracleCommandBuilder();
                LOracleCommandBuilder11009.ConflictOption = ConflictOption.OverwriteChanges;
                LOracleCommandBuilder11009.SetAllValues = false;
                LOracleCommandBuilder11009.DataAdapter = LOracleDataAdapter11009New;
                LOracleDataAdapter11009New.Update(LDataSet11009, "T_11_009_" + AListStrRentDataSetted[1]);
                LDataSet11009.AcceptChanges();
                LOracleCommandBuilder11009.Dispose();
                LOracleDataAdapter11009New.Dispose();

                #endregion

                #region 插入密码变更记录
                LStrStep = "Step 05";

                DataSet LDataSet00002 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_00_002 WHERE 1 = 2";
                OracleDataAdapter LOracleDataAdapter11 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                LOracleDataAdapter11.Fill(LDataSet00002, "T_00_002");
                LOracleDataAdapter11.Dispose();

                DataRow LDataRowNew002 = LDataSet00002.Tables[0].NewRow();
                LDataRowNew002.BeginEdit();
                LDataRowNew002["C000"] = AListStrRentDataSetted[1];
                LDataRowNew002["C001"] = long.Parse(LStrAdminID);
                LDataRowNew002["C002"] = 102;
                LDataRowNew002["C003"] = "C004";
                LDataRowNew002["C004"] = DateTime.UtcNow;
                LDataRowNew002["C005"] = CreateSpliterCharater();
                LDataRowNew002["C006"] = EncryptionAndDecryption.EncryptStringSHA512(LStrAdminID + LStrAdminLoginPassword, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LDataRowNew002["C007"] = "2002";
                LDataRowNew002["C008"] = long.Parse(LStrAdminID);
                LDataRowNew002.EndEdit();
                LDataSet00002.Tables[0].Rows.Add(LDataRowNew002);

                OracleDataAdapter LOracleDataAdapter00002New = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                OracleCommandBuilder LOracleCommandBuilder00002 = new OracleCommandBuilder();
                LOracleCommandBuilder00002.ConflictOption = ConflictOption.OverwriteChanges;
                LOracleCommandBuilder00002.SetAllValues = false;
                LOracleCommandBuilder00002.DataAdapter = LOracleDataAdapter00002New;
                LOracleDataAdapter00002New.Update(LDataSet00002, "T_00_002");
                LDataSet00002.AcceptChanges();
                LOracleCommandBuilder00002.Dispose();
                LOracleDataAdapter00002New.Dispose();
                #endregion

                #region 删除角色 1060000000000000002，1060000000000000003
                LStrDynamicSQL = "DELETE FROM T_11_004_" + AListStrRentDataSetted[1] + " WHERE C001 = 1060000000000000002 OR C001 = 1060000000000000003";
                LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                LOracleCommand.ExecuteNonQuery();
                LOracleCommand.Dispose();
                LOracleCommand = null;
                #endregion

                #region 删除角色包含的用户
                LStrDynamicSQL = "DELETE FROM T_11_201_" + AListStrRentDataSetted[1] + " WHERE C003 = 1060000000000000002 OR C003 = 1060000000000000003";
                LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                LOracleCommand.ExecuteNonQuery();
                LOracleCommand.Dispose();
                LOracleCommand = null;
                #endregion

                #region 删除角色拥有的功能操作权限
                LStrDynamicSQL = "DELETE FROM T_11_202_" + AListStrRentDataSetted[1] + " WHERE C001 = 1060000000000000002 OR C001 = 1060000000000000003";
                LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                LOracleCommand.ExecuteNonQuery();
                LOracleCommand.Dispose();
                LOracleCommand = null;
                #endregion
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "InitTableByRentInfo()\n" + LStrStep + "\n" + ex.ToString();
            }
            finally
            {
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        private static OperationDataArgs InitTablesDataMethodI(List<string> AListStrArguments, XmlDocument AXmlDocumentTableData, string AStrTargetTable)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();

            string LStrVerificationCode104 = string.Empty;

            List<string> LListStrDatabaseProfile = new List<string>();
            string LStrConnectParam = string.Empty;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            string LStrDynamicSQL = string.Empty;

            List<string> LListStrArgs = new List<string>();
            List<string> LListStrValues = new List<string>();
            List<string> LListStrDataType = new List<string>();
            List<int> LListIntDataLength = new List<int>();
            List<int> LListIntPrecision = new List<int>();
            List<int> LListIntScale = new List<int>();
            List<string> LListStrNullAble = new List<string>();

            int LIntAllImportRows = 0, LIntEffectRows = 0;
            string LStrSQLCommand = string.Empty;

            string LStrColumnName = string.Empty;
            string LStrColumnValue = string.Empty;
            string LStrColumnType = string.Empty;

            int LIntLoopColumn, LIntAllColumn;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 数据库连接
                LListStrDatabaseProfile.Add(AListStrArguments[2]);
                LListStrDatabaseProfile.Add(AListStrArguments[3]);
                LListStrDatabaseProfile.Add(AListStrArguments[4]);
                LListStrDatabaseProfile.Add(EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrDatabaseProfile.Add(AListStrArguments[6]);

                LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDatabaseProfile[0], LListStrDatabaseProfile[1], LListStrDatabaseProfile[4], LListStrDatabaseProfile[2], LListStrDatabaseProfile[3]);
                LOracleConnection = new OracleConnection(LStrConnectParam);
                LOracleConnection.Open();
                #endregion

                #region 获取表各字段的基本信息
                DataSet LDataSetTable = new DataSet();
                LStrDynamicSQL = "SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + AStrTargetTable + "'";
                OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                LOracleDataAdapter.Fill(LDataSetTable, AStrTargetTable);
                #endregion

                #region 向数据库中插入数据
                XmlNodeList LXMLNodeTableDataRowsList = AXmlDocumentTableData.SelectSingleNode("TableData").SelectSingleNode("RowsList").ChildNodes;
                foreach (XmlNode LXmlNodeSingleTableRow in LXMLNodeTableDataRowsList)
                {
                    LListStrArgs.Clear(); LListStrValues.Clear(); LListStrDataType.Clear(); LListIntDataLength.Clear();
                    LListIntPrecision.Clear(); LListIntScale.Clear(); LListStrNullAble.Clear();

                    LStrSQLCommand = "INSERT INTO " + AStrTargetTable + "(";
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeSingleTableRow.ChildNodes)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Name.ToUpper();
                        LStrColumnValue = LXmlNodeSingleColumn.InnerText;
                        LStrSQLCommand += LStrColumnName;
                        if (LXmlNodeSingleColumn.NextSibling != null) { LStrSQLCommand += ", "; }
                        LListStrArgs.Add(":Arg" + LStrColumnName);
                        LListStrValues.Add(LStrColumnValue);
                        LStrColumnType = LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[1].ToString();
                        LListStrDataType.Add(LStrColumnType);
                        LListIntDataLength.Add(int.Parse(LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[2].ToString()));
                        if (LStrColumnType == "NUMBER")
                        {
                            LListIntPrecision.Add(int.Parse(LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[3].ToString()));
                            LListIntScale.Add(int.Parse(LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[4].ToString()));
                        }
                        else
                        {
                            LListIntPrecision.Add(0); LListIntScale.Add(0);
                        }
                        LListStrNullAble.Add(LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[4].ToString());
                    }

                    LStrSQLCommand += ") VALUES (";
                    foreach (string LStrArgs in LListStrArgs) { LStrSQLCommand += LStrArgs + ", "; }
                    LStrSQLCommand = LStrSQLCommand.Substring(0, LStrSQLCommand.Length - 2) + ")";

                    OracleCommand LOracleCommandInsert = new OracleCommand(LStrSQLCommand, LOracleConnection);

                    LIntAllColumn = LListStrArgs.Count;
                    for (LIntLoopColumn = 0; LIntLoopColumn < LIntAllColumn; LIntLoopColumn++)
                    {
                        LStrColumnType = LListStrDataType[LIntLoopColumn];
                        OracleParameter LOracleParam = new OracleParameter();
                        LOracleParam.ParameterName = LListStrArgs[LIntLoopColumn].Substring(1);
                        switch (LStrColumnType)
                        {
                            case "NUMBER":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = "-1"; }
                                if (LListIntScale[LIntLoopColumn] > 0)
                                {
                                    LOracleParam.OracleDbType = OracleDbType.Decimal;
                                    LOracleParam.Value = decimal.Parse(LListStrValues[LIntLoopColumn]);
                                }
                                else
                                {
                                    if (LListIntPrecision[LIntLoopColumn] > 10)
                                    {
                                        LOracleParam.OracleDbType = OracleDbType.Int64;
                                        LOracleParam.Size = 8;
                                        LOracleParam.Value = Int64.Parse(LListStrValues[LIntLoopColumn]);
                                    }
                                    else
                                    {
                                        LOracleParam.OracleDbType = OracleDbType.Int32;
                                        LOracleParam.Size = 4;
                                        LOracleParam.Value = Int32.Parse(LListStrValues[LIntLoopColumn]);
                                    }
                                }
                                break;
                            case "NVARCHAR2":
                                LOracleParam.OracleDbType = OracleDbType.NVarchar2;
                                LOracleParam.Size = LListIntDataLength[LIntLoopColumn];
                                LOracleParam.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "VARCHAR2":
                                LOracleParam.OracleDbType = OracleDbType.Varchar2;
                                LOracleParam.Size = LListIntDataLength[LIntLoopColumn];
                                LOracleParam.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "CHAR":
                                LOracleParam.OracleDbType = OracleDbType.Char;
                                LOracleParam.Size = LListIntDataLength[LIntLoopColumn];
                                LOracleParam.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "NCHAR":
                                LOracleParam.OracleDbType = OracleDbType.NChar;
                                LOracleParam.Size = LListIntDataLength[LIntLoopColumn];
                                LOracleParam.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "DATE":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); }
                                LOracleParam.OracleDbType = OracleDbType.Date;
                                LOracleParam.Value = DateTime.Parse(LListStrValues[LIntLoopColumn]);
                                break;
                            default:
                                continue;
                        }
                        LOracleCommandInsert.Parameters.Add(LOracleParam);
                    }
                    LIntEffectRows = LOracleCommandInsert.ExecuteNonQuery();
                    LOracleCommandInsert.Dispose(); LOracleCommandInsert = null;
                    LIntAllImportRows += LIntEffectRows;
                }
                #endregion

                LOperationDataArgsReturn.StringReturn = LIntAllImportRows.ToString();
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "InitTablesDataMethodI()\n" + LStrSQLCommand + "\n" + LStrColumnValue + ex.Message;
            }
            finally
            {
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        private static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
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

        /// <summary>
        /// 生成系统SpliterCharater,结果为字符char(27)
        /// </summary>
        /// <returns></returns>
        private static string CreateSpliterCharater()
        {
            string LStrSpliter = string.Empty;

            try
            {
                System.Text.ASCIIEncoding LAsciiEncoding = new System.Text.ASCIIEncoding();
                byte[] LByteArray = new byte[] { (byte)27 };
                string LStrCharacter = LAsciiEncoding.GetString(LByteArray);
                LStrSpliter = LStrCharacter;
            }
            catch { LStrSpliter = string.Empty; }

            return LStrSpliter;
        }
    }
}