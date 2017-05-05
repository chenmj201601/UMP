//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c882ab0c-460b-4a69-bc36-797ad5a8e681
//        CLR Version:              4.0.30319.18444
//        Name:                     MachineInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                MachineInfo
//
//        created by Charley at 2015/4/1 17:10:27
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// Sftp服务器信息
    /// </summary>
    public class SftpServerInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string HostAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int HostPort { get; set; }
    }
}
