using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class FilesItemInfo
    {
        public int RowNumber { get; set; }

        /// <summary>
        /// 教材ID
        /// </summary>
        public long FileID { get; set; }
        /// <summary>
        /// 文件夹主键
        /// </summary>
        public long FolderID { get; set; }
        /// <summary>
        /// 教材名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 教材路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 教材描述
        /// </summary>
        public string FileDescription { get; set; }
        /// <summary>
        /// 教材来源, 0为手动上传,1为录音查询界面添加
        /// </summary>
        public string FromType { get; set; }
        public string mFromType { get; set; }
        /// <summary>
        /// 当由录音界面添加的时候判断是否 0、未加密；1、加密
        /// </summary>
        public string IsEncrytp { get; set; }
        /// <summary>
        /// 上传的教材是否是音频文件(.wav等) 0、否；1、是
        /// </summary>
        public string FileType { get; set; }
        public string mFileType { get; set; }


    }
}
