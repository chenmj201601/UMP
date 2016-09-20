//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a991200e-4083-4f70-859e-7796a422b780
//        CLR Version:              4.0.30319.18063
//        Name:                     SDKRequest
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                SDKRequest
//
//        created by Charley at 2015/7/30 9:45:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 请求消息
    /// </summary>
    [DataContract]
    public class SDKRequest
    {
        /// <summary>
        /// 请求代码
        /// </summary>
        [DataMember]
        public int Code { get; set; }
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
        /// 创建一个SDKRequest对象，并初始化
        /// </summary>
        public SDKRequest()
        {
            ListData = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("[Code:{0}]", Code);
        }
    }
}
