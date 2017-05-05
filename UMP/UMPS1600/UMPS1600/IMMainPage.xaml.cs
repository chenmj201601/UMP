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
using VoiceCyber.UMP.Communications;
using UMPS1600.Service16002;

namespace UMPS1600
{
    /// <summary>
    /// IMMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class IMMainPage : IService16001Callback
    {
        private SessionInfo session;
        /// <summary>
        /// 用户登录状态变更事件
        /// </summary>
        /// <param name="msgType">事件类型 </param>
        /// <param name="strMsg"></param>
        public event Action<int, string> StatusChangeEvent;
        IService16001Channel iClient = null;
        ObservableCollection<ContacterInListBox> lstContacters = new ObservableCollection<ContacterInListBox>();

        //心跳计时器
        Timer HeartTimer = null;

        //当前已经打开的聊天窗口  好友名 和TabItem
        Dictionary<string, TabItem> dicChatWins = new Dictionary<string, TabItem>();
        Dictionary<long, CookieEntity> dicCookies = new Dictionary<long, CookieEntity>();

        //打开的头像闪动的线程
        Dictionary<long, Timer> dicImgThread = new Dictionary<long, Timer>();

        //保存收到的未读消息
        List<ChatMessage> lstUnReadMsg = new List<ChatMessage>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_session"></param>
        /// <param name="_lstParams">
        /// </param>
        public IMMainPage(SessionInfo _session)
        {
            InitializeComponent();
            try
            {
                session = _session;
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
            InitLanguage();
        }

        private void InitLanguage()
        {
            lblContact.Text = InitApp.GetLanguage("1600006", "My contact");
        }

        #region 公开给UMP或智能客户端调用的函数
        /// <summary>
        /// 设置资源（会话从一个资源 切换到另一个资源 使得本次聊天的内容与该资源关联）
        /// </summary>
        /// <param name="strContacterName">联系人的名字</param>
        /// <param name="lContacterID">联系人的ID</param>
        /// <param name="lResourceID">要关联的资源ID</param>
        public OperationReturn SetResource(string strContacterName, long lContacterID, long lResourceID)
        {
            OperationReturn optReturn = new OperationReturn();
            List<ContacterInListBox> lstContacterByID = lstContacters.Where(p => p.UserID == lContacterID).ToList();
            if (lstContacterByID.Count <= 0)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S1600WcfError.NotInContacterList;
                return optReturn;
            }
            ContacterInListBox con = lstContacterByID.First();
            CookieEntity currCookie = null;
            //判断是否有未读消息 有未读消息 说明窗口没有打开 将创建新的cookie信息和聊天窗口
            List<ChatMessage> lstUnReadMsgByID = lstUnReadMsg.Where(p => p.SenderID == lContacterID).ToList();
            if (lstUnReadMsgByID.Count > 0)
            {
                currCookie = new CookieEntity();
                //有未读消息 判断对方是否在线 如果在线 再判断最后一条消息所属的会话是否关闭 
                //如果关闭 则之前的会话视为无效 新会话的资源ID为新传入的ID， 反之 则接收最后一条消息的cookieID和ResourceID作为本次会话的cookieID和ResourceID
                if (con.Status == "0")  //离线
                {
                    currCookie.ResourceID = lResourceID;
                    currCookie.CookieCreated = 0;
                    currCookie.CookieID = 0;
                }
                else
                {
                    //判断最后一条消息的会话是否关闭 
                    long lLastResID = long.Parse(lstUnReadMsgByID.Last().ResourceID.ToString());
                    string strLastCookieID = lstUnReadMsgByID.Last().CookieID.ToString();
                    bool boCookieEffective = GetCookieStatusByID(strLastCookieID);
                    currCookie.ResourceID = boCookieEffective ? lLastResID : lResourceID;
                    currCookie.CookieCreated = boCookieEffective ? 2 : 0;
                    currCookie.CookieID = boCookieEffective ? long.Parse(strLastCookieID) : 0;
                }
                currCookie.UserID = lContacterID;
                currCookie.UserName = strContacterName;

                //创建聊天窗口
                currCookie.ChatTab = CreateChatWindow(con);
                (currCookie.ChatTab.Content as ChartWindow).cookie = currCookie;
                dicCookies.Add(lContacterID, currCookie);
                //将消息写入聊天窗口 并让头像停止闪烁
                WriteUnReadMsgToWin(lContacterID, currCookie.ChatTab.Content as ChartWindow);
                dicImgThread[con.UserID].Stop();
                dicImgThread[con.UserID].Dispose();
                dicImgThread[con.UserID].Close();
                dicImgThread.Remove(con.UserID);
                con.IMGOpacity = "1";

                if (currCookie.CookieID == 0)
                {
                    //如果会话ID是0 证明前一次会话已经结束 可能会开启一次新的会话 所以需要给timer赋值
                    currCookie.CookieTimer = CreateTimer(currCookie, lContacterID);
                }

                optReturn.Result = true;
                return optReturn;
            }
            //如果没有未读消息 判断窗口是否被打开了
            List<string> lstParamsForEndCookie = new List<string>();
            if (dicCookies.Keys.Contains(lContacterID))
            {
                //如果聊天窗口已经打开了 需要结束上一个会话 资源ID改为传入的ID 如果timer不为null 需要停掉timer重新开始计时
                currCookie = dicCookies[lContacterID];
                lstParamsForEndCookie.Clear();
                lstParamsForEndCookie.Add(currCookie.CookieID.ToString());
                lstParamsForEndCookie.Add(session.UserID.ToString());
                lstParamsForEndCookie.Add(lContacterID.ToString());
                if (currCookie.CookieID != 0)
                {
                    iClient.EndCookieByID(lstParamsForEndCookie);
                }
                dicCookies[lContacterID].CookieID = 0;
                dicCookies[lContacterID].CookieCreated = 0;
                dicCookies[lContacterID].ResourceID = 0;
                dicCookies[lContacterID].CookieTimer = null;
                //dicCookies.Remove(lContacterID);
            }
            else
            {
                currCookie = new CookieEntity();
                currCookie.ChatTab = CreateChatWindow(con);
                currCookie.UserID = lContacterID;
                currCookie.UserName = strContacterName;
            }
            currCookie.CookieID = 0;
            currCookie.CookieCreated = 0;
            if (currCookie.CookieTimer != null)
            {
                currCookie.CookieTimer.Stop();
            }
            else
            {

                currCookie.CookieTimer = CreateTimer(currCookie, lContacterID);
            }

            currCookie.ResourceID = lResourceID;
            currCookie.ChatTab.IsSelected = true;
            (currCookie.ChatTab.Content as ChartWindow).cookie = currCookie;
            if (dicCookies.Keys.Contains(lContacterID))
            {
                dicCookies.Remove(lContacterID);
            }
            dicCookies.Add(lContacterID, currCookie);
            return optReturn;
        }

        private Timer CreateTimer(CookieEntity cookie, long lContacterID)
        {
            Timer timer = new Timer();
            timer.Interval = 1 * 60 * 1000;
            timer.Elapsed += (s, e) =>
            {
                //20分钟后 关闭此次会话 
                List<string> lstParamsForEndCookie = new List<string>();
                lstParamsForEndCookie.Add(cookie.CookieID.ToString());
                lstParamsForEndCookie.Add(session.UserID.ToString());
                lstParamsForEndCookie.Add(lContacterID.ToString());
                try
                {
                    iClient.EndCookieByID(lstParamsForEndCookie);
                }
                catch
                { }
                timer.Stop();
                cookie.ResourceID = 0;
                cookie.CookieID = 0;
                cookie.CookieCreated = 0;
            };
            return timer;
        }
        #endregion

        #region 主窗口自己的函数(被调用的函数)
        /// <summary>
        /// 创建cookie对象和一个聊天窗口
        /// </summary>
        private TabItem CreateChatWindow(ContacterInListBox con)
        {
            TabItem tab = new TabItem();
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
            ChartWindow chatWin = new ChartWindow();
            chatWin.con = lstContacters.Where(p => p.UserID == con.UserID).ToList().First();
            chatWin.CloseChatEvent += chartWin_CloseChatEvent;
            chatWin.SendChatMsgEvent += chartWin_SendChatMsgEvent;
            tab.Content = chatWin;
            tabChatWins.Items.Add(tab);
            tab.Tag = con.UserID;
            tab.IsSelected = true;
            return tab;
        }

        /// <summary>
        /// 获得会话状态 
        /// </summary>
        /// <param name="cookieID"></param>
        /// <returns>true: 会话未结束 有效 false: 会话已结束 无效</returns>
        private bool GetCookieStatusByID(string cookieID)
        {
            bool bReturn = false;
            Service16002Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)S1600RequestCode.GetCookieStatus;
                webRequest.ListData.Add(cookieID);
                client = new Service16002Client(WebHelper.CreateBasicHttpBinding(session),
                     WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service16002"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    bReturn = false;
                    return bReturn;
                }
                if (webReturn.Data == "1")
                {
                    bReturn = true;
                }
                else
                {
                    bReturn = false;
                }
            }
            catch
            {
                bReturn = false;
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
            return bReturn;
        }

        /// <summary>
        /// 根据用户在线状态 设置IM是否可用
        /// </summary>
        /// <param name="bStatus">在线 ：true  离线：false</param>
        private void SetIMStatus(bool bStatus)
        {
            lbContacter.IsEnabled = bStatus;
            if (tabChatWins.Items.Count > 0)
            {
                ChartWindow chatWin = null;
                foreach (TabItem item in tabChatWins.Items)
                {
                    chatWin = item.Content as ChartWindow;
                    chatWin.IsEnabled = bStatus;
                }
            }
        }

        #endregion

        #region 本窗口自己的控件事件
        void tabChatWins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabChatWins.SelectedItem != null)
            {
                TabItem tab = tabChatWins.SelectedItem as TabItem;
                //long lContacterID = long.Parse(tab.Tag.ToString());
                //if (dicImgThread.Keys.Contains(lContacterID))
                //{
                //    ChartWindow chartWin = tab.Content as ChartWindow;
                //    WriteUnReadMsgToWin(lContacterID, chartWin);
                tab.Foreground = Brushes.Black;
                //}
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
                DuplexChannelFactory<IService16001Channel> fac = new DuplexChannelFactory<IService16001Channel>(this, binding, myEndpoint);
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
            try
            {
                iClient.LogOff(session);
                if (StatusChangeEvent != null)
                {
                    StatusChangeEvent((int)UserStatusChangeType.LogOff, "");
                }

                System.Threading.Thread.Sleep(1000);
                if (iClient.State == CommunicationState.Opened)
                {
                    iClient.Close();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 给好友发送消息
        /// </summary>
        /// <param name="lFriendID">好友ID</param>
        /// <param name="strMsg">发送的消息</param>
        void chartWin_SendChatMsgEvent(ChatMessage chatMsg)
        {
            //Client.SendChatMessage(session.UserID, lFriendID, strMsg);
            bool boCookieCreated = false;
            if (chatMsg.CookieID != 0)
            {
                boCookieCreated = false;
            }
            else
            {
                boCookieCreated = true;
            }
            try
            {
                iClient.SendChatMessage(chatMsg, boCookieCreated);
                if (dicCookies[chatMsg.ReceiverID].CookieTimer != null)
                {
                    //把会话的timer时间重置 
                    dicCookies[chatMsg.ReceiverID].CookieTimer.Stop();
                    dicCookies[chatMsg.ReceiverID].CookieTimer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //如果会话没有开始 就开启线程 并更改会话的属性(由回调函数SendSysMessage messagetype=CookieID设置)
        }
        #endregion


        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HeartTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (iClient.State == CommunicationState.Opened)
                {
                    iClient.SendHeartMsg();
                }
            }
            catch (Exception ex)
            {

                //MessageBox.Show("");
            }
        }


        #region WCF回调函数
        void IService16001Callback.SendSysMessage(S1600MessageType msgType, List<string> lstSysMsg)
        {
            string strMsg = string.Empty;
            long lUserID = 0;

            switch (msgType)
            {
                case S1600MessageType.ConnServerFailed:

                    break;
                case S1600MessageType.FriendOffline:
                    //好友上线 更新名字颜色和图标
                    long.TryParse(lstSysMsg[0], out lUserID);
                    List<ContacterInListBox> lstFriend = lstContacters.Where(p => p.UserID == lUserID).ToList();
                    if (lstFriend.Count > 0)
                    {
                        lstFriend.First().Status = "1";
                        lstFriend.First().ForegGround = Brushes.Gray;
                        string strType = lstFriend.First().UserID.ToString().Substring(0, 3);
                        lstFriend.First().Icon = strType == ConstValue.RESOURCE_AGENT.ToString() ?
                            "Themes/Default/UMP1600/Images/AgentOffline.png" : "Themes/Default/UMP1600/Images/UserOffline.png";
                    }
                    ListSorter.SortList<ObservableCollection<ContacterInListBox>, ContacterInListBox>(ref lstContacters, "Status", SortDirection.Descending);
                    break;
                case S1600MessageType.FriendOnline:
                    //好友上线 更新名字颜色和图标
                    long.TryParse(lstSysMsg[0], out lUserID);
                    lstFriend = lstContacters.Where(p => p.UserID == lUserID).ToList();
                    if (lstFriend.Count > 0)
                    {
                        lstFriend.First().Status = "1";
                        lstFriend.First().ForegGround = Brushes.Red;
                        string strType = lstFriend.First().UserID.ToString().Substring(0, 3);
                        lstFriend.First().Icon = strType == ConstValue.RESOURCE_AGENT.ToString() ?
                            "Themes/Default/UMP1600/Images/AgentOnline.png" : "Themes/Default/UMP1600/Images/UserOnline.png";
                        //lstFriend.First().Icon = "Themes/Default/UMP1600/Images/OnLine.ico";
                    }
                    ListSorter.SortList<ObservableCollection<ContacterInListBox>, ContacterInListBox>(ref lstContacters, "Status", SortDirection.Descending);
                    break;
                case S1600MessageType.GetFriendFailed:
                    MessageBox.Show(InitApp.GetLanguage("1600007", "Failed to get a buddy list"));
                    break;
                case S1600MessageType.Logined:
                    //MessageBox.Show(InitApp.GetLanguage("1600001", "Your account has been logged in at another place, you have been forced offline"));

                    //lblCurrStatus.Content = session.UserInfo.UserName +"("+ InitApp.GetLanguage("1600009", "Offline")+")";
                    //lblCurrStatus.Foreground = Brushes.LightGray;

                    StatusChangeEvent((int)UserStatusChangeType.LogOff, "");
                    SetIMStatus(false);
                    if (iClient.State == CommunicationState.Opened)
                    {
                        try
                        {
                            iClient.Close();
                        }
                        catch
                        {
                        }
                    }

                    break;
                case S1600MessageType.LoginFailed:
                    MessageBox.Show(lstSysMsg[0]);
                    break;
                case S1600MessageType.LoginSuceess:
                    //lblCurrStatus.Content = session.UserInfo.UserName +"("+ InitApp.GetLanguage("1600009", "Offline")+")";
                    //lblCurrStatus.Foreground = Brushes.LightGreen;
                    StatusChangeEvent((int)UserStatusChangeType.Login, "");

                    #region 发送心跳的计时器
                    HeartTimer = new Timer();
                    HeartTimer.AutoReset = true;
                    HeartTimer.Interval = 180 * 1000;        //时间间隔30秒
                    HeartTimer.Elapsed += HeartTimer_Elapsed;
                    HeartTimer.Start();
                    #endregion
                    if (StatusChangeEvent != null)
                    {
                        StatusChangeEvent((int)UserStatusChangeType.Login, "");
                    }
                    break;
                case S1600MessageType.HeartMsg:
                    //MessageBox.Show("心跳消息"+DateTime.Now);
                    break;
                case S1600MessageType.TestMsg:
                    //MessageBox.Show(lstSysMsg[0]);
                    break;
                case S1600MessageType.SendMsgError:
                    strMsg = InitApp.GetLanguage("1600002", "Your message '{0}' failed to send the other party may have been offline, has been sent as an offline message");
                    strMsg = string.Format(strMsg, lstSysMsg[0]);
                    //找到对应的窗口 显示此消息 
                    string strFriendName = lstSysMsg[1];
                    if (dicChatWins.Keys.Contains(strFriendName))
                    {
                        ChartWindow win = dicChatWins[strFriendName].Content as ChartWindow;
                        win.txtRecord.Text += strMsg;
                    }
                    else
                    {
                        //还没想好该咋处理 先空着
                    }
                    break;
                case S1600MessageType.EndCookie:
                    //结束会话
                    string strContacterName = lstSysMsg[0];
                    long lContacterID = long.Parse(lstSysMsg[1]);
                    if (dicCookies.Keys.Contains(lContacterID))
                    {
                        if (dicCookies[lContacterID].CookieTimer != null)
                        {
                            dicCookies[lContacterID].CookieTimer.Stop();
                            dicCookies[lContacterID].CookieTimer = null;
                            //MessageBox.Show("cookie closed");
                        }
                    }
                    break;
                case S1600MessageType.CookieID:
                    lContacterID = long.Parse(lstSysMsg[0]);
                    long cookieID = long.Parse(lstSysMsg[1]);
                    if (dicCookies.Keys.Contains(lContacterID))
                    {
                        dicCookies[lContacterID].CookieID = cookieID;
                        dicCookies[lContacterID].CookieCreated = 1;
                        dicCookies[lContacterID].CookieTimer.Start();
                    }
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
                    List<ContacterInListBox> lstExists = lstContacters.Where(p => p.UserID == con.UserID).ToList();
                    if (lstExists.Count > 0)
                    {
                        continue;
                    }
                    conInList.UserID = con.UserID;
                    conInList.UserName = S16001EncryptOperation.DecryptWithM004(con.UserName);
                    conInList.FullName = S16001EncryptOperation.DecryptWithM004(con.UserName) + " (" + S16001EncryptOperation.DecryptWithM004(con.FullName) + ")";
                    conInList.OrgID = con.OrgID;
                    conInList.OrgName = S16001EncryptOperation.DecryptWithM004(con.OrgName);
                    conInList.ParentOrgID = con.ParentOrgID;
                    conInList.Status = con.Status;
                    conInList.IMGOpacity = "1";
                    string strType = conInList.UserID.ToString().Substring(0, 3);
                    if (strType == ConstValue.RESOURCE_AGENT.ToString())
                    {
                        if (conInList.Status == "1")
                        {
                            conInList.Icon = "Themes/Default/UMP1600/Images/AgentOnline.png";
                        }
                        else
                        {
                            conInList.Icon = "Themes/Default/UMP1600/Images/AgentOffline.png";
                        }
                    }
                    else
                    {
                        if (conInList.Status == "1")
                        {
                            conInList.Icon = "Themes/Default/UMP1600/Images/UserOnline.png";
                        }
                        else
                        {
                            conInList.Icon = "Themes/Default/UMP1600/Images/UserOffline.png";
                        }
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
            ListSorter.SortList<ObservableCollection<ContacterInListBox>, ContacterInListBox>(ref lstContacters, "Status", SortDirection.Descending);
        }

        /// <summary>
        /// 双击新建一个好友聊天窗口 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ContacterInListBox con = lbContacter.SelectedValue as ContacterInListBox;
            SetResource(con.UserName, con.UserID, 0);
        }

        /// <summary>
        /// 关闭聊天窗口
        /// </summary>
        /// <param name="strUser"></param>
        void chartWin_CloseChatEvent(long lUserID)
        {
            if (dicCookies.Keys.Contains(lUserID))
            {
                if (dicCookies[lUserID].CookieID != 0)
                {
                    //关闭会话
                    List<string> lstParams = new List<string>();
                    lstParams.Add(dicCookies[lUserID].CookieID.ToString());
                    lstParams.Add(session.UserID.ToString());
                    lstParams.Add(lUserID.ToString());

                    try
                    {
                        iClient.EndCookieByID(lstParams);
                    }
                    catch
                    {
                        //可能通道出现问题 以后处理
                    }
                }

                TabItem tab = dicCookies[lUserID].ChatTab;
                tabChatWins.Items.Remove(tab);
                dicCookies.Remove(lUserID);
            }
        }

        /// <summary>
        /// 接收到联系人发来的消息
        /// </summary>
        /// <param name="mstObj"></param>
        void IService16001Callback.ReceiveChatMsg(ChatMessage msgObj)
        {
            List<ContacterInListBox> cons = lstContacters.Where(p => p.UserID == msgObj.SenderID).ToList();
            if (cons.Count <= 0)
            {
                // MessageBox.Show(strSender + " not my contacter");
                return;
            }
            //如果窗口已经打开 则直接显示消息 并修改header的颜色
            if (dicCookies.Keys.Contains(msgObj.SenderID))
            {
                TabItem item = dicCookies[msgObj.SenderID].ChatTab;
                if (item.IsSelected == false)
                {
                    item.Foreground = Brushes.Orange;
                }
                ChartWindow chartWin = item.Content as ChartWindow;
                chartWin.txtRecord.Text += msgObj.MsgContent;
                if (dicCookies[msgObj.SenderID].CookieTimer != null)
                {
                    //把会话的timer时间重置 
                    dicCookies[msgObj.SenderID].CookieTimer.Stop();
                    dicCookies[msgObj.SenderID].CookieTimer.Start();
                }
            }
            else
            {
                //如果没有打开窗口 则先把消息保存进dicUnReadMsg 在打开窗口时检查是否有该联系人的消息 如果有 就显示进聊天窗口
                //UnReadMsg unRead = new UnReadMsg(strSender, strMsg);
                //lstUnReadMsg.Add(unRead);
                lstUnReadMsg.Add(msgObj);
                StatusChangeEvent((int)UserStatusChangeType.NewMsg, lstUnReadMsg.Count.ToString());
                #region 闪烁
                //开启线程 修改头像的透明度  无论是否打开了窗口 头像都要闪动 直到tab获得焦点为止  但如果窗口本身就是获得焦点的 就不用闪烁
                ContacterInListBox conSender = null;
                ContacterInListBox con = null;
                for (int i = 0; i < lbContacter.Items.Count; i++)
                {
                    con = lbContacter.Items[i] as ContacterInListBox;
                    if (con.UserID == msgObj.SenderID)
                    {
                        conSender = con;
                        break;
                    }
                }
                if (conSender == null)
                {
                    return;
                }
                if (!dicImgThread.Keys.Contains(msgObj.SenderID))
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
                    dicImgThread.Add(msgObj.SenderID, imgTimer);
                    imgTimer.Start();
                }
                #endregion

            }
        }
        #endregion

        /// <summary>
        /// 将消息写入聊天窗口
        /// </summary>
        private void WriteUnReadMsgToWin(long lContacterID, ChartWindow chatWin)
        {
            List<ChatMessage> lstMsg = lstUnReadMsg.Where(p => p.SenderID == lContacterID).ToList();
            if (lstMsg.Count > 0)
            {
                for (int i = 0; i < lstMsg.Count; i++)
                {
                    chatWin.txtRecord.Text += lstMsg[i].MsgContent;
                    lstUnReadMsg.Remove(lstMsg[i]);
                }
                //消息被读取后，告知客户端消息数量变更，参数1：消息类型 参数2：剩下的未读消息数
                StatusChangeEvent((int)UserStatusChangeType.NewMsg, lstUnReadMsg.Count.ToString());
            }
        }
    }
}
