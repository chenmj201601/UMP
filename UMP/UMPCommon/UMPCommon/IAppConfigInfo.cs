//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    21e0c65e-4ba5-4957-83f4-211cee462843
//        CLR Version:              4.0.30319.42000
//        Name:                     IAppConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                IAppConfigInfo
//
//        created by Charley at 2016/2/3 10:07:30
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 应用配置信息
    /// </summary>
    public interface IAppConfigInfo : IAppInfo
    {
        /// <summary>
        /// 模块名（如：S1202App）
        /// </summary>
        string ModuleName { get; set; }
        /// <summary>
        /// 模块的完整类型（包含命名空间）（如：UMPS1202.S1202App）
        /// </summary>
        string ModuleType { get; set; }
        /// <summary>
        /// 所在浮动面板的的ContentID
        /// </summary>
        string PanelName { get; set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        string StartArgs { get; set; }
        /// <summary>
        /// 模块程序版本识别串，如：UMPS1202_8_02_002_23
        /// </summary>
        string Version { get; set; }
        /// <summary>
        /// Session信息，模块可根据此Session更新自身的Session以获得当前全局信息
        /// </summary>
        SessionInfo Session { get; set; }
        /// <summary>
        /// 模块所在容器
        /// </summary>
        object Container { get; set; }
        /// <summary>
        /// 关联的App
        /// </summary>
        object UMPApp { get; set; }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="session"></param>
        void Update(SessionInfo session);
    }
}
