using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Controls;

namespace UMPS1600.Entities
{
    /// <summary>
    /// 会话实例
    /// </summary>
    public class CookieEntity
    {
        private long _UserID;

        /// <summary>
        /// 联系人ID
        /// </summary>
        public long UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        private string _UserName;

        /// <summary>
        /// 联系人名字
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private long _ResourceID;

        /// <summary>
        /// 关联的资源ID 可以为0
        /// </summary>
        public long ResourceID
        {
            get { return _ResourceID; }
            set { _ResourceID = value; }
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
        private int _CookieCreated;

        /// <summary>
        /// 会话是否已经创建 （会话在由会话发起人发送第一条消息时创建）
        ///  0：未创建，1：已创建，2：从别人那里接收来的，不需要自己创建
        /// </summary>
        public int CookieCreated
        {
            get { return _CookieCreated; }
            set { _CookieCreated = value; }
        }
       
        private Timer _CookieTimer;

        /// <summary>
        /// 会话的timer 用来在20分钟无消息时 关闭会话
        /// </summary>
        public Timer CookieTimer
        {
            get { return _CookieTimer; }
            set { _CookieTimer = value; }
        }
        private TabItem _CharTab;

        /// <summary>
        /// 聊天窗口
        /// </summary>
        public TabItem ChatTab
        {
            get { return _CharTab; }
            set { _CharTab = value; }
        }
    }

}
