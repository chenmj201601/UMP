//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    24b591e9-17d4-4dc0-b6ff-ab4ab10abbc9
//        CLR Version:              4.0.30319.18063
//        Name:                     RequestMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                RequestMessage
//
//        created by Charley at 2015/6/23 17:14:05
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Communications
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

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Command, SessionID);
        }

        /// <summary>
        /// 记录请求的详细信息
        /// </summary>
        /// <returns></returns>
        public string LogInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Command:{0}\r\n", Command));
            sb.Append(string.Format("SessionID:{0}\r\n", SessionID));
            sb.Append(string.Format("Data:{0}\r\n", Data));
            for (int i = 0; i < mListData.Count; i++)
            {
                sb.Append(string.Format("ListData{0}:{1}\r\n", i, mListData[i]));
            }
            return sb.ToString();
        }
    }
}
