namespace Common3604
{
    public class ContentsParam
    {
        #region 目录参数
        /// <summary>
        /// 主键:编号
        /// </summary>
        public long LongNodeId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string StrNodeName { get; set; }

        /// <summary>
        /// 试题类型父级节点ID,根节点默认0
        /// </summary>
        public long LongParentNodeId { get; set; }

        /// <summary>
        /// 试题类型父级节点名,根节点默认0
        /// </summary>
        public string StrParentNodeName { get; set; }

        /// <summary>
        /// 创建人ID,对应T_11_005_00000.c001
        /// </summary>
        public long LongFounderId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string StrFounderName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string StrDateTime { get; set; }

        #endregion
    }
}
