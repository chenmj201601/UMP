using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace UMP.Tools.PublicClasses
{
    public static class ConnectedServerXmlOperations
    {
        public static ConnectedServerInformation ReadConnectedServerList(string AStrProgramDataDirectory, bool ABoolCreateFile)
        {
            ConnectedServerInformation LConnectedServerInformation = new ConnectedServerInformation();
            string LStrConnectedServerXMLFile = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                LStrConnectedServerXMLFile = Path.Combine(AStrProgramDataDirectory, @"UMP.Client\iTools\ConnectedServer.xml");

                #region 文件不存在，且传入的参数 ABoolCreateFile = true 说明要创建该文件
                if (!File.Exists(LStrConnectedServerXMLFile) && ABoolCreateFile)
                {
                    if (!CreateConnectedServerXmlFile(AStrProgramDataDirectory, "127.0.0.1", "administrator", "", "0", ref LStrCallReturn))
                    {
                        LConnectedServerInformation.BoolReturn = false;
                        LConnectedServerInformation.StrReturn = LStrCallReturn;
                        return LConnectedServerInformation;
                    }
                }
                #endregion

                #region 读取已经连接的应用服务器
                XmlDocument LXmlDocConnectedServerLoaded = new XmlDocument();
                LXmlDocConnectedServerLoaded.Load(LStrConnectedServerXMLFile);
                XmlNode LXMLNodeConnectServer = LXmlDocConnectedServerLoaded.SelectSingleNode("ConnectedServer");
                XmlNodeList LXmlNodeListServer = LXMLNodeConnectServer.ChildNodes;
                foreach (XmlNode LXmlNodeSingleServer in LXmlNodeListServer)
                {
                    List<string> LListSterConnectedServer = new List<string>();
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["ServerName"].Value);
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["LastConnect"].Value);
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["NetworkProtocol"].Value);
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["ConnectionTimeOut"].Value);
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["ExcutionTimeOut"].Value);
                    LListSterConnectedServer.Add(LXmlNodeSingleServer.Attributes["CertificateHashString"].Value);
                    LConnectedServerInformation.ListStrConnectedServer.Add(LListSterConnectedServer);

                    XmlNodeList LXmlNodeListConnectArgs = LXmlNodeSingleServer.ChildNodes;
                    List<List<string>> LListStrConnectArgsList = new List<List<string>>();
                    foreach (XmlNode LXmlNodeSingleArgs in LXmlNodeListConnectArgs)
                    {
                        List<string> LListStrSingleConnectArgs = new List<string>();
                        LListStrSingleConnectArgs.Add(LXmlNodeSingleArgs.Attributes["Attribute1"].Value);       //最后连接的服务器
                        LListStrSingleConnectArgs.Add(LXmlNodeSingleArgs.Attributes["Attribute2"].Value);       //记住密码
                        LListStrSingleConnectArgs.Add(LXmlNodeSingleArgs.Attributes["Attribute3"].Value);       //登录用户
                        LListStrSingleConnectArgs.Add(LXmlNodeSingleArgs.Attributes["Attribute4"].Value);       //登录密码
                        LListStrConnectArgsList.Add(LListStrSingleConnectArgs);
                    }
                    LConnectedServerInformation.ListListStrConnectedArgs.Add(LListStrConnectArgsList);
                }
                #endregion
            }
            catch (Exception ex)
            {
                LConnectedServerInformation.BoolReturn = false;
                LConnectedServerInformation.StrReturn = "ECSXO001" + App.GStrSpliterChar + ex.Message;
            }

            return LConnectedServerInformation;
        }

        /// <summary>
        /// 将连接成功的信息添加到  ConnectedServer.xml 中
        /// </summary>
        /// <param name="AStrProgramDataDirectory">ProgramData 的路径</param>
        /// <param name="AListStrConnectInfo">
        /// 0：应用服务器名或IP地址
        /// 1：端口
        /// 2：用户名（账号）
        /// 3：密码
        /// 4：是否记住登录密码
        /// 5：连接协议 的 index
        /// 6：连接协议 http 或 https
        /// 7：连接超时（秒）
        /// 8：执行超时（秒）
        /// 9：应用服务器使用的证书HashString
        /// </param>
        /// <param name="ABoolCreateFile"></param>
        /// <returns></returns>
        public static ConnectedServerInformation AddConnectedServerInformation(string AStrProgramDataDirectory, List<string> AListStrConnectInfo, bool ABoolCreateFile)
        {
            ConnectedServerInformation LConnectedServerInformation = new ConnectedServerInformation();
            string LStrConnectedServerXMLFile = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrCallReturn = string.Empty;

            string LStrReadServerName = string.Empty;
            string LStrReadLoginName = string.Empty;
            bool LBoolFindServerName = false;
            bool LBoolFindLoginName = false;

            try
            {
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrConnectedServerXMLFile = Path.Combine(AStrProgramDataDirectory, @"UMP.Client\iTools\ConnectedServer.xml");

                #region 文件不存在，且传入的参数 ABoolCreateFile = true 说明要创建该文件
                if (!File.Exists(LStrConnectedServerXMLFile) && ABoolCreateFile)
                {
                    if (!CreateConnectedServerXmlFile(AStrProgramDataDirectory, "127.0.0.1", "administrator", "", "0", ref LStrCallReturn))
                    {
                        LConnectedServerInformation.BoolReturn = false;
                        LConnectedServerInformation.StrReturn = LStrCallReturn;
                        return LConnectedServerInformation;
                    }
                }
                #endregion

                XmlDocument LXmlDocConnectedServerLoaded = new XmlDocument();
                LXmlDocConnectedServerLoaded.Load(LStrConnectedServerXMLFile);
                XmlNode LXMLNodeConnectServer = LXmlDocConnectedServerLoaded.SelectSingleNode("ConnectedServer");
                XmlNodeList LXmlNodeListServer = LXMLNodeConnectServer.ChildNodes;
                foreach (XmlNode LXmlNodeSingleServer in LXmlNodeListServer)
                {
                    LStrReadServerName = LXmlNodeSingleServer.Attributes["ServerName"].Value;
                    LXmlNodeSingleServer.Attributes["LastConnect"].Value = "0";
                    if (LStrReadServerName == AListStrConnectInfo[0])
                    {
                        LBoolFindServerName = true;
                        LXmlNodeSingleServer.Attributes["LastConnect"].Value = "1";
                        LXmlNodeSingleServer.Attributes["NetworkProtocol"].Value = AListStrConnectInfo[5];
                        LXmlNodeSingleServer.Attributes["ConnectionTimeOut"].Value = AListStrConnectInfo[7];
                        LXmlNodeSingleServer.Attributes["ExcutionTimeOut"].Value = AListStrConnectInfo[8];
                        LXmlNodeSingleServer.Attributes["CertificateHashString"].Value = AListStrConnectInfo[9];
                        XmlNodeList LXmlNodeListConnectArgs = LXmlNodeSingleServer.ChildNodes;
                        foreach (XmlNode LXmlNodeSingleArgs in LXmlNodeListConnectArgs)
                        {
                            LStrReadLoginName = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleArgs.Attributes["Attribute3"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                            LXmlNodeSingleArgs.Attributes["Attribute1"].Value = "0";
                            if (LStrReadLoginName == AListStrConnectInfo[2])
                            {
                                LBoolFindLoginName = true;
                                LXmlNodeSingleArgs.Attributes["Attribute1"].Value = "1";
                                LXmlNodeSingleArgs.Attributes["Attribute2"].Value = AListStrConnectInfo[4];
                                LXmlNodeSingleArgs.Attributes["Attribute4"].Value = EncryptionAndDecryption.EncryptDecryptString(AListStrConnectInfo[3],LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                            }
                        }
                        if (!LBoolFindLoginName)
                        {
                            XmlElement LXmlElementLoginArgs = LXmlDocConnectedServerLoaded.CreateElement("LoginArgs");
                            LXmlElementLoginArgs.SetAttribute("Attribute1", "1");                                                       //最后连接的服务器
                            LXmlElementLoginArgs.SetAttribute("Attribute2", AListStrConnectInfo[4]);                                      //记住密码
                            LXmlElementLoginArgs.SetAttribute("Attribute3", EncryptionAndDecryption.EncryptDecryptString(AListStrConnectInfo[2],LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));   //登录用户
                            LXmlElementLoginArgs.SetAttribute("Attribute4", EncryptionAndDecryption.EncryptDecryptString(AListStrConnectInfo[3],LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));   //登录密码
                            LXmlNodeSingleServer.AppendChild(LXmlElementLoginArgs);
                        }
                    }
                }
                if (!LBoolFindServerName)
                {
                    XmlElement LXmlElementSingleServer = LXmlDocConnectedServerLoaded.CreateElement("ServerObject");
                    LXmlElementSingleServer.SetAttribute("ServerName", AListStrConnectInfo[0]);
                    LXmlElementSingleServer.SetAttribute("LastConnect", "1");
                    LXmlElementSingleServer.SetAttribute("NetworkProtocol", AListStrConnectInfo[5]);
                    LXmlElementSingleServer.SetAttribute("ConnectionTimeOut", AListStrConnectInfo[7]);
                    LXmlElementSingleServer.SetAttribute("ExcutionTimeOut", AListStrConnectInfo[8]);
                    LXmlElementSingleServer.SetAttribute("CertificateHashString", AListStrConnectInfo[9]);
                    LXMLNodeConnectServer.AppendChild(LXmlElementSingleServer);

                    XmlElement LXmlElementLoginArgs = LXmlDocConnectedServerLoaded.CreateElement("LoginArgs");
                    LXmlElementLoginArgs.SetAttribute("Attribute1", "1");                                                       //最后连接的服务器
                    LXmlElementLoginArgs.SetAttribute("Attribute2", AListStrConnectInfo[4]);                                      //记住密码
                    LXmlElementLoginArgs.SetAttribute("Attribute3", EncryptionAndDecryption.EncryptDecryptString(AListStrConnectInfo[2], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));   //登录用户
                    LXmlElementLoginArgs.SetAttribute("Attribute4", EncryptionAndDecryption.EncryptDecryptString(AListStrConnectInfo[3], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));   //登录密码
                    LXmlElementSingleServer.AppendChild(LXmlElementLoginArgs);
                }

                LXmlDocConnectedServerLoaded.Save(LStrConnectedServerXMLFile);
            }
            catch (Exception ex)
            {
                LConnectedServerInformation.BoolReturn = false;
                LConnectedServerInformation.StrReturn = "ECSXO002" + App.GStrSpliterChar + ex.Message;
            }

            return LConnectedServerInformation;
        }

        private static bool CreateConnectedServerXmlFile(string AStrProgramDataDirectory, string AStrServerName, string AStrLoginName, string AStrLoginPassword, string AStrRememberPassword, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrConnectedServerXMLFile = string.Empty;
            string LStrVerificationCode001 = string.Empty;

            try
            {
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);

                LStrConnectedServerXMLFile = Path.Combine(AStrProgramDataDirectory, @"UMP.Client\iTools\ConnectedServer.xml");

                XmlDocument LXmlDocConnectedServer = new XmlDocument();
                LXmlDocConnectedServer.AppendChild(LXmlDocConnectedServer.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlElement LXmlElementLoacal = LXmlDocConnectedServer.CreateElement("ConnectedServer");
                LXmlDocConnectedServer.AppendChild(LXmlElementLoacal);

                XmlElement LXmlElementSingleServer = LXmlDocConnectedServer.CreateElement("ServerObject");
                LXmlElementSingleServer.SetAttribute("ServerName", AStrServerName);
                LXmlElementSingleServer.SetAttribute("LastConnect", "1");
                LXmlElementSingleServer.SetAttribute("NetworkProtocol", "0");
                LXmlElementSingleServer.SetAttribute("ConnectionTimeOut", "60");
                LXmlElementSingleServer.SetAttribute("ExcutionTimeOut", "0");
                LXmlElementSingleServer.SetAttribute("CertificateHashString", "");
                LXmlElementLoacal.AppendChild(LXmlElementSingleServer);

                XmlElement LXmlElementLoginArgs = LXmlDocConnectedServer.CreateElement("LoginArgs");

                //最后连接的服务器
                LXmlElementLoginArgs.SetAttribute("Attribute1", "1");
                //记住密码
                LXmlElementLoginArgs.SetAttribute("Attribute2", AStrRememberPassword);
                //登录用户
                LXmlElementLoginArgs.SetAttribute("Attribute3", EncryptionAndDecryption.EncryptDecryptString(AStrLoginName, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));
                //登录密码
                LXmlElementLoginArgs.SetAttribute("Attribute4", EncryptionAndDecryption.EncryptDecryptString(AStrLoginPassword, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));         
                
                LXmlElementSingleServer.AppendChild(LXmlElementLoginArgs);

                LXmlDocConnectedServer.Save(LStrConnectedServerXMLFile);
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "ECSXO101" + App.GStrSpliterChar + ex.Message;
            }

            return LBoolReturn;
        }

        #region 创建加密解密验证字符串
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
        #endregion
    }

    public class ConnectedServerInformation
    {
        public bool BoolReturn = true;
        public string StrReturn = string.Empty;

        public List<List<string>> ListStrConnectedServer = new List<List<string>>();
        public List<List<List<string>>> ListListStrConnectedArgs = new List<List<List<string>>>();
    }
}
