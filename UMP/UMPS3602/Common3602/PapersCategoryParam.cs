namespace Common3602
{
    public class PapersCategoryParam
    {
        #region 试题类型
        /// <summary>
        /// 主键:试题类型编号
        /// </summary>
        public long LongNum { get; set; }

        /// <summary>
        /// 试题类型名称
        /// </summary>
        public string StrName { get; set; }

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
