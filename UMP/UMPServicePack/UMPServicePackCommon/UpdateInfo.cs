//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    894fed5f-16e4-49c8-ad90-51c02452d010
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateInfo
//
//        created by Charley at 2016/5/9 9:46:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPServicePackCommon
{
    /// <summary>
    /// 更新信息详情
    /// </summary>
    [XmlRoot(ElementName = "UpdateInfo", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [XmlAttribute]
        public string Version { get; set; }
        /// <summary>
        /// 依赖版本
        /// </summary>
        [XmlAttribute]
        public string BaseVersion { get; set; }
        /// <summary>
        /// 安装包类型
        /// </summary>
        [XmlAttribute]
        public int Type { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        [XmlAttribute]
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// 描述/说明
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        private List<UpdateModule> mListModules = new List<UpdateModule>();

        /// <summary>
        /// 更新的功能模块列表
        /// </summary>
        [XmlArray(ElementName = "Modules")]
        [XmlArrayItem(ElementName = "Module")]
        public List<UpdateModule> ListModules
        {
            get { return mListModules; }
        }

        private List<UpdateFile> mListFiles = new List<UpdateFile>();
        /// <summary>
        /// 更新的文件列表（包括文件夹）
        /// </summary>
        [XmlArray(ElementName = "Files")]
        [XmlArrayItem(ElementName = "File")]
        public List<UpdateFile> ListFiles
        {
            get { return mListFiles; }
        }

        private List<UpdateSqlScript> mListSqlScripts = new List<UpdateSqlScript>();
        /// <summary>
        /// 更新的数据库脚本（脚本文件）列表
        /// </summary>
        [XmlArray(ElementName = "SqlScripts")]
        [XmlArrayItem(ElementName = "SqlScript")]
        public List<UpdateSqlScript> ListSqlScripts
        {
            get { return mListSqlScripts; }
        }

        private List<UpdateService> mListServices = new List<UpdateService>();
        /// <summary>
        /// 更新中涉及到的服务列表
        /// </summary>
        [XmlArray(ElementName = "Services")]
        [XmlArrayItem(ElementName = "Service")]
        public List<UpdateService> ListServices
        {
            get { return mListServices; }
        }

        private List<UpdateFollow> mListFollows = new List<UpdateFollow>();
        /// <summary>
        /// 更新后的后续操作列表
        /// </summary>
        [XmlArray(ElementName = "Follows")]
        [XmlArrayItem(ElementName = "Follow")]
        public List<UpdateFollow> ListFollows
        {
            get { return mListFollows; }
        }

        public const string FILE_NAME = "UpdateInfo.xml";
    }
}
