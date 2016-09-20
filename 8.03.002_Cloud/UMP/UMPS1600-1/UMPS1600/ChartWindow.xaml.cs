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

namespace UMPS1600
{
    /// <summary>
    /// ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartWindow 
    {
        public ContacterInListBox con;
        public event DelCloseChatWin CloseChatEvent;    //关闭窗口事件
        public event DelChatMsgSend SendChatMsgEvent;   //发送聊天消息

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
        }

        #region InitLanguage
        private void InitLanguage()
        {
            
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
            txtRecord.Text += string.Format("{0}    {1}\r\n{2}\r\n", InitApp.Session.UserInfo.UserName, System.DateTime.Now, strMsg);
            txtChartMsg.Text = "";
            SendChatMsgEvent(con.UserID, strMsg);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseChatEvent(con.UserName);
        }
    }

    public delegate void DelCloseChatWin(string strUser);
    public delegate void DelChatMsgSend(long lFriendID, string strMsg);
}
