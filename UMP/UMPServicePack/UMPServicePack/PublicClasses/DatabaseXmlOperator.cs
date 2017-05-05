//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    54bb5ca5-10e0-47d6-b25b-9c35da77fdc2
//        CLR Version:              4.0.30319.18408
//        Name:                     DatabaseXmlOperator
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPServicePack.PublicClasses
//        File Name:                DatabaseXmlOperator
//
//        created by Charley at 2016/7/29 11:06:43
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Xml;
using UMPServicePackCommon;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;


namespace UMPServicePack.PublicClasses
{
    public class DatabaseXmlOperator
    {
        /// <summary>
        /// 从配置文件中读取数据库信息
        /// </summary>
        /// <returns></returns>
        public static OperationReturn GetDBInfo(string strFilePath, ref DatabaseInfo dbInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            dbInfo = new DatabaseInfo();
            try
            {
                string LStrXmlFileName = string.Empty;
                LStrXmlFileName = Path.Combine(strFilePath, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    optReturn.Code = ConstDefines.RET_Database_Null;
                    optReturn.Result = false;
                    return optReturn;
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                if (LXmlNodeListDatabase.Count <= 0)
                {
                    optReturn.Code = ConstDefines.RET_Database_Null;
                    optReturn.Result = false;
                    optReturn.Message = "database xml node count  = " + LXmlNodeListDatabase.Count;
                    return optReturn;
                }

                string LStrAttributesData = string.Empty;
                #region 数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {

                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptOperations.DecryptWithM004(LStrAttributesData);
                    //UMPService00.IEventLog.WriteEntry("Database Enable: " + LStrAttributesData);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    dbInfo.TypeID = int.Parse(LStrAttributesData);


                    //数据库服务器名或IP地址
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P04"].Value;
                    dbInfo.Host = EncryptOperations.DecryptWithM004(LStrAttributesData);

                    //数据库服务端口
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P05"].Value;
                    dbInfo.Port = int.Parse(EncryptOperations.DecryptWithM004(LStrAttributesData));

                    //数据库名或Service Name
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P06"].Value;
                    dbInfo.DBName = EncryptOperations.DecryptWithM004(LStrAttributesData);

                    //登录用户
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P07"].Value;
                    dbInfo.LoginName = EncryptOperations.DecryptWithM004(LStrAttributesData);

                    //登录密码
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P08"].Value;
                    dbInfo.Password = EncryptOperations.DecryptWithM004(LStrAttributesData);
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
                if (string.IsNullOrEmpty(dbInfo.DBName))
                {
                    optReturn.Code = ConstDefines.RET_Database_Null;
                    optReturn.Result = false;
                    return optReturn;
                }
                optReturn.Data = dbInfo;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Code = ConstDefines.Get_Database_Info_Exception;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
    }
}
