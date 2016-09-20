//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    458fb5e7-a97c-4ec9-95fb-126152c990c2
//        CLR Version:              4.0.30319.42000
//        Name:                     AppConfigInfo
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                AppConfigInfo
//
//        created by Charley at 2016/2/19 13:34:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Xml.Serialization;
using VoiceCyber.UMP.Common;


namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 应用配置信息
    /// </summary>
    [XmlRoot]
    public class AppConfigInfo : IAppConfigInfo
    {
        public const string FILE_NAME = "AppInfo.xml";

        /// <summary>
        /// 每个Application应该在创建的时候自动生成一个唯一标识，通常这是Guid串
        /// </summary>
        [XmlAttribute]
        public string SessionID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [XmlAttribute]
        public int AppType { get; set; }
        /// <summary>
        /// 表示应用的可识别名称
        /// </summary>
        [XmlAttribute]
        public string AppName { get; set; }
        /// <summary>
        /// 表示应用的标题
        /// </summary>
        [XmlAttribute]
        public string AppTitle { get; set; }
        /// <summary>
        /// 所属模块号，4位小模块号
        /// 这个模块号通常与小模块号对应，但有时候一个Application可能有多个模块，这时通常使用两位的大模块号+00表示
        /// 这个模块号会影响操作日志，文本日志，语言包等功能
        /// </summary>
        [XmlAttribute]
        public int ModuleID { get; set; }
        /// <summary>
        /// 最后活动时间
        /// </summary>
        [XmlAttribute]
        public DateTime LastActiveTime { get; set; }
        /// <summary>
        /// 模块名（如：S1202App）
        /// </summary>
        [XmlAttribute]
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块的完整类型（包含命名空间）（如：UMPS1202.S1202App）
        /// </summary>
        [XmlAttribute]
        public string ModuleType { get; set; }
        /// <summary>
        /// 所在浮动面板的的ContentID
        /// </summary>
        [XmlAttribute]
        public string PanelName { get; set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        [XmlAttribute]
        public string StartArgs { get; set; }
        /// <summary>
        /// Session信息，模块可根据此Session更新自身的Session以获得当前全局信息
        /// </summary>
        [XmlIgnore]
        public SessionInfo Session { get; set; }
        /// <summary>
        /// 模块所在容器
        /// </summary>
        [XmlIgnore]
        public object Container { get; set; }
        /// <summary>
        /// 关联的App
        /// </summary>
        [XmlIgnore]
        public object UMPApp { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        [XmlIgnore]
        public string Icon { get; set; }
        /// <summary>
        /// 模块程序版本识别串，如：UMPS1202_8_02_002_23
        /// </summary>
        [XmlAttribute]
        public string Version { get; set; }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="session"></param>
        public void Update(SessionInfo session)
        {
            SessionID = session.SessionID;
            Session = session;
        }
    }
}
