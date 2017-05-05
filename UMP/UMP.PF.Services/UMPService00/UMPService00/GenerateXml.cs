using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.ResourceXmls;

namespace UMPService00
{
    public class GenerateXml : IEncryptable
    {
        DatabaseInfo dbInfo = null;
        SessionInfo Session = null;
        string strFilePath = string.Empty;
        ResourceXmlHelper generator = null;
        bool IsInitSuccess = false;
        string strAuthenServerHost = string.Empty;
        string strAuthenServerPort= string.Empty;
        string strReplaceModuleNumber = string.Empty;
        bool IsParamXmlExists = false;

        public GenerateXml(string strXmlPath, DatabaseInfo _dbInfo, AppServerInfo _serverInfo, string _AuthenServerHost, string _AuthenServerPort)
        {
            dbInfo = _dbInfo;
            strFilePath = strXmlPath;
            Session = CreateSessionInfo(dbInfo, _serverInfo);
            generator = new ResourceXmlHelper();
            generator.Session = Session;
            generator.EncryptObject = this;
            OperationReturn RReturn = generator.Init();
            if (!RReturn.Result)
            {
                UMPService00.WriteLog(LogMode.Error,"Init error info : " + RReturn.Message);
            }
            IsInitSuccess = RReturn.Result;
            strAuthenServerHost = _AuthenServerHost;
            strAuthenServerPort = _AuthenServerPort;
            //strReplaceModuleNumber = _strReplaceModuleNumber;
            //UMPService00.IEventLog.WriteEntry("init() strReplaceMdouleNumber = " + strReplaceModuleNumber, EventLogEntryType.Warning);
        }

        public int Generate(bool bisUMPServer,ref bool OBIsParamExists)
        {
            OBIsParamExists = CheckParamXmlExists();
            bool isSuccess = GenerateFullXml();
            if (!isSuccess)
            {
                return -1;
            }
            isSuccess = GenerateSimpXml();
            if (!isSuccess)
            {
                return -2;
            }
            isSuccess = GenerateVoiceFile();
            if (!isSuccess)
            {
                return -3;
            }
            isSuccess = GeneratePBXDeviceXml();
            if (!isSuccess)
            {
                return -4;
            }
            DirectoryInfo dir = new DirectoryInfo(strFilePath);
            if (dir.Exists)
            {
                dir.Delete();
            }
            if (bisUMPServer)
            {
                UpdateLicenseServer();
            }
            return 0;
        }

        public void ClenData()
        {
            if (generator != null)
            {
                generator.CleanData();
            }
        }

        /// <summary>
        /// 检查是否已经存在参数文件 
        /// </summary>
        /// <returns>true：存在 false：不存在</returns>
        private bool CheckParamXmlExists()
        {
            string strTargetDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config";
            List<string> lstFiles = Directory.GetFiles(strTargetDir).ToList();
            string strFileFullName = strTargetDir + "\\umpparam_full.xml";
            if (lstFiles.Contains(strFileFullName))
            {
                return true;
            }
            strFileFullName = strTargetDir + "\\umpparam_simp.xml";
            if (lstFiles.Contains(strFileFullName))
            {
                return true;
            }
            strFileFullName = strTargetDir + "\\umpparam_pbxdev.xml";
            if (lstFiles.Contains(strFileFullName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 生成xml框架 包括Configurations--LocalMachine、Sites--Resources--ParameterDatabases、
        /// Resources--AuthenticateServers 、Resources--PBXDevices
        /// </summary>
        private void GenerateFreamwork(XmlDocument xmlDoc, XMLOperator xmlOperator)
        {
            if (xmlOperator != null)
            {
                List<AttribEntry> lstAttribs = new List<AttribEntry>();
                List<string> lstChildNames = new List<string>();
                xmlOperator.InsertAttrib("Configurations", "Version", "2.0");
                XmlNode node = xmlOperator.SelectNode("Configurations", "");
                #region 添加Configurations下的子节点Configuration和Configuration下的子节点(一次添加两级)
                lstChildNames.Add("LocalMachine");
                lstChildNames.Add("Sites");
                xmlOperator.InsertNode("Configurations", "Configuration", lstChildNames, null);
                lstChildNames.Clear();
                #endregion

                #region LocalMachine的属性
                lstAttribs.Add(new AttribEntry("SiteID", "0"));
                lstAttribs.Add(new AttribEntry("LogPath", "d:\\log\\"));
                lstAttribs.Add(new AttribEntry("LicenseModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("DecServerModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("CTIHubServerModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("CTIDBBServerModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("VoiceModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("CMServerModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("AlarmMonitorNumber", "-1"));
                lstAttribs.Add(new AttribEntry("AlarmServerNumber", "-1"));
                lstAttribs.Add(new AttribEntry("FOModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("ScreenSvrNumber", "-1"));
                lstAttribs.Add(new AttribEntry("DBbridgeNumber", "-1"));
                lstAttribs.Add(new AttribEntry("SFTPNumber", "-1"));
                ////mark 增加2个参数 begin  20161024  create
                lstAttribs.Add(new AttribEntry("RecoverNumber", "-1"));
                lstAttribs.Add(new AttribEntry("CaptureNumber", "-1"));
                //end
                lstAttribs.Add(new AttribEntry("MachineModuleNumber", "-1"));
                lstAttribs.Add(new AttribEntry("KeyGenModuleNumber", "-1"));
                xmlOperator.InsertAttribs("Configurations/Configuration/LocalMachine", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Sites的子节点
                lstChildNames.Add("ParameterDatabases");
                lstChildNames.Add("Resources");
                xmlOperator.InsertNode("Configurations/Configuration/Sites", "Site", lstChildNames, null);
                lstChildNames.Clear();
                #endregion

                //添加site节点的属性
                xmlOperator.InsertAttrib("Configurations/Configuration/Sites/Site", "ID", "0");

                #region 添加ParameterDatabases的子节点
                lstChildNames.Add("Host");
                lstChildNames.Add("Port");
                lstChildNames.Add("Name");
                lstChildNames.Add("Authention");
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/ParameterDatabases", "Database", lstChildNames, null);
                lstChildNames.Clear();
                #endregion

                #region 添加Database节点的属性
                string strKey = dbInfo.TypeID + "-" + dbInfo.Host + ':' + dbInfo.Port + "-" + dbInfo.DBName;
                lstAttribs.Add(new AttribEntry("Key", EncryptionAndDecryption.EncryptStringSHA256(strKey)));
                lstAttribs.Add(new AttribEntry("Type", dbInfo.TypeID.ToString()));
                if (dbInfo.TypeID == 2)
                {
                    lstAttribs.Add(new AttribEntry("Provider", "SQLNCLI10"));
                }
                else if (dbInfo.TypeID == 3)
                {
                    lstAttribs.Add(new AttribEntry("Provider", "OraOLEDB.Oracle"));
                }
                else
                {
                    lstAttribs.Add(new AttribEntry("Provider", ""));
                }
                lstAttribs.Add(new AttribEntry("Options", ""));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Host节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(dbInfo.Host.ToString())));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Host", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Port节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(dbInfo.Port.ToString())));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Port", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Name节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(dbInfo.DBName)));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Name", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Authention的子节点
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Authention", "Username");
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Authention", "Password");
                #endregion

                #region 添加Username节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(dbInfo.LoginName)));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Authention/Username", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加Password节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(dbInfo.Password)));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/ParameterDatabases/Database/Authention/Password", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加AuthenticateServers节点及其子节点
                lstChildNames.Add("AuthenticateServer");
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/Resources", "AuthenticateServers", lstChildNames, null);
                lstChildNames.Clear();
                #endregion

                #region 添加AuthenticateServer节点的属性
                lstAttribs.Add(new AttribEntry("SSL", "1"));
                lstAttribs.Add(new AttribEntry("Key", "0"));
                lstAttribs.Add(new AttribEntry("Enable", "1"));
                lstAttribs.Add(new AttribEntry("ModuleNumber", "0"));
                lstAttribs.Add(new AttribEntry("Continent", "AS"));
                lstAttribs.Add(new AttribEntry("Country", "CHN"));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加AuthenticateServer下的子节点
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer", "HostAddress");
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer", "HostName");
                xmlOperator.InsertNode("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer", "HostPort");
                #endregion



                #region 添加HostAddress节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", EncryptString(strAuthenServerHost)));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer/HostAddress", lstAttribs);
                lstAttribs.Clear();
                #endregion


                #region 添加HostName节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                lstAttribs.Add(new AttribEntry("Value", "5DFE4FA874F738D9B5B7B56A49F433E28869BB3B3964E480E23339F0BFB3257CFACD8F4A9BF234FECA7C953CD7D2DF5A"));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer/HostName", lstAttribs);
                lstAttribs.Clear();
                #endregion

                #region 添加HostPort节点的属性
                lstAttribs.Add(new AttribEntry("Encryption", "2"));
                lstAttribs.Add(new AttribEntry("Encoding", "hex"));
                int iAuthenServerPort = 8081;
                int.TryParse(strAuthenServerPort, out iAuthenServerPort);
                iAuthenServerPort = iAuthenServerPort - 1;
                lstAttribs.Add(new AttribEntry("Value", EncryptString(iAuthenServerPort.ToString())));
                xmlOperator.InsertAttribs("Configurations/Configuration/Sites/Site/Resources/AuthenticateServers/AuthenticateServer/HostPort", lstAttribs);
                lstAttribs.Clear();
                #endregion

            }
        }

        /// <summary>
        /// 生成完整版的XML
        /// </summary>
        /// <returns></returns>
        private bool GenerateFullXml()
        {
            if (!IsInitSuccess)
            {
                return false;
            }
            string strXmlFileName = "umpparam_full.xml";
            XmlDocument xmlDoc = Common.CreateXmlDocumentIfNotExists(strFilePath, strXmlFileName, "Configurations");
            XMLOperator xmlOperator = new XMLOperator(xmlDoc);
            string strXmlFilePath = strFilePath + "\\" + strXmlFileName;
            //  UMPService00.IEventLog.WriteEntry("strXmlFilePath = " + strXmlFilePath, EventLogEntryType.Warning);
            bool bIsSuccess = false;
            GenerateFreamwork(xmlDoc, xmlOperator);
            bIsSuccess = GeneratDataWithoutVoiceAndPbx(xmlOperator);
            if (!bIsSuccess)
            {
                return false;
            }
            xmlDoc.Save(strXmlFilePath);

            XmlNode resourceNode = xmlOperator.SelectNode("Configurations/Configuration/Sites/Site/Resources", "");
            bIsSuccess = GeneratePBXDevice(resourceNode, xmlOperator);
            // UMPService00.IEventLog.WriteEntry("GeneratDataWithoutVoiceAndPbx()", EventLogEntryType.Warning);
            if (!bIsSuccess)
            {
                return false;
            }
            xmlDoc.Save(strXmlFilePath);
            bIsSuccess = GenerateVoice(xmlOperator);
            // UMPService00.IEventLog.WriteEntry("GenerateVoice()", EventLogEntryType.Warning);
            if (!bIsSuccess)
            {
                return false;
            }
            xmlDoc.Save(strXmlFilePath);
            UpdateLocalMachineNode(generator, xmlOperator);
            xmlOperator.Save(strXmlFilePath);
            ////将备机代替的主机从原来的xml中读出 写入新生成的xml 然后覆盖文件
            //if (!string.IsNullOrEmpty(strReplaceModuleNumber))
            //{
            //    UMPService00.IEventLog.WriteEntry("strReplaceModuleNumber is not null", EventLogEntryType.Warning);
            //    XmlNode backupVoiceNode = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "StandByRole", "3");
            //    if (backupVoiceNode != null)
            //    {
            //        UMPService00.IEventLog.WriteEntry("Write strReplaceMdouleNumber", EventLogEntryType.Warning);
            //        xmlOperator.InsertAttrib(backupVoiceNode, "ReplaceModuleNumber", strReplaceModuleNumber);
            //        xmlOperator.Save(strXmlFilePath);
            //    }
            //    else
            //    {
            //        UMPService00.IEventLog.WriteEntry("backupVoiceNode is null", EventLogEntryType.Warning);
            //    }
            //}

            string strTargetDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\"+strXmlFileName;
            bIsSuccess = Common.CopyFile(strXmlFilePath, strTargetDir);
            if (!bIsSuccess)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成简单版的XML 不包含PBX和voice的Channel
        /// </summary>
        /// <returns></returns>
        private bool GenerateSimpXml()
        {
            if (!IsInitSuccess)
            {
                return false;
            }
            string strXmlFileName = "umpparam_simp.xml";
            XmlDocument xmlDoc = Common.CreateXmlDocumentIfNotExists(strFilePath, strXmlFileName, "Configurations");
            XMLOperator xmlOperator = new XMLOperator(xmlDoc);
            string strXmlFilePath = strFilePath + "\\" + strXmlFileName;
            GenerateFreamwork(xmlDoc, xmlOperator);
            bool bIsSuccess = GeneratDataWithoutVoiceAndPbx(xmlOperator);
            if (!bIsSuccess)
            {
                return false;
            }
            bIsSuccess = GenerateVoiceWithoutChannel(xmlOperator);
            var voices = generator.ListResourceObjects.Where(p => p.ObjType == 221).ToList();

            UpdateLocalMachineNode(generator, xmlOperator);
            xmlOperator.Save(strXmlFilePath);
            ////将备机代替的主机从原来的xml中读出 写入新生成的xml 然后覆盖文件
            //if (!string.IsNullOrEmpty(strReplaceModuleNumber))
            //{
            //    XmlNode backupVoiceNode = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "StandByRole", "3");
            //    if (backupVoiceNode != null)
            //    {
            //        xmlOperator.InsertAttrib(backupVoiceNode, "ReplaceModuleNumber", strReplaceModuleNumber);
            //        xmlOperator.Save(strXmlFilePath);
            //    }
            //}

            string strTargetDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\" + strXmlFileName;
            bIsSuccess = Common.CopyFile(strXmlFilePath, strTargetDir);
            if (!bIsSuccess)
            {
                return false;
            }
            return true;
        }

        private bool GeneratePBXDeviceXml()
        {
            if (!IsInitSuccess)
            {
                return false;
            }
            string strXmlFileName= "umpparam_pbxdev.xml";
            XmlDocument xmlDoc = Common.CreateXmlDocumentIfNotExists(strFilePath, strXmlFileName, "Configurations");
            XMLOperator xmlOperator = new XMLOperator(xmlDoc);
            string strXmlFilePath = strFilePath + "//" + strXmlFileName;
            GenerateFreamwork(xmlDoc, xmlOperator);
            bool bIsSuccess = GeneratDataWithoutVoiceAndPbx(xmlOperator);
            if (!bIsSuccess)
            {
                return false;
            }
            XmlNode resourceNode = xmlOperator.SelectNode("Configurations/Configuration/Sites/Site/Resources", "");
            bIsSuccess = GeneratePBXDevice(resourceNode, xmlOperator);
            // UMPService00.IEventLog.WriteEntry("GeneratDataWithoutVoiceAndPbx()", EventLogEntryType.Warning);
            if (!bIsSuccess)
            {
                return false;
            }
            bIsSuccess = GenerateVoiceWithoutChannel(xmlOperator);
            var voices = generator.ListResourceObjects.Where(p => p.ObjType == 221).ToList();

            UpdateLocalMachineNode(generator, xmlOperator);
            xmlOperator.Save(strXmlFilePath);
            ////将备机代替的主机从原来的xml中读出 写入新生成的xml 然后覆盖文件
            //if (!string.IsNullOrEmpty(strReplaceModuleNumber))
            //{
            //    XmlNode backupVoiceNode = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "StandByRole", "3");
            //    if (backupVoiceNode != null)
            //    {
            //        xmlOperator.InsertAttrib(backupVoiceNode, "ReplaceModuleNumber", strReplaceModuleNumber);
            //        xmlOperator.Save(strXmlFilePath);
            //    }
            //}

            string strTargetDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\" + strXmlFileName;
            bIsSuccess = Common.CopyFile(strXmlFilePath, strTargetDir);
            if (!bIsSuccess)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 分开生成每个voice的xml
        /// </summary>
        /// <returns></returns>
        private bool GenerateVoiceFile()
        {
            if (!IsInitSuccess)
            {
                return false;
            }
            var voices = generator.ListResourceObjects.Where(p => p.ObjType == 221).ToList();
            if (voices.Count > 0)
            {
                int iVoiceKey = 0;
                string strVoiceXmlFileName = string.Empty;
                XmlDocument xmlDoc = null;
                XMLOperator xmlOperator = null;
                bool bIsSuccess = false;

                foreach (VoiceCyber.UMP.ResourceXmls.ResourceObject obj in voices)
                {
                    iVoiceKey = obj.Key;
                    strVoiceXmlFileName = string.Format("umpparam_voc{0:0000}.xml", iVoiceKey);
                    xmlDoc = Common.CreateXmlDocumentIfNotExists(strFilePath, strVoiceXmlFileName, "Configurations");
                    xmlOperator = new XMLOperator(xmlDoc);
                    string strXmlFilePath = strFilePath + "//" + strVoiceXmlFileName;
                    GenerateFreamwork(xmlDoc, xmlOperator);
                    bIsSuccess = GeneratDataWithoutVoiceAndPbx(xmlOperator);
                    if (!bIsSuccess)
                    {
                        return false;
                    }
                    bIsSuccess = GenerateSingleVoice(xmlOperator, obj.ObjID);
                    if (!bIsSuccess)
                    {
                        return false;
                    }
                    UpdateLocalMachineNode(generator, xmlOperator);
                    xmlOperator.Save(strXmlFilePath);

                    ////将备机代替的主机从原来的xml中读出 写入新生成的xml 然后覆盖文件
                    //if (!string.IsNullOrEmpty(strReplaceModuleNumber))
                    //{
                    //    XmlNode backupVoiceNode = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "StandByRole", "3");
                    //    if (backupVoiceNode != null)
                    //    {
                    //        xmlOperator.InsertAttrib(backupVoiceNode, "ReplaceModuleNumber", strReplaceModuleNumber);
                    //        xmlOperator.Save(strXmlFilePath);
                    //    }
                    //}

                    string strTargetDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\" + strVoiceXmlFileName;
                    bIsSuccess = Common.CopyFile(strXmlFilePath, strTargetDir);
                    if (!bIsSuccess)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 生成除了voice和PBX以外的节点
        /// </summary>
        /// <param name="AXmloperator"></param>
        /// <param name="ASXmlFilePath"></param>
        /// <returns></returns>
        private bool GeneratDataWithoutVoiceAndPbx(XMLOperator AXmloperator)
        {
            try
            {
                OperationReturn RReturn = null;
                //UpdateLocalMachineNode(generator, xmlOperator);
                XmlNode resourceNode = AXmloperator.SelectNode("Configurations/Configuration/Sites/Site/Resources", "");
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_MACHINE, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_LICENSESERVER, resourceNode, GenerateOption.Default);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_DATATRANSFERSERVER, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CTIHUBSERVER, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CTIDBBRIDGE, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ARCHIVESTRATEGY, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                //2016.10.11  增加回删的参数 和  RESOURCE_ARCHIVESTRATEGY 一样的
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_BACKUPSTRATEGY, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }


                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_STORAGEDEVICE, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ALARMMONITOR, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ALARMSERVER, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                bool isSuccess = GenerateDBBridges(resourceNode, AXmloperator);
                if (!isSuccess)
                {
                    return false;
                }
                UMPService00.WriteLog("SFTP START");
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_SFTP, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                UMPService00.WriteLog("SFTP END,result =" + RReturn.Result + ",message = " + RReturn.Message);
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_KEYGENERATOR, resourceNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                XmlNode siteNode = AXmloperator.SelectNode("Configurations/Configuration/Sites/Site", "");
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CTIPOLICY, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_FILEOPERATOR, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ALARMMONITORPARAM, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                //创建AlarmServerParam节点
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ALARMSERVERPARAM, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ALARMLOGGINGPARAM, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CMSERVER, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_ISASERVER, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_SCREENSERVER, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_SPEECHANALYSISPARAM, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }

                // mark 20160905 增加两个结点 恢复录音服务器集和抓包服务器集
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_RECOVERSERVER, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }
                RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CAPTURESERVER, siteNode);
                if (!RReturn.Result)
                {
                    UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error, "GeneratDataWithoutVoiceAndPbx() Exception,Error : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 生成所有voice服务器信息 不包含通道
        /// </summary>
        /// <param name="AXmloperator"></param>
        /// <param name="ASXmlFilePath"></param>
        /// <returns></returns>
        private bool GenerateVoiceWithoutChannel(XMLOperator AXmloperator)
        {
            if (!IsInitSuccess)
            {
                return false;
            }
            XmlNode siteNode = AXmloperator.SelectNode("Configurations/Configuration/Sites/Site", "");
            OperationReturn RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_VOICESERVER, siteNode, 
                GenerateOption.Default| GenerateOption.IgnoreChannel);
            if (!RReturn.Result)
            {
                UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                return false;
            }
            //XmlNode voiceNode = AXmloperator.SelectNode("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "");
            //if (voiceNode == null)
            //{
            //    return true;
            //}
            //RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_NETWORKCARD, voiceNode);
            //if (!RReturn.Result)
            //{
            //    return false;
            //}
            //RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_VOIPPROTOCAL, voiceNode);
            //if (!RReturn.Result)
            //{
            //    return false;
            //}
            //RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_CONCURRENT, voiceNode);
            //if (!RReturn.Result)
            //{
            //    return false;
            //}
            //RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_NTIDRVPATH, voiceNode);
            //if (!RReturn.Result)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 生成所有的voice服务器信息 包含通道
        /// </summary>
        /// <param name="AXmloperator"></param>
        /// <param name="ASXmlFilePath"></param>
        /// <returns></returns>
        private bool GenerateVoice(XMLOperator AXmloperator)
        {
            XmlNode siteNode = AXmloperator.SelectNode("Configurations/Configuration/Sites/Site", "");
            OperationReturn RReturn = null;
            RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_VOICESERVER, siteNode);
            if (!RReturn.Result)
            {
                UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
            }
            return RReturn.Result;
        }

        /// <summary>
        /// 生成单个voice资源
        /// </summary>
        /// <param name="AXmloperator"></param>
        /// <param name="ASXmlFilePath"></param>
        /// <param name="ALObjID"></param>
        /// <returns></returns>
        private bool GenerateSingleVoice(XMLOperator AXmloperator, long ALObjID)
        {
            XmlNode voiceNode = AXmloperator.InsertNode("Configurations/Configuration/Sites/Site", "VoiceServers");
            OperationReturn RReturn = generator.GenerateResourceXmlNode(ALObjID, voiceNode);
            if (!RReturn.Result)
            {
                UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
            }
            return RReturn.Result;
        }

        /// <summary>
        /// 生成DBBridage节点 临时函数
        /// </summary>
        /// <param name="siteNode"></param>
        /// <param name="xmlOperator"></param>
        /// <returns></returns>
        private bool GenerateDBBridges(XmlNode siteNode, XMLOperator xmlOperator)
        {
            var lstDBBridges = generator.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_DBBRIDGE).ToList();
            if (lstDBBridges.Count > 0)
            {
                List<long> lstObjIDs = new List<long>();
                for (int i = 0; i < lstDBBridges.Count; i++)
                {
                    if (!lstObjIDs.Contains(lstDBBridges[i].ObjID))
                    {
                        lstObjIDs.Add(lstDBBridges[i].ObjID);
                    }
                }
                if (lstObjIDs.Count > 0)
                {
                    XmlNode DBBridgesNode = xmlOperator.InsertNode("DBbridges", siteNode);
                    OperationReturn RReturn = null;
                    for (int i = 0; i < lstObjIDs.Count; i++)
                    {
                        RReturn = generator.GenerateResourceXmlNode(lstObjIDs[i], DBBridgesNode, GenerateOption.Property);
                        if (!RReturn.Result)
                        {
                            UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                            return false;
                        }
                        GenerateDBBridageName(DBBridgesNode.ChildNodes[i], xmlOperator);
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// 生成DBBridageName节点 临时函数
        /// </summary>
        /// <param name="DBBrigeNode"></param>
        /// <param name="xmlOperator"></param>
        /// <returns></returns>
        private bool GenerateDBBridageName(XmlNode DBBrigeNode, XMLOperator xmlOperator)
        {
            XmlNode DBBridgeNameNode = xmlOperator.InsertNode("DBbridgeName", DBBrigeNode);
            xmlOperator.InsertAttrib(DBBridgeNameNode, "id", "0");
            XmlNode DatabaseNode = xmlOperator.SelectNode("Configurations//Configuration//Sites//Site//ParameterDatabases//Database", "");
            string strDBKey = xmlOperator.SelectAttrib(DatabaseNode, "Key");
            xmlOperator.InsertAttrib(DBBridgeNameNode, "index", strDBKey);
            return true;
        }

        /// <summary>
        /// 生成PBXDevice节点
        /// </summary>
        /// <returns></returns>
        private bool GeneratePBXDevice(XmlNode resourceNode, XMLOperator xmlOperator)
        {
            OperationReturn RReturn = generator.GenerateTypeXmlNode(S1110Consts.RESOURCE_PBXDEVICE, resourceNode);
            if (!RReturn.Result)
            {
                UMPService00.WriteLog("Generator", string.Format("Fail.\t{0}\t{1}", RReturn.Code, RReturn.Message));
                return false;
            }
            return true;
        }

        /// <summary> 
        /// 更新LocalMachine节点
        /// </summary>
        private void UpdateLocalMachineNode(ResourceXmlHelper helper, XMLOperator xmlOperator)
        {
            try
            {
                //获得本机的所有IP
                List<IPAddress> lstAddress = Common.GetLocalMachineIP();
                if (lstAddress.Count > 0)
                {
                    foreach (IPAddress address in lstAddress)
                    {
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            continue;
                        }
                        var LicenseSvrs = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_LICENSESERVER && p.HostAddress == address.ToString()).ToList();
                        if (LicenseSvrs.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "LicenseModuleNumber", LicenseSvrs[0].ModuleNumber.ToString());
                        }

                        var DecSvr = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_DATATRANSFERSERVER && p.HostAddress == address.ToString()).ToList();
                        if (DecSvr.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "DecServerModuleNumber", DecSvr[0].ModuleNumber.ToString());
                        }

                        //mark 增加2个参数 begin  20160906
                        var recoverNumber = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_RECOVERSERVER && p.HostAddress == address.ToString()).ToList();
                        if (recoverNumber.Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "RecoverNumber", recoverNumber[0].ModuleNumber.ToString());
                        }

                        var captureNumber = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_CAPTURESERVER && p.HostAddress == address.ToString()).ToList();
                        if (captureNumber.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "CaptureNumber", captureNumber[0].ModuleNumber.ToString());
                        }
                        //end
 
                        var CTIHubSvr = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_CTIHUBSERVER && p.HostAddress == address.ToString()).ToList();
                        if (CTIHubSvr.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "CTIHubServerModuleNumber", CTIHubSvr[0].ModuleNumber.ToString());
                        }

                        var CTIDBbridge = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_CTIDBBRIDGE && p.HostAddress == address.ToString()).ToList();
                        if (CTIDBbridge.Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "CTIDBBServerModuleNumber", CTIDBbridge[0].ModuleNumber.ToString());
                        }

                        var VoiceSvr = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_VOICESERVER && p.HostAddress == address.ToString()).ToList();
                        //UMPService00.IEventLog.WriteEntry("ASXmlFilePath = " + ASXmlFilePath + "  ; address = " + address.ToString() + " ; VoiceSvr count =" + VoiceSvr.Count, EventLogEntryType.Warning);
                        if (VoiceSvr.ToList().Count > 0)
                        {
                            //UMPService00.IEventLog.WriteEntry("VoiceSvr[0].ModuleNumber = " + VoiceSvr[0].ModuleNumber + "VoiceSvr[0].HostAddress = " + VoiceSvr[0].HostAddress, EventLogEntryType.Warning);
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "VoiceModuleNumber", VoiceSvr[0].ModuleNumber.ToString());
                        }

                        var CMServer = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_CMSERVER && p.HostAddress == address.ToString()).ToList();
                        if (CMServer.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "CMServerModuleNumber", CMServer[0].ModuleNumber.ToString());
                        }

                        var AlarmMonitor = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_ALARMMONITOR && p.HostAddress == address.ToString()).ToList();
                        if (AlarmMonitor.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "AlarmMonitorNumber", AlarmMonitor[0].ModuleNumber.ToString());
                        }

                        var AlarmServer = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_ALARMSERVER && p.HostAddress == address.ToString()).ToList();
                        if (AlarmServer.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "AlarmServerNumber", AlarmServer[0].ModuleNumber.ToString());
                        }


                        var FOServer = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_FILEOPERATOR && p.HostAddress == address.ToString()).ToList();
                        if (FOServer.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "FOModuleNumber", FOServer[0].ModuleNumber.ToString());
                        }

                        var ScreenSvr = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_SCREENSERVER && p.HostAddress == address.ToString()).ToList();
                        if (ScreenSvr.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "ScreenSvrNumber", ScreenSvr[0].ModuleNumber.ToString());
                        }

                        var DBbridge = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_DBBRIDGE && p.HostAddress == address.ToString()).ToList();
                        if (DBbridge.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "DBbridgeNumber", DBbridge[0].ModuleNumber.ToString());
                        }

                        var SFTP = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_SFTP && p.HostAddress == address.ToString()).ToList();
                        if (SFTP.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "SFTPNumber", SFTP[0].ModuleNumber.ToString());
                        }

                        var localMachine = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_MACHINE && p.HostAddress == address.ToString()).ToList();
                        if (localMachine.ToList().Count > 0)
                        {
                            var Pros = localMachine[0].ListPropertyValues.Where(p => p.ObjType == S1110Consts.RESOURCE_MACHINE && p.PropertyID == 11).ToList();
                            if (Pros.Count > 0)
                            {
                                string strLogPath = Pros[0].Value.ToString();
                                xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "LogPath", strLogPath);
                            }
                        }

                        var machines = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_MACHINE && p.HostAddress == address.ToString()).ToList();
                        if (machines.Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "MachineModuleNumber", machines[0].ModuleNumber.ToString());
                        }

                        var keyGenServers = helper.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_KEYGENERATOR && p.HostAddress == address.ToString()).ToList();
                        if (keyGenServers.ToList().Count > 0)
                        {
                            xmlOperator.UpdateAttrib("Configurations/Configuration/LocalMachine", "KeyGenModuleNumber", keyGenServers[0].ModuleNumber.ToString());
                        }


                    }
                }
            }
            catch (Exception ex)
            {
       
                UMPService00.WriteLog(LogMode.Error, "UpdateLocalMachineNode Error: " + ex.Message);

            }
        }

        private void UpdateLicenseServer()
        {
            var LicenseServers = generator.ListResourceObjects.Where(p => p.ObjType == S1110Consts.RESOURCE_LICENSESERVER).ToList();
            if (LicenseServers.Count > 0)
            {
                List<LicenseServer> lstServers = new List<LicenseServer>();
                LicenseServer server = null;
                string strIsEnable = string.Empty;
                foreach (VoiceCyber.UMP.ResourceXmls.ResourceObject obj in LicenseServers)
                {
                    var pros = obj.ListPropertyValues.Where(p => p.PropertyID == 6).ToList();
                    if (pros.Count > 0)
                    {
                        strIsEnable = pros[0].Value;
                    }
                    if (strIsEnable.Equals("1"))
                    {
                        server = new LicenseServer();
                        pros = obj.ListPropertyValues.Where(p => p.PropertyID == 5).ToList();
                        if (pros.Count > 0)
                        {
                            server.IsMain = int.Parse(pros[0].Value);
                        }
                        pros = obj.ListPropertyValues.Where(p => p.PropertyID == 7).ToList();
                        if (pros.Count > 0)
                        {
                            server.Host = pros[0].Value;
                        }
                        pros = obj.ListPropertyValues.Where(p => p.PropertyID == 9).ToList();
                        if (pros.Count > 0)
                        {
                            server.Port = int.Parse(pros[0].Value);
                        }
                        lstServers.Add(server);
                    }
                }
                if (lstServers.Count > 0)
                {
                    string strFileName = Common.GetCurrentBaseDirectory().Trim('\\') + "\\GlobalSettings";
                    XmlDocument doc =Common.CreateXmlDocumentIfNotExists(strFileName, "Args02.UMP.xml","Parameters02");
                    XMLOperator xmloperator = new XMLOperator(doc);
                    XmlNode ServerNode = xmloperator.SelectNode("Parameters02/LicenseServer", "");
                    if (ServerNode == null)
                    {
                        ServerNode = xmloperator.InsertNode("Parameters02", "LicenseServer");
                    }
                    XmlNodeList childs = ServerNode.ChildNodes;
                    foreach (XmlNode node in childs)
                    {
                        if (node.Name.Equals("LicServer"))
                        {
                            ServerNode.RemoveChild(node);
                        }
                    }

                    List<AttribEntry> lstAttribs = null;
                    foreach (LicenseServer ser in lstServers)
                    {
                        XmlNode LicNode = xmloperator.InsertNode("LicServer", ServerNode);
                        lstAttribs = new List<AttribEntry>();
                        lstAttribs.Add(new AttribEntry("P01", ser.IsMain.ToString()));
                        lstAttribs.Add(new AttribEntry("P02", "1"));
                        lstAttribs.Add(new AttribEntry("P03", ser.Host));
                        lstAttribs.Add(new AttribEntry("P04", (ser.Port).ToString()));
                        lstAttribs.Add(new AttribEntry("P05", ""));
                        xmloperator.InsertAttribs(LicNode, lstAttribs);
                    }
                    xmloperator.Save(strFileName+"\\Args02.UMP.xml");
                }
            }
        }

        /// <summary>
        /// 创建SessionInfo
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        private SessionInfo CreateSessionInfo(DatabaseInfo dbInfo, AppServerInfo serverInfo)
        {
            #region 创建SessionInfo
            SessionInfo Session = new SessionInfo();
            Session.SessionID = Guid.NewGuid().ToString();
            Session.AppName = "UMP";
            Session.LastActiveTime = DateTime.Now;

            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            Session.RentInfo = rentInfo;
            Session.RentID = ConstValue.RENT_DEFAULT;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;


            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;
            Session.RoleID = ConstValue.ROLE_SYSTEMADMIN;

            Session.AppServerInfo = serverInfo;

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            Session.ThemeInfo = themeInfo;
            Session.ThemeName = "Default";

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            Session.SupportThemes.Add(themeInfo);

            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            Session.SupportLangTypes.Add(langType);
            Session.LangTypeInfo = langType;
            Session.LangTypeID = langType.LangID;

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "zh-cn";
            langType.Display = "繁体中文";
            Session.SupportLangTypes.Add(langType);

            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            Session.InstallPath = @"C:\UMPRelease";

            Session.ListPartitionTables.Clear();
            PartitionTableInfo partInfo = new PartitionTableInfo();
            partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            partInfo.PartType = TablePartType.DatetimeRange;
            partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            Session.ListPartitionTables.Add(partInfo);
            #endregion

            return Session;
        }

        public string DecryptString(string source, int mode)
        {
            throw new NotImplementedException();
        }

        public string DecryptString(string source)
        {
            throw new NotImplementedException();
        }

        public string EncryptString(string source, int mode)
        {
            string strEncryString = string.Empty;
            switch (mode)
            {
                case (int)EncryptionMode.AES256V01Hex:
                    string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    strEncryString = EncryptionAndDecryption.EncryptDecryptString(source, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    break;
                case (int)EncryptionMode.SHA256V00Hex:
                    strEncryString = EncryptionAndDecryption.EncryptStringSHA256(source);
                    break;
            }
            return strEncryString;
        }

        public string EncryptString(string source)
        {
            string strEncryString = string.Empty;
            string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
            strEncryString = EncryptionAndDecryption.EncryptDecryptString(source, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M001);
            return strEncryString;
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return source;
        }
    }
}
