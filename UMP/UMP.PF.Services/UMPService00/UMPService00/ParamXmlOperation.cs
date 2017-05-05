using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using PFShareClassesS;

namespace UMPService00
{
    public class ParamXmlOperation
    {
        /// <summary>
        /// 启用备机 更新xml
        /// </summary>
        /// <param name="strSourceKey">备机key</param>
        /// <param name="strTargetKey">主机key</param>
        public static void StartBackupMachine(string strSourceKey, string strTargetKey)
        {
            DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
            if (!dir.Exists)
            {
                UMPService00.WriteLog(System.Diagnostics.EventLogEntryType.Error,"Config director is not exists");
                return;
            }
            FileInfo[] lstFileList = dir.GetFiles("*.xml");
            XmlDocument xmlDoc = null;
            XMLOperator xmlOperator = null;
            foreach (FileInfo file in lstFileList)
            {
                //如果不是参数的xml 跳过
                if (!file.Name.ToLower().StartsWith("umpparam_"))
                {
                    continue;
                }
                UMPService00.WriteLog("File Name = " + file.Name);
                xmlDoc = new XmlDocument();
                xmlDoc.Load(file.FullName);
                xmlOperator = new XMLOperator(xmlDoc);
                #region 修改备机xml部分
                XmlNode node = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "ModuleNumber", strSourceKey);
                if (node != null)
                {
                    string strStandByRole = xmlOperator.SelectAttrib(node, "StandByRole");
                    if (!strStandByRole.Equals("3"))
                    {
                        UMPService00.WriteLog(System.Diagnostics.EventLogEntryType.Error,"The backup machine module number is error");
                        break;
                    }
                    //查找这个属性 如果返回值为空 表示没有这个属性
                    string strAttrContent = xmlOperator.SelectAttrib(node, "ReplaceModuleNumber");

                    UMPService00.WriteLog("old ReplaceModuleNumber = " + strAttrContent);
                    if (string.IsNullOrEmpty(strAttrContent))
                    {
                        //没有这个属性 则增加
                        xmlOperator.InsertAttrib(node, "ReplaceModuleNumber", strTargetKey);
                    }
                    else
                    {
                        bool bo = xmlOperator.UpdateAttrib(node, "ReplaceModuleNumber", strTargetKey);
                    }
                }
                else
                {
                    
                    UMPService00.WriteLog("file " + file.Name + ", module number = " + strSourceKey + " in voice server ,node is null"); ;
                }
                
                #endregion

                #region 修改主机xml部分
                node = xmlOperator.SelectNodeByAttribute("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer", "ModuleNumber", strTargetKey);
                if (node != null)
                {
                    string strStandByRole = xmlOperator.SelectAttrib(node, "StandByRole");
                    if (!strStandByRole.Equals("0"))
                    {
                        UMPService00.WriteLog(System.Diagnostics.EventLogEntryType.Error,"The main machine module number is error,StandByRole = " + strStandByRole);
                        break;
                    }
                    //查找这个属性 如果返回值为空 表示没有这个属性
                    string strAttrContent = xmlOperator.SelectAttrib(node, "ReplaceModuleNumber");
                    UMPService00.WriteLog("old ReplaceModuleNumber = " + strAttrContent);
                    if (string.IsNullOrEmpty(strAttrContent))
                    {
                        //没有这个属性 则增加
                        xmlOperator.InsertAttrib(node, "ReplaceModuleNumber", strSourceKey);
                    }
                    else
                    {
                        bool bo = xmlOperator.UpdateAttrib(node, "ReplaceModuleNumber", strSourceKey);
                    }
                }
                else
                {
                    UMPService00.WriteLog("file " + file.Name + ", module number = " + strSourceKey + " in voice server ,node is null");
                }
                
                #endregion

                xmlOperator.Save(file.FullName);
                UMPService00.WriteLog("save file " + file.FullName);
            }
        }

        /// <summary>
        /// 获取备机代替的主机Key/ModuleNumber
        /// </summary>
        public static string GetBackupMachineInfo()
        {
            string strKey = string.Empty;
            DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
            if (!dir.Exists)
            {
                UMPService00.WriteLog("Config director is not exists");
                return strKey;
            }
            string strSimpleFilePath = dir.FullName + "\\umpparam_simp.xml";
            FileInfo fileInfo = new FileInfo(strSimpleFilePath);
            if (!fileInfo.Exists)
            {
                UMPService00.WriteLog("umpparam_simp.xml is not exists");
                return strKey;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strSimpleFilePath);
            XMLOperator xmlOp = new XMLOperator(xmlDoc);
            XmlNode VoiceServersNodes = xmlOp.SelectNode("Configurations/Configuration/Sites/Site/VoiceServers", "");
            if (VoiceServersNodes.ChildNodes.Count <= 0)
            {
                UMPService00.WriteLog("No voiceserver");
                return strKey;
            }

            foreach (XmlNode node in VoiceServersNodes.ChildNodes)
            {
                strKey = xmlOp.SelectAttrib(node, "ReplaceModuleNumber");
                if (!string.IsNullOrEmpty(strKey))
                {
                    break;
                }
            }
            //strKey = xmlOp.SelectAttrib()
            return strKey;
        }

        /// <summary>
        /// 根据ModuleNumber从voicexml中读取IP
        /// </summary>
        /// <param name="strModuleNumber"></param>
        /// <returns></returns>
        public static string GetVoiceServerHostByModuleNumber(int strModuleNumber)
        {
            string strHost = string.Empty;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
                if (!dir.Exists)
                {
                    UMPService00.WriteLog( "Config director is not exists");
                    return strHost;
                }
                string strFileName = string.Format("umpparam_voc{0:0000}.xml", strModuleNumber);
                string strVoiceleFilePath = dir.FullName + "\\" + strFileName;
                FileInfo fileInfo = new FileInfo(strVoiceleFilePath);
                if (!fileInfo.Exists)
                {
                    UMPService00.WriteLog("umpparam_simp.xml is not exists");
                    return strHost;
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strVoiceleFilePath);
                XMLOperator xmlOperator = new XMLOperator(xmlDoc);
                XmlNode hostNode = xmlOperator.SelectNode("Configurations/Configuration/Sites/Site/VoiceServers/VoiceServer/HostAddress", "");
                strHost = xmlOperator.SelectAttrib(hostNode, "Value");
                string LStrVerificationCode101 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                strHost = EncryptionAndDecryption.EncryptDecryptString(strHost, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                UMPService00.WriteLog(EventLogEntryType.Warning,"Host = " + strHost);
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog("GetVoiceServerHostByModuleNumber error:" + ex.Message);
            }
            return strHost;
        }

    }
}
