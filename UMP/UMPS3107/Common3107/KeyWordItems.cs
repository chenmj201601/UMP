using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    public class KeyWordItems
    {
        /// <summary>
        /// 记录流水号 T_51_009.c002
        /// </summary>
        public long SerialID { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 关键词编号
        /// </summary>
        public long KeyWordID { get; set; }

        /// <summary>
        /// 关键词描述
        /// </summary>
        public string KeyWordSrt { get; set; }

        /// <summary>
        /// 关键词内容编号
        /// </summary>
        public long KWContentID { get; set; }

        /// <summary>
        /// 关键词内容描述
        /// </summary>
        public string KWContent { get; set; }

        /// <summary>
        /// 是否启用 T_51_007.c003
        /// </summary>
        public bool IsUsed { get; set; }


        /// <summary>
        /// 是否绑定 是否启用 T_51_008.c003
        /// </summary>
        public bool IsBinding { get; set; }
    }
}
