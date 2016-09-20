using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlHelper;
using System.Data;
using System.IO;
using System.Xml;

namespace UMP.PF.MAMT.Classes
{
    public class ServerConfigOperationInLocal
    {

        /// <summary>
        /// 获得所有已经连接过的服务器配置信息
        /// </summary>
        /// <returns></returns>
        public static List<ServerInfomation> GetAllServerInfo()
        {
            List<ServerInfomation> lstResult = new List<ServerInfomation>();
            bool isExists = CheckFileExists();
            if (isExists)
            {
                XMLOperator xmlOperator = new XMLOperator(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\ServerConfig.xml");
                DataSet ds = xmlOperator.GetDs("Root");
                if (ds.Tables.Count > 0)
                {
                    ServerInfomation server;
                    DataRow row = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        row = ds.Tables[0].Rows[i];
                        server = new ServerInfomation();
                        server.Host = row["Host"].ToString();
                        server.Port = row["Port"].ToString();
                        server.UserName = row["Username"].ToString();
                        lstResult.Add(server);
                    }
                }
                else
                {
                    ServerInfomation server = new ServerInfomation();
                    server.Host = "127.0.0.1";
                    server.Port = "8081";
                    server.UserName = "administrator";
                    lstResult.Add(server);
                }
            }
            else
            {
                ServerInfomation server = new ServerInfomation();
                server.Host = "127.0.0.1";
                server.Port = "8081";
                server.UserName = "administrator";
                lstResult.Add(server);
            }
            return lstResult;
        }

        /// <summary>
        /// 检查文件是否存在 存在返回True 不存在 创建文件 并返回false
        /// 根据返回值来判断 是否还需要去读取文件内容
        /// </summary>
        /// <returns></returns>
        private static bool CheckFileExists()
        {
            bool isExists = false;
            App.GStrLoginUserApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DirectoryInfo dir = new DirectoryInfo(App.GStrLoginUserApplicationDataPath);
            List<DirectoryInfo> dirChilds = dir.GetDirectories().ToList();
            DirectoryInfo dirConfig;
            //如果存在UMP.PF.MAMT文件夹
            if (dirChilds.Exists(p => p.Name == "UMP.PF.MAMT"))
            {
                dirConfig = new DirectoryInfo(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT");
                List<FileInfo> files = dirConfig.GetFiles().ToList();
                //如果没有找到配置文件
                if (!files.Exists(p => p.Name == "ServerConfig.xml"))
                {
                    InitConfig();
                    isExists = false;
                }
                else
                {
                    isExists = true;
                }
            }//end 如果存在UMP.PF.MAMT文件夹
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT");
                dirInfo.Create();
                InitConfig();
                isExists = false;
            }
            return isExists;
        }

        /// <summary>
        /// 在没有ServerConfig.xml的情况下 初始化里面的值
        /// </summary>
        private static void InitConfig()
        {
            FileStream fs = new FileStream(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\ServerConfig.xml", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("<?xml version='1.0' encoding='utf-8' ?>");
            sw.WriteLine("<Root>");
            sw.WriteLine("</Root>");
            sw.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 添加服务器配置
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strUsername"></param>
        /// <returns></returns>
        public static int AddServer(string strHost, string strPort, string strUsername, XMLOperator xmlOperator)
        {
            int iResult = 0;
            List<string> lstElements = new List<string>();
            lstElements.Add("Host");
            lstElements.Add("Port");
            lstElements.Add("Username");
            List<string> lstContents = new List<string>();
            lstContents.Add(strHost);
            lstContents.Add(strPort);
            lstContents.Add(strUsername);
            xmlOperator.InsertNode("Root", "Server", lstElements, lstContents);
            xmlOperator.Save(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\ServerConfig.xml");
            return iResult;
        }

        /// <summary>
        ///  更新配置文件
        /// </summary>
        /// <returns></returns>
        public static int UpdateConfigFile(string strHost, string strPort, string strUsername)
        {
            int iResult = 0;
            XMLOperator xmlOperator = new XMLOperator(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\ServerConfig.xml");
            XmlNode node = xmlOperator.SelectNode("Root/Server/Host", strHost);
            
            if (node == null)
            {
                iResult = AddServer(strHost, strPort, strUsername, xmlOperator);
            }
            else
            {
                XmlNodeList childNodes = node.ParentNode.ChildNodes;
                 bool isSuccess =true;
                for (int i = 0; i < childNodes.Count; i++)
                {
                    if (childNodes[i].Name == "Port")
                    {
                       isSuccess = xmlOperator.UpdateNode(childNodes[i], strPort);
                    }
                    else if (childNodes[i].Name == "Username")
                    {
                       isSuccess = xmlOperator.UpdateNode(childNodes[i], strUsername);
                    }
                    if (isSuccess && iResult == 0)
                    {
                        iResult = 0;
                    }
                    else
                    {
                        iResult = -1;
                    }
                }
              
                xmlOperator.Save(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\ServerConfig.xml");
            }
            return iResult;
        }
    }
}
