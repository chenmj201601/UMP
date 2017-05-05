using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
namespace UMPService00
{
    public class GloableParamOperation
    {
        XMLOperator xmlOperator = null;
        XmlDocument xmlDoc = null;
        string strFileName = string.Empty;

        public GloableParamOperation(string strPath)
        {
            strFileName = strPath + "\\GlobalSettings\\UMP.Server.01.xml";

            UMPService00.WriteLog("path = " + strFileName);
            xmlDoc = Common.CreateXmlDocumentIfNotExists(strPath + "\\GlobalSettings", "UMP.Server.01.xml", "UMPSetted");
            if (xmlDoc == null)
            {
                UMPService00.WriteLog("xmlDoc is null");
            }
            xmlOperator = new XMLOperator(xmlDoc);
        }

        /// <summary>
        /// 检查是否是ump服务器
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static bool CheckIsUMPServer()
        {
            bool bResult = false;
            try
            {
                string strPath = Common.GetCurrentBaseDirectory().Trim('\\') + "\\GlobalSettings";
                List<string> lstFiles = Directory.GetFiles(strPath).ToList();
                string strXmlFilePath = strPath + @"\" + "UMP.Server.01.xml";
                UMPService00.WriteLog("check umpserver  path = " + strXmlFilePath);
                if (lstFiles.Contains(strXmlFilePath))
                {
                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch
            {
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 获得网站的端口信息
        /// </summary>
        /// <returns></returns>
        public bool GetWebSitePort(ref int iPort)
        {
            bool bResult = false;
            try
            {
                XmlNode node = xmlOperator.SelectNodeByAttribute("UMPSetted/IISBindingProtocol/ProtocolBind", "Used", "1");
                string strPort = xmlOperator.SelectAttrib(node, "BindInfo");
                iPort = int.Parse(strPort);
                bResult = true;
            }
            catch (Exception ex)
            {
                iPort = 0;

                UMPService00.WriteLog("Get port error as umpserver: " + ex.Message + ".\r\nIf not umpserver, please ignore this message");
            }
            return bResult;
        }

        /// <summary>
        /// 从ProgramData获得中获得IIS信息（IP、端口）
        /// </summary>
        /// <returns></returns>
        public bool GetWebSitePort(ref int iPort, ref string strHost)
        {
            bool bResult = false;
            try
            {
                XmlNode node = xmlOperator.SelectNodeByAttribute("UMPSetted/IISBindingProtocol/ProtocolBind", "Used", "1");
                string strPort = xmlOperator.SelectAttrib(node, "BindInfo");
                iPort = int.Parse(strPort);
                strHost = xmlOperator.SelectAttrib(node, "IPAddress");
                bResult = true;
            }
            catch (Exception ex)
            {
                bResult = false;
                UMPService00.WriteLog("Get port error as voiceserver : " + ex.Message + ".\r\nIf not voiceserver, please ignore this message");
            }
            return bResult;
        }

        /// <summary>
        /// 获得认证服务器信息(Host,port)
        /// </summary>
        /// <returns></returns>
        public bool GetAuthenticateServerInfo(ref List<string> ALstResult)
        {
            bool bResult = false;
            try
            {
                XmlNode node = xmlOperator.SelectNodeByAttribute("UMPSetted/IISBindingProtocol/ProtocolBind", "Protocol", "http");
                ALstResult.Add(xmlOperator.SelectAttrib(node, "IPAddress"));
                ALstResult.Add(xmlOperator.SelectAttrib(node, "BindInfo"));
                bResult = true;
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(EventLogEntryType.Warning, "GetAuthenticateServerInfo() error : " + ex.Message);
                ALstResult.Add(string.Empty);
                ALstResult.Add(string.Empty);
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 在Voice服务器上创建UMP网站的地址和端口
        /// </summary>
        /// <param name="strWebSiteHost"></param>
        /// <param name="iIISPort"></param>
        /// <returns></returns>
        public bool WriteWebSiteInfoOnVoiceServer(string strWebSiteHost, int iIISPort)
        {
            bool bIsSucccess = false;
            try
            {
                XmlNode IISNode = xmlOperator.SelectNode("UMPSetted/IISBindingProtocol", "");
                if (IISNode == null)
                {
                    XmlNode rootNode = xmlOperator.SelectNode("UMPSetted", "");
                    IISNode = xmlOperator.InsertNode("IISBindingProtocol", rootNode);
                }
                bool bIsExistsBinding = false;
                foreach (XmlNode node in IISNode.ChildNodes)
                {
                    if (node.Name.Equals("ProtocolBind"))
                    {
                        if (xmlOperator.SelectAttrib(node, "Used") == "1")
                        {
                            bIsExistsBinding = true;
                            xmlOperator.UpdateAttrib(node, "BindInfo", iIISPort.ToString());
                            xmlOperator.UpdateAttrib(node, "IPAddress", strWebSiteHost);
                            xmlOperator.Save(strFileName);
                            break;
                        }
                    }
                }
                //如果不存在ProtocolBind节点 则增加
                if (!bIsExistsBinding)
                {
                   XmlNode node =  xmlOperator.InsertNode("ProtocolBind", IISNode);
                    List<AttribEntry> lstAttribs = new List<AttribEntry>();
                    lstAttribs.Add(new AttribEntry("BindInfo", iIISPort.ToString()));
                    lstAttribs.Add(new AttribEntry("IPAddress", strWebSiteHost));
                    lstAttribs.Add(new AttribEntry("Used", "1"));
                    xmlOperator.InsertAttribs(node, lstAttribs);
                    xmlOperator.Save(strFileName);
                }
                bIsSucccess = true;
            }
            catch(Exception ex)
            {
                UMPService00.WriteLog("WriteWebSiteInfoOnVoiceServer() error : " + ex.Message);
            }
            return bIsSucccess;
        }
    }
}
