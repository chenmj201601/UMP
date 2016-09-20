using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common1111
{
    public class ResourceNameNum
    {
         /// <summary>
        /// 资源编号
        /// </summary>
        public long ResourceNum { get; set; }
        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// 资源编码
        /// </summary>
        public long ResourceCode { get; set; }

        public ResourceNameNum()
        { }

        public override string ToString()
        {
            return ResourceName;
        }
    }
}
