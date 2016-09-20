//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dbaa2bed-6456-496d-8e34-4db4e15bed7b
//        CLR Version:              4.0.30319.18444
//        Name:                     FileInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                FileInfo
//
//        created by Charley at 2015/3/23 14:29:35
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 文件名称（含扩展名）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件名（不含扩展名）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension { get; set; }
    }
}
