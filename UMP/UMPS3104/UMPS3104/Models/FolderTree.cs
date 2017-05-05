using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3104.Models
{
    public class FolderTree : CheckableItemBase
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
        /// 磁盘物理路径
        /// </summary>
        public string ParentDiskPath { get; set; }
        /// <summary>
        /// 被分配人ID
        /// </summary>
        public string UserID1 { get; set; }
        public string UserID2 { get; set; }
        public string UserID3 { get; set; }

        //private string mTipNew;
        //public string TipNew
        //{
        //    get { return mTipNew; }
        //    set { mTipNew = value; OnPropertyChanged("TipNew"); }
        //}

        //private string mTipRename;
        //public string TipRename
        //{
        //    get { return mTipRename; }
        //    set { mTipRename = value; OnPropertyChanged("TipRename"); }
        //}

        //private string mTipDelete;
        //public string TipDelete
        //{
        //    get { return mTipDelete; }
        //    set { mTipDelete = value; OnPropertyChanged("TipDelete"); }
        //}

        //private string mTipAllot;
        //public string TipAllot
        //{
        //    get { return mTipAllot; }
        //    set { mTipAllot = value; OnPropertyChanged("TipAllot"); }
        //}

        //private string mTipUpload;
        //public string TipUpload
        //{
        //    get { return mTipUpload; }
        //    set { mTipUpload = value; OnPropertyChanged("TipUpload"); }
        //}


    }
}
