using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class FolderInfo
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public long FolderID { get; set; }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public long TreeParentID { get; set; }
        /// <summary>
        /// 父节点名称
        /// </summary>
        public string TreeParentName { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorTime { get; set; }
        /// <summary>
        /// 被分配人ID
        /// </summary>
        public string UserID1 { get; set; }
        public string UserID2 { get; set; }
        public string UserID3 { get; set; }

    }
}
