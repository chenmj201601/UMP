//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ed0341ac-d971-4e76-8a94-280ca1303eec
//        CLR Version:              4.0.30319.18444
//        Name:                     RequestMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                CommonService03
//        File Name:                RequestMessage
//
//        created by Charley at 2015/3/27 9:51:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.CommonService03
{
    /// <summary>
    /// 请求消息
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// 请求的命令
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 请求的数据，如果请求的数据有多个，请使用ListData
        /// </summary>
        public string Data { get; set; }

        private List<string> mListData;
        /// <summary>
        /// 请求的数据列表
        /// </summary>
        public List<string> ListData
        {
            get { return mListData; }
        }

        public RequestMessage()
        {
            mListData = new List<string>();
        }
    }
}
