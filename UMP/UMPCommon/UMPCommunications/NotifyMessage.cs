//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cd00c958-c97d-4aca-accd-3c3ac91970a3
//        CLR Version:              4.0.30319.18063
//        Name:                     NotifyMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NotifyMessage
//
//        created by Charley at 2015/6/24 9:10:52
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Communications
{
    public class NotifyMessage
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 通知的命令，用于区别不同的通知
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 通知的数据，如果通知的数据有多个，请使用ListData
        /// </summary>
        public string Data { get; set; }

        private List<string> mListData;
        /// <summary>
        /// 通知的数据列表
        /// </summary>
        public List<string> ListData
        {
            get { return mListData; }
        }

        public NotifyMessage()
        {
            mListData = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Command, SessionID);
        }
        /// <summary>
        /// 记录通知的详细信息
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
