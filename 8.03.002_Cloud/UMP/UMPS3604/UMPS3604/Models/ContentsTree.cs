using System.Collections.Generic;
using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3604.Models
{
    public class ContentsTree : CheckableItemBase
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public long LongNodeId { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string StrNodeName { get; set; }
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
        /// 子节点
        /// </summary>
        public List<ContentsTree> LstChildInfos { get; set; }

        /// <summary>
        /// 同级节点
        /// </summary>
        public List<ContentsTree> LstNodeInfos { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long LongFounderId { get; set; }
        public string StrFounderName { get; set; }
        public string StrDateTime { get; set; }
    }
}
