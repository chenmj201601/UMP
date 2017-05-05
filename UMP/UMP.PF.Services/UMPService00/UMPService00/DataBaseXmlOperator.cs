using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common;
using System.Xml;
using System.IO;
using System.Diagnostics;
using PFShareClassesS;
using VoiceCyber.Common;

namespace UMPService00
{
    public class DataBaseXmlOperator
    {
        /// <summary>
        /// 从配置文件中读取数据库信息
        /// </summary>
        /// <returns></returns>
        public static string GetDBInfo(string strFilePath)
        {
            try
            {
                DatabaseInfo dbInfo = new DatabaseInfo();
                string LStrXmlFileName = string.Empty;
                //LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(strFilePath, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    UMPService00.WriteLog(LogMode.Error,"GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    
                    return "Error01";
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                if (LXmlNodeListDatabase.Count <= 0)
                {
                    UMPService00.WriteLog(LogMode.Error, "LXmlNodeListDatabase.Count:  " + LXmlNodeListDatabase.Count);
                    UMPService00.WriteLog(LogMode.Error, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return "Error02";
                }

                string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrAttributesData = string.Empty;
                #region 数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {

                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    //UMPService00.IEventLog.WriteEntry("Database Enable: " + LStrAttributesData);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    dbInfo.TypeID = int.Parse(LStrAttributesData);


                    //数据库服务器名或IP地址
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P04"].Value;
                    dbInfo.Host = LStrAttributesData;

                    //数据库服务端口
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P05"].Value;
                    dbInfo.Port = int.Parse(EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104));

                    //数据库名或Service Name
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P06"].Value;
                    dbInfo.DBName = LStrAttributesData;

                    //登录用户
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P07"].Value;
                    dbInfo.LoginName = LStrAttributesData;

                    //登录密码
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P08"].Value;
                    dbInfo.Password = LStrAttributesData;
                    break;
                }
                #endregion
                switch (dbInfo.TypeID)
                {
                    case 2:
                        dbInfo.TypeName = "MSSQL";
                        break;
                    case 3:
                        dbInfo.TypeName = "ORCL";
                        break;
                }
                LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                string strResult = dbInfo.TypeID + Common.AscCodeToChr(27) + dbInfo.TypeName + Common.AscCodeToChr(27) + dbInfo.Host + Common.AscCodeToChr(27);
                strResult += EncryptionAndDecryption.EncryptDecryptString(dbInfo.Port.ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)
                    + Common.AscCodeToChr(27) + dbInfo.LoginName + Common.AscCodeToChr(27);
                strResult += dbInfo.Password + Common.AscCodeToChr(27);
                strResult += dbInfo.DBName;
                return strResult;
            }
            catch (Exception ex)
            {
                return "Error 002: " + ex.Message;
            }
        }

        /// <summary>
        /// 从配置文件中读取数据库信息
        /// </summary>
        /// <returns></returns>
        public static bool GetDBInfo(string strFilePath,ref DatabaseInfo dbInfo)
        {
            dbInfo = new DatabaseInfo();
            try
            {
                string LStrXmlFileName = string.Empty;
                //LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(strFilePath, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {

                    UMPService00.WriteLog(LogMode.Warn, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return false;
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                if (LXmlNodeListDatabase.Count <= 0)
                {
      
                    UMPService00.WriteLog(LogMode.Warn, "LXmlNodeListDatabase.Count:  " + LXmlNodeListDatabase.Count);                 
                    UMPService00.WriteLog(LogMode.Warn, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return false;
                }

                string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrAttributesData = string.Empty;
                #region 数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {

                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    //UMPService00.IEventLog.WriteEntry("Database Enable: " + LStrAttributesData);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    dbInfo.TypeID = int.Parse(LStrAttributesData);


                    //数据库服务器名或IP地址
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P04"].Value;
                    dbInfo.Host = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104) ;

                    //数据库服务端口
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P05"].Value;
                    dbInfo.Port = int.Parse(EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104));

                    //数据库名或Service Name
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P06"].Value;
                    dbInfo.DBName = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    //登录用户
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P07"].Value;
                    dbInfo.LoginName = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    //登录密码
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P08"].Value;
                    dbInfo.Password = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    break;
                }
                #endregion
                switch (dbInfo.TypeID)
                {
                    case 2:
                        dbInfo.TypeName = "MSSQL";
                        break;
                    case 3:
                        dbInfo.TypeName = "ORCL";
                        break;
                }
                dbInfo.RealPassword = dbInfo.Password;
              
                return true;
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error, "Error 002: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 在voice服务器上生成数据库连接的xml
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public static bool WriteDBInfoInVoiceServer(DatabaseInfo dbInfo,string strPath )
        {
            bool isSuccess= false;
            try
            {
                string strXmlFileDir = strPath + "\\UMP.Server";
                string strXmlFileName = "Args01.UMP.xml";
                XmlDocument xmlDoc = Common.CreateXmlDocumentIfNotExists(strXmlFileDir, strXmlFileName, "DatabaseParameters");
                XMLOperator xmlOperator = new XMLOperator(xmlDoc);
                XmlNode paramNode = xmlOperator.SelectNode("DatabaseParameters", "");
                bool bIsExistsDB = false;
                foreach (XmlNode node in paramNode)
                {
                    if (node.Name == "Database")
                    {
                        string strIsUsed = xmlOperator.SelectAttrib(node, "P03");
                        string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        strIsUsed = EncryptionAndDecryption.EncryptDecryptString(strIsUsed, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        if (strIsUsed == "1")
                        {
                            bIsExistsDB = true;
                            LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            xmlOperator.UpdateAttrib(node, "P02", dbInfo.TypeID.ToString());
                            xmlOperator.UpdateAttrib(node, "P03", EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node, "P04", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Host, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node, "P05", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Port.ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node, "P06", EncryptionAndDecryption.EncryptDecryptString(dbInfo.DBName, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node, "P07", EncryptionAndDecryption.EncryptDecryptString(dbInfo.LoginName, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node, "P08", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Password, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                            xmlOperator.UpdateAttrib(node,"P10", dbInfo.TypeName);
                            xmlOperator.Save(strXmlFileDir + "\\" + strXmlFileName);
                            break;
                        }
                    }
                }
                if (!bIsExistsDB)
                {
                    string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    XmlNode DatabaseNode = xmlOperator.InsertNode("Database", paramNode);
                    List<AttribEntry> lstAttribs = new List<AttribEntry>();
                    lstAttribs.Add(new AttribEntry("P01", "1"));
                    lstAttribs.Add(new AttribEntry("P02", dbInfo.TypeID.ToString()));
                    lstAttribs.Add(new AttribEntry("P03", EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P04", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Host, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P05", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Port.ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P06", EncryptionAndDecryption.EncryptDecryptString(dbInfo.DBName, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P07", EncryptionAndDecryption.EncryptDecryptString(dbInfo.LoginName, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P08", EncryptionAndDecryption.EncryptDecryptString(dbInfo.Password, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004)));
                    lstAttribs.Add(new AttribEntry("P09", ""));
                    lstAttribs.Add(new AttribEntry("P10", dbInfo.TypeName));
                    xmlOperator.InsertAttribs(DatabaseNode, lstAttribs);
                    xmlOperator.Save(strXmlFileDir + "\\" + strXmlFileName);
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        
    }
}
