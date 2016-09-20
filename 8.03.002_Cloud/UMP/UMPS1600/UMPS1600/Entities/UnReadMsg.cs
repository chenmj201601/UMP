using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1600.Entities
{
    public class UnReadMsg
    {
        public UnReadMsg(string strSender, string strMsg)
        {
            Sender = strSender;
            Msg = strMsg;
        }

        private string _Sender;

        /// <summary>
        /// 消息发送人
        /// </summary>
        public string Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }
        private string _Msg;

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg
        {
            get { return _Msg; }
            set { _Msg = value; }
        }

        private DateTime _SendTime;

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime
        {
            get { return _SendTime; }
            set { _SendTime = value; }
        }
    }
}
