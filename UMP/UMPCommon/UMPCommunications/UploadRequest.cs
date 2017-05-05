//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    e9832343-2ab1-411c-b3d1-50aebe78d567
//        CLR Version:              4.0.30319.42000
//        Name:                     UploadRequest
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                UploadRequest
//
//        Created by Charley at 2016/10/8 10:04:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using VoiceCyber.UMP.Common;


namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 上传文件专用的请求消息，只适用小文件的上传，文本文件，图片等
    /// 与WebRequest类似，增加了Cotent属性，表示文件内容数据
    /// 文件保存位置以及文件大小等信息一般在ListData中指定
    /// </summary>
    [DataContract]
    public class UploadRequest
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
        /// 文件内容数据
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UploadRequest()
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
