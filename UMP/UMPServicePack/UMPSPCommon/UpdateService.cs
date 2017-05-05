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

namespace UMPSPCommon
{
    /// <summary>
    /// 更新服务的相关操作
    /// </summary>
    [XmlRoot(ElementName = "UpdateService", Namespace = "http://www.voicecyber.com/UMP/Updaters/2016/05")]
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
    }
}
