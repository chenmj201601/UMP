//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cf30b2a6-f565-4633-804f-571d7470ee6d
//        CLR Version:              4.0.30319.18444
//        Name:                     WebRequest
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                WebRequest
//
//        created by Charley at 2014/8/25 11:49:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// Web请求消息
    /// </summary>
    [DataContract]
    public class WebRequest
    {
        /// <summary>
        /// 会话信息
        /// </summary>
        [DataMember]
        public SessionInfo Session { get; set; }
        /// <summary>
        /// 请求代码，用于区分不同的操作
        /// </summary>
        [DataMember]
        public int Code { get; set; }
        /// <summary>
        /// 请求的数据，一般是xml序列化的
        /// </summary>
        [DataMember]
        public string Data { get; set; }
        /// <summary>
        /// 如果请求的数据是集合，填充此集合
        /// </summary>
        [DataMember]
        public List<string> ListData { get; set; }
        /// <summary>
        /// 如果请求的数据是数据集，填充此数据集
        /// </summary>
        [DataMember]
        public DataSet DataSetData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WebRequest()
        {
            ListData = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", Code, Session);
        }
        /// <summary>
        /// 记录请求的详细信息
        /// </summary>
        /// <returns></returns>
        public string LogInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}]{1}\r\n", Code, Session));
            sb.Append(string.Format("Data:{0}\r\n", Data)); 
            if (ListData != null)
            {
                for (int i = 0; i < ListData.Count; i++)
                {
                    sb.Append(string.Format("ListData:{0}\r\n", ListData[i]));
                }
            }
            return sb.ToString();
        }
    }
}
