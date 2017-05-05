using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Wcf00001
{
    public class DatabaseType2Operation
    {
        public static OperationDataArgs ConnectToDatabase(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;

            string LStrVerificationCode104 = string.Empty;
            string LStrServerName = string.Empty;
            string LStrServerPort = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPwd = string.Empty;

            string LStrDataFileFolder = string.Empty;
            string LStrLogFileFolder = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerName = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerPort = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginID = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginPwd = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LSQLServerConnection = new ServerConnection(LStrServerName + "," + LStrServerPort, LStrLoginID, LStrLoginPwd);
                Server LServerClass = new Server(LSQLServerConnection);
                LStrDataFileFolder = LServerClass.Settings.DefaultFile;
                LStrLogFileFolder = LServerClass.Settings.DefaultLog;
                if (string.IsNullOrEmpty(LStrDataFileFolder)) { LStrDataFileFolder = LServerClass.Information.MasterDBPath; }
                if (string.IsNullOrEmpty(LStrLogFileFolder)) { LStrLogFileFolder = LServerClass.Information.MasterDBLogPath; }
                LStrDataFileFolder = LStrDataFileFolder + @"\"; LStrLogFileFolder = LStrLogFileFolder + @"\";
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.NetName);       //0
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.Collation);     //1
                LOperationDataArgsReturn.ListStringReturn.Add(LStrDataFileFolder);                     //2
                LOperationDataArgsReturn.ListStringReturn.Add(LStrLogFileFolder);                      //3
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.Version.Major.ToString());      //4
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.Product);       //5
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.ProductLevel);  //6
                LOperationDataArgsReturn.ListStringReturn.Add(LServerClass.Information.Edition);       //7
                DataTable LDataTableEnumCollations = LServerClass.EnumCollations();
                LOperationDataArgsReturn.DataSetReturn.Tables.Add(LDataTableEnumCollations);
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ConnectToDatabase()\n" + ex.Message;
            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs GetSubdirectories(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;

            string LStrVerificationCode104 = string.Empty;
            string LStrServerName = string.Empty;
            string LStrServerPort = string.Empty;
            string LStrLoginID = string.Empty;
            string LStrLoginPwd = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerName = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerPort = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginID = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrLoginPwd = EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LSQLServerConnection = new ServerConnection(LStrServerName + "," + LStrServerPort, LStrLoginID, LStrLoginPwd);
                Server LServerClass = new Server(LSQLServerConnection);

                LOperationDataArgsReturn.DataSetReturn.Tables.Clear();

                DataTable LDataTableEnumAvailableMediaOrEnumDirectories = null;
                if (string.IsNullOrEmpty(AListStrArguments[4]))
                {
                    LDataTableEnumAvailableMediaOrEnumDirectories = LServerClass.EnumAvailableMedia(MediaTypes.FixedDisk);
                }
                else
                {
                    LDataTableEnumAvailableMediaOrEnumDirectories = LServerClass.EnumDirectories(AListStrArguments[4]);
                }
                LOperationDataArgsReturn.DataSetReturn.Tables.Add(LDataTableEnumAvailableMediaOrEnumDirectories);

            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "GetSubdirectories()\n" + ex.Message;
            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs CreateDatabase(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;

            string LStrVerificationCode104 = string.Empty;

            List<string> LListStrDBConnectProfile = new List<string>();
            List<string> LListStrCreateDBOptions = new List<string>();
            bool LBoolMatchedSpliterCharacter = false;
            string LStrSpliterCharacter = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrSpliterCharacter = CreateSpliterCharater();

                foreach (string LStrSingleArgument in AListStrArguments)
                {
                    if (LStrSingleArgument == LStrSpliterCharacter) { LBoolMatchedSpliterCharacter = true; continue; }
                    if (!LBoolMatchedSpliterCharacter)
                    {
                        LListStrDBConnectProfile.Add(EncryptionAndDecryption.EncryptDecryptString(LStrSingleArgument, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                    }
                    else { LListStrCreateDBOptions.Add(LStrSingleArgument); }
                }

                LSQLServerConnection = new ServerConnection(LListStrDBConnectProfile[0] + "," + LListStrDBConnectProfile[1], LListStrDBConnectProfile[2], LListStrDBConnectProfile[3]);
                Server LServerClass = new Server(LSQLServerConnection);

                //检查数据库是否已经存在，如果存在且选择了“覆盖原有数据库”，则删除数据库，否则返回 “TP0001”
                foreach (Database LDatabaseExist in LServerClass.Databases)
                {
                    if (LDatabaseExist.Name == LListStrCreateDBOptions[0])
                    {
                        //数据库存在且不覆盖
                        if (LListStrCreateDBOptions[1] != "1")
                        {
                            LOperationDataArgsReturn.BoolReturn = false;
                            LOperationDataArgsReturn.ListStringReturn.Add("TP0001");
                            return LOperationDataArgsReturn;
                        }

                        //数据库存在，删除该数据库
                        if (!KillMSSQLProcess(LServerClass, LListStrCreateDBOptions[0], ref LStrCallReturn))
                        {
                            LOperationDataArgsReturn.BoolReturn = false;
                            LOperationDataArgsReturn.StringReturn = LStrCallReturn;
                            return LOperationDataArgsReturn;
                        }
                        LServerClass.KillDatabase(LListStrCreateDBOptions[0]);
                        break;
                    }
                }

                //开始创建数据库
                Database LDatabaseNew = new Database(LServerClass, LListStrCreateDBOptions[0]);
                //数据库的排序规则
                LDatabaseNew.Collation = LListStrCreateDBOptions[2];
                //数据库的恢复模式
                if (LListStrCreateDBOptions[3] == "1") { LDatabaseNew.RecoveryModel = RecoveryModel.Full; }
                if (LListStrCreateDBOptions[3] == "2") { LDatabaseNew.RecoveryModel = RecoveryModel.BulkLogged; }
                if (LListStrCreateDBOptions[3] == "3") { LDatabaseNew.RecoveryModel = RecoveryModel.Simple; }

                //数据文件
                FileGroup LFileGroupPrimary = new FileGroup(LDatabaseNew, "PRIMARY");
                LDatabaseNew.FileGroups.Add(LFileGroupPrimary);
                DataFile LDataFileMain = new DataFile(LFileGroupPrimary, LListStrCreateDBOptions[0] + "Data", System.IO.Path.Combine(LListStrCreateDBOptions[4], LListStrCreateDBOptions[0] + "Data.mdf"));
                LDataFileMain.Size = int.Parse(LListStrCreateDBOptions[5]) * 1024;
                if (LListStrCreateDBOptions[6] != "0") { LDataFileMain.MaxSize = int.Parse(LListStrCreateDBOptions[6]) * 1024; }
                if (LListStrCreateDBOptions[7] == "M")
                { LDataFileMain.GrowthType = FileGrowthType.KB; LDataFileMain.Growth = int.Parse(LListStrCreateDBOptions[8]) * 1024; }
                else { LDataFileMain.GrowthType = FileGrowthType.Percent; LDataFileMain.Growth = int.Parse(LListStrCreateDBOptions[8]); }
                LFileGroupPrimary.Files.Add(LDataFileMain);

                //日志文件
                LogFile LLogFileMain = new LogFile(LDatabaseNew, LListStrCreateDBOptions[0] + "Log", System.IO.Path.Combine(LListStrCreateDBOptions[9], LListStrCreateDBOptions[0] + "Log.ldf"));
                LLogFileMain.Size = int.Parse(LListStrCreateDBOptions[10]) * 1024;
                if (LListStrCreateDBOptions[11] != "0") { LLogFileMain.MaxSize = int.Parse(LListStrCreateDBOptions[11]) * 1024; }
                if (LListStrCreateDBOptions[12] == "M")
                { LLogFileMain.GrowthType = FileGrowthType.KB; LLogFileMain.Growth = int.Parse(LListStrCreateDBOptions[13]) * 1024; }
                else { LLogFileMain.GrowthType = FileGrowthType.Percent; LLogFileMain.Growth = int.Parse(LListStrCreateDBOptions[13]); }
                LDatabaseNew.LogFiles.Add(LLogFileMain);

                LDatabaseNew.Create();
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "CreateDatabase()\n" + ex.Message;
                LOperationDataArgsReturn.ListDataSetReturn.Clear();
            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs CreateLoginAccount(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;

            string LStrVerificationCode104 = string.Empty;
            List<string> LListStrDBConnectProfile = new List<string>();
            List<string> LListStrCreateLoginAccount = new List<string>();
            bool LBoolMatchedSpliterCharacter = false;
            string LStrSpliterCharacter = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrSpliterCharacter = CreateSpliterCharater();

                foreach (string LStrSingleArgument in AListStrArguments)
                {
                    if (LStrSingleArgument == LStrSpliterCharacter) { LBoolMatchedSpliterCharacter = true; continue; }
                    if (!LBoolMatchedSpliterCharacter)
                    {
                        LListStrDBConnectProfile.Add(EncryptionAndDecryption.EncryptDecryptString(LStrSingleArgument, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                    }
                    else { LListStrCreateLoginAccount.Add(LStrSingleArgument); }
                }

                LSQLServerConnection = new ServerConnection(LListStrDBConnectProfile[0] + "," + LListStrDBConnectProfile[1], LListStrDBConnectProfile[2], LListStrDBConnectProfile[3]);
                Server LServerClass = new Server(LSQLServerConnection);

                //检测将要创建的用户是否已经存在
                foreach (Login LLoginExsit in LServerClass.Logins)
                {
                    if (LLoginExsit.Name == LListStrCreateLoginAccount[0])
                    {
                        LOperationDataArgsReturn.BoolReturn = false;
                        LOperationDataArgsReturn.ListStringReturn.Add("TP0002");
                        return LOperationDataArgsReturn;
                    }
                }

                //创建Login
                Login LLoginNew = new Login(LServerClass, LListStrCreateLoginAccount[0]);
                LLoginNew.LoginType = LoginType.SqlLogin;
                if (LListStrCreateLoginAccount[2] == "1") { LLoginNew.PasswordPolicyEnforced = true; } else { LLoginNew.PasswordPolicyEnforced = false; }
                if (!string.IsNullOrEmpty(LListStrCreateLoginAccount[3])) { LLoginNew.DefaultDatabase = LListStrCreateLoginAccount[3]; }
                LLoginNew.Create(LListStrCreateLoginAccount[1], LoginCreateOptions.None);

                //创建用户映射，如果数据库是新创建的
                if (!string.IsNullOrEmpty(LListStrCreateLoginAccount[3]))
                {
                    Database LDatabaseCreated = LServerClass.Databases[LListStrCreateLoginAccount[3]];
                    User LUserNew = new User(LDatabaseCreated, LListStrCreateLoginAccount[0]);
                    LUserNew.Login = LListStrCreateLoginAccount[0];
                    LUserNew.Create();
                    LUserNew.AddToRole("db_owner");
                }

                //增加到系统角色
                foreach (ServerRole LServerRole in LServerClass.Roles)
                {
                    if (LServerRole.Name == "serveradmin" && LListStrCreateLoginAccount[5] == "1")
                    {
                        LServerRole.AddMember(LListStrCreateLoginAccount[0]); continue;
                    }
                    if (LServerRole.Name == "sysadmin" && LListStrCreateLoginAccount[4] == "1")
                    {
                        LServerRole.AddMember(LListStrCreateLoginAccount[0]); continue;
                    }
                }
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "CreateLoginAccount()\n" + ex.Message;
            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs ObtainDatabasesLogins(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;
            string LStrVerificationCode104 = string.Empty;
            List<string> LListStrDatabaseProfile = new List<string>();

            string LStrDatabase = string.Empty;
            string LStrLoginAccount = string.Empty;

            List<string> LListStrSysAdmin = new List<string>();
            bool LBoolLoginExist = false;

            try
            {
                LListStrDatabaseProfile = new List<string>(AListStrArguments.ToArray());
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[0] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[1] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[2] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[3] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LSQLServerConnection = new ServerConnection(LListStrDatabaseProfile[0] + "," + LListStrDatabaseProfile[1], LListStrDatabaseProfile[2], LListStrDatabaseProfile[3]);
                Server LServerClass = new Server(LSQLServerConnection);

                foreach (Login LLoginExsit in LServerClass.Logins)
                {
                    if (!LLoginExsit.IsMember("sysadmin")) { continue; }
                    if (LLoginExsit.LoginType != LoginType.SqlLogin) { continue; }
                    LStrLoginAccount = LLoginExsit.Name;
                    LListStrSysAdmin.Add(LStrLoginAccount);
                }

                foreach (Database LDatabaseExist in LServerClass.Databases)
                {
                    if (LDatabaseExist.IsSystemObject) { continue; }
                    LStrDatabase = LDatabaseExist.Name;
                    if (LStrDatabase.Contains("ReportServer")) { continue; }
                    LOperationDataArgsReturn.ListStringReturn.Add(LStrDatabase);
                    DataTable LDataTableTargetLogin = new DataTable(LStrDatabase);
                    LDataTableTargetLogin.Columns.Add("ColLogin", typeof(string));
                    DataTable LDataTableLoginMappings = LDatabaseExist.EnumLoginMappings();
                    foreach (DataRow LDataRowLogin in LDataTableLoginMappings.Rows)
                    {
                        DataRow LDataRowUser = LDataTableTargetLogin.NewRow();
                        LDataRowUser.BeginEdit();
                        LDataRowUser[0] = LDataRowLogin[1].ToString();
                        LDataRowUser.EndEdit();
                        LDataTableTargetLogin.Rows.Add(LDataRowUser);
                    }
                    foreach (string LStrSysAdminLogin in LListStrSysAdmin)
                    {
                        LBoolLoginExist = false;
                        foreach (DataRow LDataRowLogin in LDataTableTargetLogin.Rows)
                        {
                            if (LDataRowLogin[0].ToString() == LStrSysAdminLogin) { LBoolLoginExist = true; }
                        }
                        if (!LBoolLoginExist)
                        {
                            DataRow LDataRowUser = LDataTableTargetLogin.NewRow();
                            LDataRowUser.BeginEdit();
                            LDataRowUser[0] = LStrSysAdminLogin;
                            LDataRowUser.EndEdit();
                            LDataTableTargetLogin.Rows.Add(LDataRowUser);
                        }
                    }
                    LOperationDataArgsReturn.DataSetReturn.Tables.Add(LDataTableTargetLogin);
                }
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ObtainDatabasesLogins()\n" + ex.Message;
            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        public static OperationDataArgs Connect2SpecifiedDatabaseObtainCurrentVersion(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            ServerConnection LSQLServerConnection = null;
            string LStrVerificationCode104 = string.Empty;
            List<string> LListStrDatabaseProfile = new List<string>();

            string LStrSelectC000 = string.Empty;

            try
            {
                //0-数据库服务器
                //1-端口
                //2-LogingID
                //3-Login Password
                //4-数据库名
                //5-系统管理员LoginID
                //6-系统管理员Login Password
                LListStrDatabaseProfile = new List<string>(AListStrArguments.ToArray());
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LListStrDatabaseProfile[3] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[5] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LListStrDatabaseProfile[6] = EncryptionAndDecryption.EncryptDecryptString(LListStrDatabaseProfile[6], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LSQLServerConnection = new ServerConnection(LListStrDatabaseProfile[0] + "," + LListStrDatabaseProfile[1], LListStrDatabaseProfile[5], LListStrDatabaseProfile[6]);
                Server LServerClass = new Server(LSQLServerConnection);
                Database LDatabaseTarget = LServerClass.Databases[LListStrDatabaseProfile[4]];
                Table LTableTarget = LDatabaseTarget.Tables["T_00_000"];
                LOperationDataArgsReturn.ListStringReturn.Add("0.00.000");
                if (LTableTarget != null)
                {
                    LOperationDataArgsReturn.DataSetReturn = LDatabaseTarget.ExecuteWithResults("SELECT * FROM T_00_000");
                    LStrSelectC000 = CreateSpliterCharater() + CreateSpliterCharater() + CreateSpliterCharater() + CreateSpliterCharater() + "1";
                    DataRow[] LDataRowSelected = LOperationDataArgsReturn.DataSetReturn.Tables[0].Select("C000 = '" + LStrSelectC000 + "'");
                    if (LDataRowSelected.Length == 1)
                    {
                        LOperationDataArgsReturn.ListStringReturn[0] = LDataRowSelected[0]["C002"].ToString();
                    }
                }
                AddLoginIDIntoRoleDBOwner(LListStrDatabaseProfile);
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "Connect2SpecifiedDatabaseObtainCurrentVersion()\n" + ex.Message;

                try
                {
                    if (ex.GetType() == typeof(System.Data.SqlClient.SqlException))
                    {
                        SqlException LSqlException = ex.InnerException as SqlException;
                        SqlErrorCollection LSqlErrorCollection = LSqlException.Errors;
                        SqlError LSqlError = LSqlErrorCollection[0];
                        if (LSqlError.Number == 4060) { LOperationDataArgsReturn.ListStringReturn.Clear(); LOperationDataArgsReturn.ListStringReturn.Add("TP0003"); }
                        if (LSqlError.Number == 18456) { LOperationDataArgsReturn.ListStringReturn.Clear(); LOperationDataArgsReturn.ListStringReturn.Add("TP0004"); }
                        if (LSqlError.Number == 10060) { LOperationDataArgsReturn.ListStringReturn.Clear(); LOperationDataArgsReturn.ListStringReturn.Add("TP0005"); }
                    }
                }
                catch { }

            }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
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

                    LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrCreateTableName + "', '" + LStrTabelVersion + "', 'TB', '1', GETDATE(), '" + LStrCanPartition + "', 'NULL')";
                    LStrCreateOrAlterSQL.Add(LStr00000DynamicSQL);
                    #endregion

                    #region 给表中字段加 Default Value
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrDefaultValue = LXmlNodeSingleColumn.Attributes["P06"].Value.Trim();
                        if (string.IsNullOrEmpty(LStrDefaultValue)) { continue; }
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;

                        LStrDefValueName = "DF_" + LStrCreateTableName.Substring(2) + "_" + LStrColumnName;
                        LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD CONSTRAINT " + LStrDefValueName + " DEFAULT (" + LStrDefaultValue + ") FOR " + LStrColumnName;
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                        LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrDefValueName + "', '" + LStrTabelVersion + "', 'DF', '1', GETDATE(), '" + LStrCanPartition + "', '" + LStrCreateTableName + "')";
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
                        if (LStrIsPrimaryKey == "1") { LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD  CONSTRAINT " + LStrCreateIndexName + " PRIMARY KEY CLUSTERED("; LStrIndexType = "PK"; }
                        else
                        {
                            if (LStrIsUnique == "1") { LStrDDLSql = "CREATE UNIQUE NONCLUSTERED INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + "("; }
                            else { LStrDDLSql = "CREATE NONCLUSTERED INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + "("; }
                            LStrIndexType = "IX";
                        }
                        foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                        {
                            LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                            LStrOrderType = LXmlNodeSingleIndexColumn.Attributes["P02"].Value;
                            if (LStrOrderType == "A") { LStrDDLSql += LStrIndexColumn + " ASC"; } else { LStrDDLSql += LStrIndexColumn + " DESC"; }
                            if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                        }
                        LStrDDLSql += ")";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);

                        LStr00000DynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AListStrArguments[9] + "', '" + LStrCreateIndexName + "', '" + LStrTabelVersion + "', '" + LStrIndexType + "', '1', GETDATE(), '0', '" + LStrCreateTableName + "')";
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

                string LStrObjectCreateSQL = File.ReadAllText(LStrTableObjectFile, Encoding.UTF8);
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

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            string LStrConnectParam = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LListStrDatabaseProfile.Add(AListStrArguments[2]);
                LListStrDatabaseProfile.Add(AListStrArguments[3]);
                LListStrDatabaseProfile.Add(AListStrArguments[4]);
                LListStrDatabaseProfile.Add(EncryptionAndDecryption.EncryptDecryptString(AListStrArguments[5], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                LListStrDatabaseProfile.Add(AListStrArguments[6]);

                LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDatabaseProfile[0], LListStrDatabaseProfile[1], LListStrDatabaseProfile[4], LListStrDatabaseProfile[2], LListStrDatabaseProfile[3]);
                LSqlConnection = new SqlConnection(LStrConnectParam);
                LSqlConnection.Open();
                foreach (string LStrSingleDynamicSQL in AListStrDDLSql)
                {
                    LOperationDataArgsReturn.ListStringReturn.Add(LStrSingleDynamicSQL);
                    LSqlCommand = new SqlCommand(LStrSingleDynamicSQL, LSqlConnection);
                    LSqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException LSqlException)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ExecuteDynamicSql()\n" + LSqlException.Message;
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ExecuteDynamicSql()\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
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
            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;

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
                LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", AListStrDatabaseProfile[1], AListStrDatabaseProfile[2], AListStrDatabaseProfile[5], AListStrDatabaseProfile[3], LStrLoginPassword);
                LSqlConnection = new SqlConnection(LStrConnectParam);
                LSqlConnection.Open();
                #endregion

                #region 插入租户
                LStrStep = "Step 01";
                string LStrRentName = EncryptionAndDecryption.EncryptDecryptString(AListStrRentDataSetted[0], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                string LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(AListStrRentDataSetted[1], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrDynamicSQL = "INSERT INTO T_00_121 VALUES(1000000000000000000, '" + LStrRentName + "', '62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2', '9A5898067968489B93CEB7AB3D0D7D1629EABA4BF65117E54BCA8BD252A40EAAEE7F688C8659128943EB17C72C658538', '" + LStrRentToken + "', '2BE3BD86F5887DD2358AF32D148569EB')";
                LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                LSqlCommand.ExecuteNonQuery();
                LSqlCommand.Dispose();
                LSqlCommand = null;
                #endregion

                #region 插入机构
                LStrStep = "Step 02";
                string LStrRootOrgID = "101" + AListStrRentDataSetted[1] + "00000000001";
                LStrDynamicSQL = "INSERT INTO T_11_006_" + AListStrRentDataSetted[1] + " VALUES(" + LStrRootOrgID + ", '" + LStrRentName + "', 0, 0, '1', '0', '11111111111111111111111111111111', '62E1F5CC29EC44DCFA48C0984AAC4841A6E7E01779E987E1BC26375CE3486FE60523BBB411AA00CA7C075AFB78D735E2', '6EB61DB7AC715A60440B4938B6F07C91B35A20BB4248DBC8D61ACA150F380F2F', 1020000000000000001, GETUTCDATE(), N'')";
                LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                LSqlCommand.ExecuteNonQuery();
                LSqlCommand.Dispose();
                LSqlCommand = null;
                #endregion

                #region 插入系统管理员用户、文件上传默认用户
                LStrStep = "Step 03";
                string LStrAdminID = "102" + AListStrRentDataSetted[1] + "00000000001";
                DataSet LDataSet11005 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_11_005_" + AListStrRentDataSetted[1] + " WHERE C001 = " + LStrAdminID;
                SqlDataAdapter LSqlDataAdapter1 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                LSqlDataAdapter1.Fill(LDataSet11005, "T_11_005_" + AListStrRentDataSetted[1]);
                LSqlDataAdapter1.Dispose();
                
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

                SqlDataAdapter LSqlDataAdapter2 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                LSqlCommandBuilder.SetAllValues = false;
                LSqlCommandBuilder.DataAdapter = LSqlDataAdapter2;
                LSqlDataAdapter2.Update(LDataSet11005, "T_11_005_" + AListStrRentDataSetted[1]);
                LDataSet11005.AcceptChanges();
                LSqlCommandBuilder.Dispose();
                LSqlDataAdapter2.Dispose();
                #endregion

                #region 插入默认机构类型
                LStrStep = "Step 04";
                string LStrOrgRootID = "905" + AListStrRentDataSetted[1] + "00000000000";
                DataSet LDataSet11009 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_11_009_" + AListStrRentDataSetted[1] + " WHERE C001 = " + LStrAdminID;
                SqlDataAdapter LSqlDataAdapter11009Old = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                LSqlDataAdapter11009Old.Fill(LDataSet11009, "T_11_009_" + AListStrRentDataSetted[1]);
                LSqlDataAdapter11009Old.Dispose();

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

                SqlDataAdapter LSqlDataAdapter11009New = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                SqlCommandBuilder LSqlCommandBuilder11009 = new SqlCommandBuilder();
                LSqlCommandBuilder11009.ConflictOption = ConflictOption.OverwriteChanges;
                LSqlCommandBuilder11009.SetAllValues = false;
                LSqlCommandBuilder11009.DataAdapter = LSqlDataAdapter11009New;
                LSqlDataAdapter11009New.Update(LDataSet11009, "T_11_009_" + AListStrRentDataSetted[1]);
                LDataSet11009.AcceptChanges();
                LSqlCommandBuilder11009.Dispose();
                LSqlDataAdapter11009New.Dispose();
                #endregion

                #region 插入密码变更记录
                LStrStep = "Step 05";
                
                DataSet LDataSet00002 = new DataSet();
                LStrDynamicSQL = "SELECT * FROM T_00_002 WHERE 1 = 2";
                SqlDataAdapter LSqlDataAdapter11 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                LSqlDataAdapter11.Fill(LDataSet00002, "T_00_002");
                LSqlDataAdapter11.Dispose();

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

                SqlDataAdapter LSqlDataAdapter00002New = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                SqlCommandBuilder LSqlCommandBuilder00002 = new SqlCommandBuilder();
                LSqlCommandBuilder00002.ConflictOption = ConflictOption.OverwriteChanges;
                LSqlCommandBuilder00002.SetAllValues = false;
                LSqlCommandBuilder00002.DataAdapter = LSqlDataAdapter00002New;
                LSqlDataAdapter00002New.Update(LDataSet00002, "T_00_002");
                LDataSet00002.AcceptChanges();
                LSqlCommandBuilder00002.Dispose();
                LSqlDataAdapter00002New.Dispose();
                #endregion

                #region 删除角色 1060000000000000002，1060000000000000003
                LStrDynamicSQL = "DELETE FROM T_11_004_" + AListStrRentDataSetted[1] + " WHERE C001 = 1060000000000000002 OR C001 = 1060000000000000003";
                LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                LSqlCommand.ExecuteNonQuery();
                LSqlCommand.Dispose();
                LSqlCommand = null;
                #endregion

                #region 删除角色包含的用户
                LStrDynamicSQL = "DELETE FROM T_11_201_" + AListStrRentDataSetted[1] + " WHERE C003 = 1060000000000000002 OR C003 = 1060000000000000003";
                LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                LSqlCommand.ExecuteNonQuery();
                LSqlCommand.Dispose();
                LSqlCommand = null;
                #endregion

                #region 删除角色拥有的功能操作权限
                LStrDynamicSQL = "DELETE FROM T_11_202_" + AListStrRentDataSetted[1] + " WHERE C001 = 1060000000000000002 OR C001 = 1060000000000000003";
                LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                LSqlCommand.ExecuteNonQuery();
                LSqlCommand.Dispose();
                LSqlCommand = null;
                #endregion
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "InitTableByRentInfo()\n" + LStrStep + "\n" + ex.ToString();
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
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
            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;

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

                LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDatabaseProfile[0], LListStrDatabaseProfile[1], LListStrDatabaseProfile[4], LListStrDatabaseProfile[2], LListStrDatabaseProfile[3]);
                LSqlConnection = new SqlConnection(LStrConnectParam);
                LSqlConnection.Open();
                #endregion

                #region 获取表各字段的基本信息
                DataSet LDataSetTable = new DataSet();
                LStrDynamicSQL = "SELECT A.NAME AS COLUMN_NAME, UPPER(B.NAME) AS DATA_TYPE, A.LENGTH AS DATA_LENGTH, A.PREC AS DATA_PRECISION, A.SCALE AS DATA_SCALE, A.ISNULLABLE AS NULLABLE, A.XTYPE ";
                LStrDynamicSQL += "FROM SYSCOLUMNS A, SYSTYPES B ";
                LStrDynamicSQL += "WHERE A.ID = OBJECT_ID('" + AStrTargetTable + "') AND A.XUSERTYPE = B.XUSERTYPE ";
                LStrDynamicSQL += "ORDER BY A.COLID ASC";
                SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                LSqlDataAdapter.Fill(LDataSetTable, AStrTargetTable);
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
                        LListStrArgs.Add("@Arg" + LStrColumnName);
                        LListStrValues.Add(LStrColumnValue);
                        LStrColumnType = LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[1].ToString();
                        LListStrDataType.Add(LStrColumnType);
                        LListIntDataLength.Add(int.Parse(LDataSetTable.Tables[0].Select("COLUMN_NAME = '" + LStrColumnName + "'").FirstOrDefault()[2].ToString()));
                        if (LStrColumnType == "NUMERIC")
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

                    SqlCommand LSqlCommandInsert = new SqlCommand(LStrSQLCommand, LSqlConnection);

                    LIntAllColumn = LListStrArgs.Count;
                    for (LIntLoopColumn = 0; LIntLoopColumn < LIntAllColumn; LIntLoopColumn++)
                    {
                        LStrColumnType = LListStrDataType[LIntLoopColumn];
                        SqlParameter LSqlParameter = new SqlParameter();
                        LSqlParameter.ParameterName = LListStrArgs[LIntLoopColumn].Substring(0);
                        switch (LStrColumnType)
                        {
                            case "NUMERIC":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = "-1"; }
                                if (LListIntScale[LIntLoopColumn] > 0)
                                {
                                    LSqlParameter.SqlDbType = SqlDbType.Decimal;
                                    LSqlParameter.Value = decimal.Parse(LListStrValues[LIntLoopColumn]);
                                }
                                else
                                {
                                    if (LListIntPrecision[LIntLoopColumn] > 10)
                                    {
                                        LSqlParameter.SqlDbType = SqlDbType.BigInt;
                                        LSqlParameter.Size = 8;
                                        LSqlParameter.Value = Int64.Parse(LListStrValues[LIntLoopColumn]);
                                    }
                                    else
                                    {
                                        LSqlParameter.SqlDbType = SqlDbType.Int;
                                        LSqlParameter.Size = 4;
                                        LSqlParameter.Value = Int32.Parse(LListStrValues[LIntLoopColumn]);
                                    }
                                }
                                break;
                            case "SMALLINT":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = "-1"; }
                                LSqlParameter.SqlDbType = SqlDbType.SmallInt;
                                LSqlParameter.Size = 2;
                                LSqlParameter.Value = Int16.Parse(LListStrValues[LIntLoopColumn]);
                                break;
                            case "BIGINT":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = "-1"; }
                                LSqlParameter.SqlDbType = SqlDbType.BigInt;
                                LSqlParameter.Size = 8;
                                LSqlParameter.Value = Int64.Parse(LListStrValues[LIntLoopColumn]);
                                break;
                            case "INT":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = "-1"; }
                                LSqlParameter.SqlDbType = SqlDbType.Int;
                                LSqlParameter.Size = 4;
                                 LSqlParameter.Value = Int32.Parse(LListStrValues[LIntLoopColumn]);
                                break;
                            case "NVARCHAR":
                                LSqlParameter.SqlDbType = SqlDbType.NVarChar;
                                LSqlParameter.Size = LListIntDataLength[LIntLoopColumn];
                                LSqlParameter.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "VARCHAR":
                                LSqlParameter.SqlDbType = SqlDbType.VarChar;
                                LSqlParameter.Size = LListIntDataLength[LIntLoopColumn];
                                LSqlParameter.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "CHAR":
                                LSqlParameter.SqlDbType = SqlDbType.Char;
                                LSqlParameter.Size = LListIntDataLength[LIntLoopColumn];
                                LSqlParameter.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "NCHAR":
                                LSqlParameter.SqlDbType = SqlDbType.NChar;
                                LSqlParameter.Size = LListIntDataLength[LIntLoopColumn];
                                LSqlParameter.Value = LListStrValues[LIntLoopColumn];
                                break;
                            case "DATETIME":
                                if (string.IsNullOrEmpty(LListStrValues[LIntLoopColumn])) { LListStrValues[LIntLoopColumn] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); }
                                LSqlParameter.SqlDbType = SqlDbType.DateTime;
                                LSqlParameter.Value = DateTime.Parse(LListStrValues[LIntLoopColumn]);
                                break;
                            default:
                                continue;
                        }
                        LSqlCommandInsert.Parameters.Add(LSqlParameter);
                    }
                    LIntEffectRows = LSqlCommandInsert.ExecuteNonQuery();
                    LSqlCommandInsert.Dispose(); LSqlCommandInsert = null;
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
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
            }

            return LOperationDataArgsReturn;
        }

        /// <summary>
        /// 删除数据库前线删除所有与该数据库的连接进程
        /// </summary>
        /// <param name="AServerClass"></param>
        /// <param name="AStrDBName">数据库名</param>
        /// <param name="AStrErrorReturn"></param>
        /// <returns></returns>
        private static bool KillMSSQLProcess(Server AServerClass, string AStrDBName, ref string AStrErrorReturn)
        {
            bool LBoolReturn = true;

            int IntPIDNum = -1, IntDBNameNum = -1, IntLoopNum, IntLoopRow;
            string LStrColumnName = string.Empty, LStrDBName = string.Empty;
            int LIntSQLSPid;
            string LStrSQLDBName;

            try
            {
                DataTable LDataTableProcessList = AServerClass.EnumProcesses();
                for (IntLoopNum = 0; IntLoopNum < LDataTableProcessList.Columns.Count; IntLoopNum++)
                {
                    LStrColumnName = LDataTableProcessList.Columns[IntLoopNum].ColumnName;
                    if (LStrColumnName.ToUpper().Trim() == "SPID") { IntPIDNum = IntLoopNum; }
                    if (LStrColumnName.ToUpper().Trim() == "DATABASE") { IntDBNameNum = IntLoopNum; }
                }
                if (IntPIDNum >= 0 && IntDBNameNum >= 0)
                {
                    for (IntLoopRow = 0; IntLoopRow < LDataTableProcessList.Rows.Count; IntLoopRow++)
                    {
                        LStrSQLDBName = LDataTableProcessList.Rows[IntLoopRow][IntDBNameNum].ToString();
                        if (LStrSQLDBName.ToUpper() == AStrDBName.ToUpper())
                        {
                            LIntSQLSPid = int.Parse(LDataTableProcessList.Rows[IntLoopRow][IntPIDNum].ToString());
                            AServerClass.KillProcess(LIntSQLSPid);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrErrorReturn = "WcfService02.KillMSSQLProcess\n" + ex.Message;
            }
            return LBoolReturn;
        }

        /// <summary>
        /// 将连接用户加入到数据库的 db_owner 角色中
        /// </summary>
        /// <param name="AListStrArguments"></param>
        private static void AddLoginIDIntoRoleDBOwner(List<string> AListStrArguments)
        {
            ServerConnection LSQLServerConnection = null;
            string LStrLoginID = string.Empty;
            bool LBoolLoginIDInDBOwner = false;

            try
            {
                LSQLServerConnection = new ServerConnection(AListStrArguments[0] + "," + AListStrArguments[1], AListStrArguments[5], AListStrArguments[6]);
                Server LServerClass = new Server(LSQLServerConnection);
                Database LDatabaseTarget = LServerClass.Databases[AListStrArguments[4]];
                DataTable LDataTableLoginMappings = LDatabaseTarget.EnumLoginMappings();

                foreach (DataRow LDataRowLogin in LDataTableLoginMappings.Rows)
                {
                    if (LDataRowLogin[1].ToString() == AListStrArguments[2])
                    {
                        LStrLoginID = LDataRowLogin[0].ToString(); break;
                    }
                }
                if (string.IsNullOrEmpty(LStrLoginID))
                {
                    User LUserNew = new User(LDatabaseTarget, AListStrArguments[2]);
                    LUserNew.Login = AListStrArguments[2];
                    LUserNew.Create();
                    LUserNew.AddToRole("db_owner");
                }
                else
                {
                    DatabaseRole LDatabaseRoleDBOwner = LDatabaseTarget.Roles["db_owner"];
                    StringCollection LStringCollectionRoleMember = LDatabaseRoleDBOwner.EnumMembers();
                    foreach (string LStrRoleMember in LStringCollectionRoleMember)
                    {
                        if (LStrRoleMember == LStrLoginID) { LBoolLoginIDInDBOwner = true; break; }
                    }
                    if (!LBoolLoginIDInDBOwner)
                    {
                        User LUserNew = new User(LDatabaseTarget, LStrLoginID);
                        LUserNew.Login = AListStrArguments[2];
                        LUserNew.AddToRole("db_owner");
                    }
                }
            }
            catch { }
            finally
            {
                if (LSQLServerConnection != null)
                {
                    if (LSQLServerConnection.IsOpen == true) { LSQLServerConnection.Disconnect(); }
                    LSQLServerConnection = null;
                }
            }
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