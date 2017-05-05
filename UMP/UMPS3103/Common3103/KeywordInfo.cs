using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
   public class KeywordInfo
    {
        public long SerialNo { get; set; }
        public string Name { get; set; }
        public long ContentNo { get; set; }
        public string Content { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// 0：正常；1：删除；2：禁用
        /// </summary>
        public int State { get; set; }
    }
}
