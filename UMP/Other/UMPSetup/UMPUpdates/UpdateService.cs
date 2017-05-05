//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    242647b9-208d-413f-8228-d14afe6de167
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateService
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateService
//
//        created by Charley at 2016/5/9 10:16:10
//        http://www.voicecyber.com 
//
//======================================================================

using System.Xml.Serialization;

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 更新服务的相关操作
    /// </summary>
    [XmlRoot(ElementName = "UpdateService", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateService : NodeItem
    {
        /// <summary>
        /// 服务名
        /// </summary>
        [XmlAttribute]
        public string ServiceName { get; set; }
        /// <summary>
        /// 服务安装模式
        /// </summary>
        [XmlAttribute]
        public int InstallMode { get; set; }
        /// <summary>
        /// 服务启动模式
        /// </summary>
        [XmlAttribute]
        public int StartMode { get; set; }

        /// <summary>
        /// 启动后等待的时间，StartMode=3时有效 单位：s（秒）
        /// </summary>
        [XmlAttribute]
        public int DelayTime { get; set; }

        /// <summary>
        /// 所属安装包
        /// </summary>
        [XmlAttribute]
        public string Package { get; set; }

        /// <summary>
        /// 文件所在路径类型（TargetPathType）
        /// </summary>
        [XmlAttribute]
        public int TargetPathType { get; set; }

        /// <summary>
        /// 相对于TargetPathType的路径
        /// </summary>
        [XmlAttribute]
        public string TargetPath { get; set; }

        /// <summary>
        /// 安装服务的命令
        /// </summary>
        [XmlAttribute]
        public string InstallCommand { get; set; }

        /// <summary>
        /// 卸载服务的命令
        /// </summary>
        [XmlAttribute]
        public string UnInstallCommand { get; set; }

        /// <summary>
        /// 服务的类型，指示应该用何种方式安装或运行服务
        /// 0：使用InstallUitl安装服务，比如.Net 的服务
        /// 1：使用Install命令安装服务，比如C++ 的服务
        /// </summary>
        [XmlAttribute]
        public int ServiceType { get; set; }
    }
}
