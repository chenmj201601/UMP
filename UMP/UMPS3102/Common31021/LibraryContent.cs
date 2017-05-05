using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 教材详细信息（T_BookDetails）
    /// </summary>
    public class LibraryContent
    {
        /// <summary>
        /// 教材ID
        /// </summary>
        public string BookID { get; set; }
        /// <summary>
        /// 教材库文件夹主键,T_31_058_00000.C001
        /// </summary>
        public string DirID { get; set; }
        /// <summary>
        /// 教材名称
        /// </summary>
        public string BookName { get; set; }
        /// <summary>
        /// 教材对应路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 教材描述
        /// </summary>
        public string Describle { get; set; }
        /// <summary>
        /// 来源 （0为手动上传,1为录音查询界面添加）
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 当由录音界面添加的时候判断是否写入该值 0/1 未加密/加密
        /// </summary>
        public string IsEncrytp { get; set; }
        /// <summary>
        /// 上传的教材是否是音频文件(.wav等) 0/1 否/是
        /// </summary>
        public string IsMedia { get; set; }
    }
}
