using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common61011
{
    public class QueryConditionItem
    {
        public long QueryConditionCode { get; set; }

        public long QueryItemCode { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 类型。如果是1：表示45表里有数据。0：只有44表里有数据
        /// </summary>
        public int Type { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }

        public string Value3 { get; set; }

        public string Value4 { get; set; }

        public string Value5 { get; set; }
    }
}
