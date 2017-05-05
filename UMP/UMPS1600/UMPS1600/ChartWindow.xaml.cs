using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1600.Entities;
using Common1600;

namespace UMPS1600
{
    /// <summary>
    /// ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartWindow 
    {
        public ContacterInListBox con;
        public event Action<long> CloseChatEvent;    //关闭窗口事件
        public event Action<ChatMessage> SendChatMsgEvent;   //发送聊天消息
        public CookieEntity cookie;

        public ChartWindow()
        {
            InitializeComponent();
            this.Loaded += ChartWindow_Loaded;
        }

        void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (con != null)
            {
                txtUserName.Text = con.FullName;
                txtOrgName.Text = con.OrgName;
            }
            txtChartMsg.KeyDown += txtChartMsg_KeyDown;
            InitLanguage();
        }

        void txtChartMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSend_Click(btnSend, null);
            }
        }

        #region InitLanguage
        private void InitLanguage()
        {
            btnClose.Content = InitApp.GetLanguage("1600004", "Close");
            btnSend.Content = InitApp.GetLanguage("1600005", "Send");
        }
        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string strMsg = txtChartMsg.Text;

            if (string.IsNullOrEmpty(strMsg))
            {
                return;
            }
            strMsg = string.Format("{0}    {1}\r\n{2}\r\n", InitApp.Session.UserInfo.UserName, System.DateTime.Now, strMsg);
            txtRecord.Text += strMsg;
            txtChartMsg.Text = "";
            ChatMessage chatMsg = new ChatMessage();
            chatMsg.CookieID = cookie.CookieID;
            chatMsg.MsgContent = strMsg;
            chatMsg.SenderID = InitApp.Session.UserID;
            chatMsg.SenderName = InitApp.Session.UserInfo.UserName;
            chatMsg.ReceiverID = con.UserID;
            chatMsg.ReceiverName = con.UserName;
            chatMsg.ResourceID = cookie.ResourceID;
            if (SendChatMsgEvent != null)
            {
                SendChatMsgEvent(chatMsg);
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (CloseChatEvent != null)
            {
                CloseChatEvent(con.UserID);
            }
        }
    }
}
