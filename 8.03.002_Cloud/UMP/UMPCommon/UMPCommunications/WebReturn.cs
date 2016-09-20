//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d7173eca-2a22-4be6-b789-ca58cd7f674c
//        CLR Version:              4.0.30319.18444
//        Name:                     WebReturn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                WebReturn
//
//        created by Charley at 2014/8/25 11:25:23
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
    /// Web请求的响应
    /// </summary>
    [DataContract]
    public class WebReturn
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        [DataMember]
        public bool Result { get; set; }
        /// <summary>
        /// 会话信息
        /// </summary>
        [DataMember]
        public SessionInfo Session { get; set; }
        /// <summary>
        /// 响应代码，0表示没有错误，其他代表错误号
        /// </summary>
        [DataMember]
        public int Code { get; set; }
        /// <summary>
        /// 响应数据，一般是xml序列化后的数据
        /// </summary>
        [DataMember]
        public string Data { get; set; }
        /// <summary>
        /// 有些情况下响应数据是集合
        /// </summary>
        [DataMember]
        public List<string> ListData { get; set; }
        /// <summary>
        /// 有些情况下响应的数据是数据集
        /// </summary>
        [DataMember]
        public DataSet DataSetData { get; set; }
        /// <summary>
        /// 其他消息，通常为错误消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WebReturn()
        {
            ListData = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]{2}", Result, Code, Session);
        }
        /// <summary>
        /// 记录返回的详细信息
        /// </summary>
        /// <returns></returns>
        public string LogInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}[{1}]{2}\r\n", Result, Code, Session));
            sb.Append(string.Format("Data:{0}\r\n", Data));
            sb.Append(string.Format("Message:{0}\r\n", Message));
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
