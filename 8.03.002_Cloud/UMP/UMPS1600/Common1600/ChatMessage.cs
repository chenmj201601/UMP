using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common1600
{
    /// <summary>
    /// 聊天消息实体类
    /// </summary>
    public class ChatMessage
    {
        private long _SenderID;

        /// <summary>
        /// 发送人ID
        /// </summary>
        public long SenderID
        {
            get { return _SenderID; }
            set { _SenderID = value; }
        }
        private string _SenderName;

        /// <summary>
        /// 发送人名字
        /// </summary>
        public string SenderName
        {
            get { return _SenderName; }
            set { _SenderName = value; }
        }
        private long _ReceiverID;

        /// <summary>
        /// 接收人ID
        /// </summary>
        public long ReceiverID
        {
            get { return _ReceiverID; }
            set { _ReceiverID = value; }
        }
        private string _ReceiverName;

        /// <summary>
        /// 接收者名字
        /// </summary>
        public string ReceiverName
        {
            get { return _ReceiverName; }
            set { _ReceiverName = value; }
        }
        private long _CookieID;

        /// <summary>
        /// 会话ID
        /// </summary>
        public long CookieID
        {
            get { return _CookieID; }
            set { _CookieID = value; }
        }
        private long _ResourceID;

        /// <summary>
        /// 资源ID
        /// </summary>
        public long ResourceID
        {
            get { return _ResourceID; }
            set { _ResourceID = value; }
        }
        private string _MsgContent;

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgContent
        {
            get { return _MsgContent; }
            set { _MsgContent = value; }
        }
    }
}
