namespace Common3601
{
    public class NodeInfo
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
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long LongCreatorId { get; set; }
        public string StrCreatorName { get; set; }
        public string StrCreatorTime { get; set; }
        /// <summary>
        /// 被分配人ID
        /// </summary>
        public string StrUserId1 { get; set; }
        public string StrUserId2 { get; set; }
        public string StrUserId3 { get; set; }
    }
}
