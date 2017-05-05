using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    public class CVoiceServer
    {
        /// <summary>
        /// 录音服务器IP
        /// </summary>
        public string VoiceServer { get; set; }

        /// <summary>
        /// 是否启用 是，否
        /// </summary>
        public string EanbleEncryption { get; set; }

        /// <summary>
        /// 是否启用 1，0
        /// </summary>
        public string NumEanbleEncryption { get; set; }

        /// <summary>
        /// 是否启用 1，0
        /// </summary>
        public string IPResourceID { get; set; }
    }
}
