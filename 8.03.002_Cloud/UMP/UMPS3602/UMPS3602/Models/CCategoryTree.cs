﻿using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3602.Models
{
    public class CCategoryTree : CheckableItemBase
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public long LongNum { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string StrName { get; set; }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public long LongParentNodeId { get; set; }
        /// <summary>
        /// 父节点名称
        /// </summary>
        public string StrParentNodeName { get; set; }

        private Brush mChangeBrush;

        public Brush ChangeBrush
        {
            get { return mChangeBrush; }
            set
            {
                mChangeBrush = value;
                OnPropertyChanged("ChangeBrush");
            }

        }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long LongFounderId { get; set; }
        public string StrFounderName { get; set; }
        public string StrDateTime { get; set; }
    }
}
