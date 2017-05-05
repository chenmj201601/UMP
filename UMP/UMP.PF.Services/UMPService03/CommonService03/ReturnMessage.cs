//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b5636faf-85f1-4eaf-b2ee-be60d27c6200
//        CLR Version:              4.0.30319.18444
//        Name:                     ReturnMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                CommonService03
//        File Name:                ReturnMessage
//
//        created by Charley at 2015/3/27 9:56:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.CommonService03
{
    /// <summary>
    /// 返回的消息
    /// </summary>
    public class ReturnMessage
    {
        /// <summary>
        /// 操作处理的结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 请求时的命令
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 错误或调试消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public string Data { get; set; }
        private List<string> mListData;
        /// <summary>
        /// 返回的数据列表
        /// </summary>
        public List<string> ListData
        {
            get { return mListData; }
        }

        public ReturnMessage()
        {
            mListData = new List<string>();
        }
    }
}
