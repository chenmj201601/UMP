//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    af50927a-98ba-4270-bcc6-66bad44f830b
//        CLR Version:              4.0.30319.18063
//        Name:                     SDKReturn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                SDKReturn
//
//        created by Charley at 2015/7/30 9:45:42
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 返回消息
    /// </summary>
    [DataContract]
    public class SDKReturn
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        [DataMember]
        public bool Result { get; set; }
        /// <summary>
        /// 操作结果代码，0 表示操作成功，大于 0 表示响应的错误代码
        /// </summary>
        [DataMember]
        public int Code { get; set; }
        /// <summary>
        /// 错误消息或调试消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 请求数据（可以是xml序列化的）
        /// </summary>
        [DataMember]
        public string Data { get; set; }
        /// <summary>
        /// 请求数据列表，如果请求数据有多个，可以使用ListData
        /// </summary>
        [DataMember]
        public List<string> ListData { get; set; }
        /// <summary>
        /// 请求的数据集
        /// </summary>
        [DataMember]
        public DataSet DataSetData { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        [DataMember]
        public string Other01 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        [DataMember]
        public string Other02 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        [DataMember]
        public string Other03 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        [DataMember]
        public string Other04 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        [DataMember]
        public string Other05 { get; set; }

        /// <summary>
        /// 创建一个返回对象
        /// </summary>
        public SDKReturn()
        {
            Result = true;
            ListData = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("[Result:{0}][Code:{1}]", Result, Code);
        }
    }
}
