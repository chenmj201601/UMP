//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6f396aec-48c7-44e4-964c-5a4754d71283
//        CLR Version:              4.0.30319.18063
//        Name:                     ReturnMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                ReturnMessage
//
//        created by Charley at 2015/6/23 17:14:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Communications
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

        public override string ToString()
        {
            return string.Format("{0}[{1}][{2}]{3}", Result, Code, SessionID, Command);
        }

        /// <summary>
        /// 记录回复的详细信息
        /// </summary>
        /// <returns></returns>
        public string LogInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Result:{0}\r\n", Result));
            sb.Append(string.Format("Code:{0}\r\n", Code));
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
