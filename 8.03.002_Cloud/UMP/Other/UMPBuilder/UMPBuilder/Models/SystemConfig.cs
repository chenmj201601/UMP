//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    61c5d4c9-a786-42aa-a2b2-d7a5371da086
//        CLR Version:              4.0.30319.18063
//        Name:                     SystemConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                SystemConfig
//
//        created by Charley at 2015/12/21 14:11:51
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;

namespace UMPBuilder.Models
{
    [XmlRoot]
    public class SystemConfig
    {
        public const string FILE_NAME = "SytemConfig.xml";

        private List<ProjectInfo> mListProjects;
        private List<UMPFileInfo> mListUMPFiles;
        private List<GlobalSetting> mListSettings;

        /// <summary>
        /// 项目列表
        /// </summary>
        [XmlArray]
        public List<ProjectInfo> ListProjects
        {
            get { return mListProjects; }
        }
        /// <summary>
        /// UMP文件列表
        /// </summary>
        [XmlArray]
        public List<UMPFileInfo> ListUMPFileInfos
        {
            get { return mListUMPFiles; }
        }
        /// <summary>
        /// 选项列表
        /// </summary>
        [XmlArray]
        public List<GlobalSetting> ListSettings
        {
            get { return mListSettings; }
        }

        /// <summary>
        /// 编译工具路径
        /// </summary>
        public string CompilerPath { get; set; }
        /// <summary>
        /// Svn客户端路径
        /// </summary>
        public string SvnProcPath { get; set; }
        /// <summary>
        /// 主目录
        /// </summary>
        public string RootDir { get; set; }
        /// <summary>
        /// 发布目录，打包所需文件的存放目录
        /// </summary>
        public string CopyDir { get; set; }
        /// <summary>
        /// 安装包存放目录
        /// </summary>
        public string PackageDir { get; set; }
        /// <summary>
        /// 更新文件的保存路径
        /// </summary>
        public string UpdateDir { get; set; }

        public SystemConfig()
        {
            mListProjects = new List<ProjectInfo>();
            mListUMPFiles = new List<UMPFileInfo>();
            mListSettings = new List<GlobalSetting>();
        }
    }
}
