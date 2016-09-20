using Common1600;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using UMPS1600.Service16001;
using System.Xml;
using System.Timers;
using UMPS1600.Entities;
using System.Collections.ObjectModel;

namespace UMPS1600
{
    /// <summary>
    /// IMMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class IMMainPage : IService16001Callback
    {
        private SessionInfo session;
        static List<string> lstParams = null;
        public event Action<int,string> StatusChangeEvent;
        IService16001 iClient = null;
        ObservableCollection<ContacterInListBox> lstContacters = new ObservableCollection<ContacterInListBox>();

        //心跳计时器
        Timer HeartTimer = null;

        //当前已经打开的聊天窗口  好友名 和TabItem
        Dictionary<string, TabItem> dicChatWins = new Dictionary<string, TabItem>();

        //打开的头像闪动的线程
        Dictionary<string, Timer> dicImgThread = new Dictionary<string, Timer>();

        //保存收到的未读消息
        List<UnReadMsg> lstUnReadMsg = new List<UnReadMsg>();

        public IMMainPage(SessionInfo _session, List<string> _lstParams)
        {
            InitializeComponent();
            try
            {
                session = _session;
                lstParams = _lstParams;
                InitApp.Session = session;
                OperationReturn optReturn = InitApp.InitLanguageInfos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Loaded += IMMainPage_Loaded;
        }

        void IMMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            tabChatWins.SelectionChanged += tabChatWins_SelectionChanged;
        }

        #region 本窗口自己的控件事件
        void tabChatWins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabChatWins.SelectedItem != null)
            {
                TabItem tab = tabChatWins.SelectedItem as TabItem;
                string strHeader = tab.Header.ToString();
                if (dicImgThread.Keys.Contains(strHeader))
                {
                    ChartWindow chartWin = tab.Content as ChartWindow;
                    WriteUnReadMsgToWin(strHeader, chartWin);
                    tab.Foreground = Brushes.Black;
                }

            }
        }
        #endregion


        #region 给wcf发消息
        /// <summary>
        /// 在用户登录UMP或智能客户端时 调用此方法 可更新用户在线信息
        /// </summary>
        public void Login()
        {
            try
            {
                NetTcpBinding binding = CommonFuncs.CreateNetTcpBinding();
                EndpointAddress myEndpoint = CommonFuncs.CreateEndPoint(session.AppServerInfo.Address, "8083");
                DuplexChannelFactory<IService16001> fac = new DuplexChannelFactory<IService16001>(this, binding, myEndpoint);
                iClient = fac.CreateChannel();
                iClient.LoginSystem(session);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LogOff()
        {
            iClient.LogOff(session);
            if (StatusChangeEvent != null)
            {
                StatusChangeEvent((int)UserStatusChangeType.LogOff, "");
            }
        }

        /// <summary>
        /// 给好友发送消息
        /// </summary>
        /// <param name="lFriendID">好友ID</param>
        /// <param name="strMsg">发送的消息</param>
        void chartWin_SendChatMsgEvent(long lFriendID, string strMsg)
        {
            iClient.SendChatMessage(session.UserID, lFriendID, strMsg);
        }
        #endregion


        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HeartTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            iClient.SendHeartMsg();
        }


        #region WCF回调函数
        void IService16001Callback.SendSysMessage(MessageType msgType, string str)
        {
            string strMsg = string.Empty;
            long lUserID = 0;
            long.TryParse(str, out lUserID);

            switch (msgType)
            {
                case MessageType.ConnServerFailed:

                    break;
                case MessageType.FriendOffline:
                    //好友上线 更新名字颜色和图标
                    List<ContacterInListBox> lstFriend = lstContacters.Where(p => p.UserID == lUserID).ToList();
                    if (lstFriend.Count > 0)
                    {
                        lstFriend.First().Status = "1";
                        lstFriend.First().ForegGround = Brushes.Gray;
                        //lstFriend.First().Icon = "Themes/Default/UMP1600/Images/OffLine.ico";
                    }
                    break;
                case MessageType.FriendOnline:
                    //好友上线 更新名字颜色和图标
                    lstFriend = lstContacters.Where(p => p.UserID == lUserID).ToList();
                    if (lstFriend.Count > 0)
                    {
                        lstFriend.First().Status = "1";
                        lstFriend.First().ForegGround = Brushes.Red;
                        //lstFriend.First().Icon = "Themes/Default/UMP1600/Images/OnLine.ico";
                    }
                    break;
                case MessageType.GetFriendFailed:
                    MessageBox.Show(msgType.ToString()+str);
                    break;
                case MessageType.Logined:
                    MessageBox.Show(InitApp.GetLanguage("1600001", "Your account has been logged in at another place, you have been forced offline"));
                    break;
                case MessageType.LoginFailed:
                    MessageBox.Show(str);
                    break;
                case MessageType.LoginSuceess:
                    #region 发送心跳的计时器
                    HeartTimer = new Timer();
                    HeartTimer.AutoReset = true;
                    HeartTimer.Interval = 30 * 1000;        //时间间隔30秒
                    HeartTimer.Elapsed += HeartTimer_Elapsed;
                    HeartTimer.Start();
                    #endregion
                    if (StatusChangeEvent != null)
                    {
                        StatusChangeEvent((int)UserStatusChangeType.Login, "");
                    }
                    break;
                case MessageType.HeartMsg:
                    //MessageBox.Show("心跳消息"+DateTime.Now);
                    break;
                case MessageType.TestMsg:
                    //MessageBox.Show(str);
                    break;
                case MessageType.SendMsgError:
                    strMsg = InitApp.GetLanguage("1600002", "Your message '{0}' failed to send the other party may have been offline, has been sent as an offline message");
                    strMsg = string.Format(strMsg, str);
                    //找到对应的窗口 显示此消息 需要从服务端传来收消息人的名字 要改此函数的第二个参数为list<string> 
                    //MessageBox.Show("adsfs");
                    break;
            }
        }


        /// <summary>
        /// 初始化好友列表
        /// </summary>
        /// <param name="lstFriends"></param>
        void IService16001Callback.InitFriendList(List<string> lstFriends)
        {
            lstContacters.Clear();
            if (lstFriends.Count <= 0)
            {
                //MessageBox.Show("您没有可以管理的用户和坐席");
                return;
            }
            lbContacter.ItemsSource = lstContacters;
            OperationReturn optReturn = null;
            Contacter con = null;
            ContacterInListBox conInList = null;
            for (int i = 0; i < lstFriends.Count; i++)
            {
                optReturn = XMLHelper.DeserializeObject<Contacter>(lstFriends[i]);
                if (optReturn.Result)
                {
                    con = optReturn.Data as Contacter;
                    conInList = new ContacterInListBox();
                    conInList.UserID = con.UserID;
                    conInList.UserName = S1600EncryptOperation.DecryptWithM002(con.UserName);
                    conInList.FullName = S1600EncryptOperation.DecryptWithM002(con.UserName) + " (" + S1600EncryptOperation.DecryptWithM002(con.FullName) + ")";
                    conInList.OrgID = con.OrgID;
                    conInList.OrgName = S1600EncryptOperation.DecryptWithM002(con.OrgName);
                    conInList.ParentOrgID = con.ParentOrgID;
                    conInList.Status = con.Status;
                    conInList.IMGOpacity = "1";
                    string strType = conInList.UserID.ToString().Substring(0, 3);
                    if (strType == ConstValue.RESOURCE_AGENT.ToString())
                    {
                        conInList.Icon = "Themes/Default/UMP1600/Images/agent.ico";
                    }
                    else
                    {
                        conInList.Icon = "Themes/Default/UMP1600/Images/user.ico";
                    }
                    if (conInList.Status == "1")
                    {
                        conInList.ForegGround = Brushes.Red;
                    }
                    else
                    {
                        conInList.ForegGround = Brushes.Gray;
                    }

                    lstContacters.Add(conInList);
                }
            }
        }

        /// <summary>
        /// 双击新建一个好友聊天窗口 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ContacterInListBox con = lbContacter.SelectedValue as ContacterInListBox;
            //双击聊天联系人时 判断是否有新消息 如果有 则修改图片的透明度为1 然后停止对应timer 并将消息写入聊天窗口
            if (dicImgThread.Keys.Contains(con.UserName))
            {
                dicImgThread[con.UserName].Dispose();
                dicImgThread[con.UserName].Close();
                con.IMGOpacity = "1";
            }

            string strFriendName = con.UserName;
            TabItem tab = null;
            ChartWindow chartWin = null;
            if (dicChatWins.Keys.Contains(strFriendName))
            {
                tab = dicChatWins[strFriendName];
                tab.IsSelected = true;

                //将消息写入聊天窗口
                chartWin = tab.Content as ChartWindow;
                WriteUnReadMsgToWin(strFriendName, chartWin);
                return;
            }
            tab = new TabItem();
            string strHeader = string.Empty;
            if (con.UserName.Length > 10)
            {
                strHeader = con.UserName.Substring(0, 6) + "...";
            }
            else
            {
                strHeader = con.UserName;
            }
            tab.Header = strHeader;
            chartWin = new ChartWindow();
            chartWin.con = con;
            chartWin.CloseChatEvent += chartWin_CloseChatEvent;
            chartWin.SendChatMsgEvent += chartWin_SendChatMsgEvent;
            tab.Content = chartWin;
            tabChatWins.Items.Add(tab);
            tab.Tag = con.UserID;
            tab.IsSelected = true;
            WriteUnReadMsgToWin(strFriendName, chartWin);
            dicChatWins.Add(strFriendName, tab);
        }

        /// <summary>
        /// 关闭聊天窗口
        /// </summary>
        /// <param name="strUser"></param>
        void chartWin_CloseChatEvent(string strUser)
        {
            if (dicChatWins.Keys.Contains(strUser))
            {
                TabItem tab = dicChatWins[strUser];
                tabChatWins.Items.Remove(tab);
                dicChatWins.Remove(strUser);
            }
        }

        /// <summary>
        /// 接收到联系人发来的消息
        /// </summary> 
        /// <param name="strMsg"></param>
        /// <param name="strSender">消息发送人</param>
        void IService16001Callback.SendChatMsg(string strMsg, string strSender)
        {
            List<ContacterInListBox> cons = lstContacters.Where(p => p.UserName == strSender).ToList();
            if (cons.Count <= 0)
            {
               // MessageBox.Show(strSender + " not my contacter");
                return;
            }
            //如果窗口已经打开 则直接显示消息 并修改header的颜色
            if (dicChatWins.Keys.Contains(strSender))
            {
                TabItem item = dicChatWins[strSender];
                if (item.IsSelected == false)
                {
                    item.Foreground = Brushes.Orange;
                }
                ChartWindow chartWin = item.Content as ChartWindow;
                chartWin.txtRecord.Text += strMsg;
            }
            else
            {
                //如果没有打开窗口 则先把消息保存进dicUnReadMsg 在打开窗口时检查是否有该联系人的消息 如果有 就显示进聊天窗口
                UnReadMsg unRead = new UnReadMsg(strSender,strMsg);
                lstUnReadMsg.Add(unRead);

                #region 闪烁
                //开启线程 修改头像的透明度  无论是否打开了窗口 头像都要闪动 直到tab获得焦点为止  但如果窗口本身就是获得焦点的 就不用闪烁
                ContacterInListBox conSender = null;
                ContacterInListBox con = null;
                for (int i = 0; i < lbContacter.Items.Count; i++)
                {
                    con = lbContacter.Items[i] as ContacterInListBox;
                    if (con.UserName == strSender)
                    {
                        conSender = con;
                        break;
                    }
                }
                if (conSender == null)
                {
                    return;
                }
                if (!dicImgThread.Keys.Contains(strSender))
                {
                    Timer imgTimer = new Timer();
                    imgTimer.Interval = 200;
                    imgTimer.AutoReset = true;
                    imgTimer.Elapsed += (s, e) =>
                    {
                        if (con.IMGOpacity == "1")
                        {
                            Dispatcher.Invoke(new Action(() => con.IMGOpacity = "0.1"));
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() => con.IMGOpacity = "1"));
                        }
                    };
                    dicImgThread.Add(strSender, imgTimer);
                    imgTimer.Start();
                }
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// 将消息写入聊天窗口
        /// </summary>
        private void WriteUnReadMsgToWin(string strFriendName, ChartWindow chatWin)
        {
            List<UnReadMsg> lstMsg = lstUnReadMsg.Where(p => p.Sender == strFriendName).ToList();
            if (lstMsg.Count > 0)
            {
                for (int i = 0; i < lstMsg.Count; i++)
                {
                    chatWin.txtRecord.Text += lstMsg[i].Msg;
                    lstUnReadMsg.Remove(lstMsg[i]);
                }
            }
        }
    }
}
