//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    193fb0e7-54c7-4f75-800b-cdaf2b5e4d37
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPMainPage
//
//        created by Charley at 2015/5/5 14:38:18
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS1600;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// MainPage的基本实现，UMP中主页面均可派生自此类型从而继承一下基本的操作
    /// 1、NetPipe消息处理
    /// 2、空闲时间检查
    /// 3、PageHead消息处理
    /// 4、PopupPanel操作
    /// 5、Waiter操作
    /// 6、取消按钮
    /// </summary>
    public class UMPMainPage : UMPPage
    {

        #region Members

        private int mIdleCheckCount;
        private int mIdleCheckInterval;
        private Timer mIdleCheckTimer;
        private IMMainPage mIMMainPage;
        private int mMsgCount;

        #endregion


        #region PageHeadType

        public static readonly DependencyProperty PageHeadTypeProperty =
            DependencyProperty.Register("PageHeadType", typeof(PageHeadType), typeof(UMPMainPage), new PropertyMetadata(PageHeadType.Default));

        public PageHeadType PageHeadType
        {
            get { return (PageHeadType)GetValue(PageHeadTypeProperty); }
            set { SetValue(PageHeadTypeProperty, value); }
        }

        #endregion


        static UMPMainPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UMPMainPage),
                new FrameworkPropertyMetadata(typeof(UMPMainPage)));
        }

        public UMPMainPage()
        {
            Loaded += UMPMainPage_Loaded;
            Unloaded += UMPMainPage_Unloaded;
            UMPApp.NetPipeEvent += App_NetPipeEvent;

            mMsgCount = 0;

        }

        private void UMPMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        void UMPMainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mIMMainPage != null)
            {
                try
                {
                    mIMMainPage.LogOff();
                }
                catch { }
                mIMMainPage = null;
            }
        }


        #region Init and Load

        protected virtual void Init()
        {
            try
            {
                mIdleCheckCount = 0;
                mIdleCheckInterval = 1000;
                mIdleCheckTimer = new Timer(mIdleCheckInterval);
                mIdleCheckTimer.Elapsed += mIdleCheckTimer_Elapsed;
                mIdleCheckTimer.Start();

                ThemeInfo = UMPApp.Session.ThemeInfo;
                LangTypeInfo = UMPApp.Session.LangTypeInfo;
                AppServerInfo = UMPApp.Session.AppServerInfo;

                if (PageHead != null)
                {
                    PageHead.SessionInfo = UMPApp.Session;
                    PageHead.InitInfo();
                    SetIMMsgState();
                }

                CreateIMPanel();

                ChangeTheme();
                ChangeLanguage();

                ShowPage();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Template

        private const string PART_GridMain = "PART_GridMain";
        private const string PART_MainPageBg = "PART_MainPageBg";
        private const string PART_PageHead = "PART_PageHead";
        private const string PART_MyWaiter = "PART_MyWaiter";
        private const string PART_StatusContent = "PART_StatusContent";
        private const string PART_BtnCancel = "PART_BtnCancel";
        private const string PART_MASK = "PART_Mask";
        private const string PART_IMPanel = "PART_IMPanel";

        protected Grid GridMain;
        protected Border BorderMainPageBg;
        protected UMPPageHead PageHead;
        protected CustomWaiter MyWaiter;
        protected TextBlock StatusContent;
        protected Button ButtonCancel;
        protected Border BorderMask;
        protected PopupPanel IMPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GridMain = GetTemplateChild(PART_GridMain) as Grid;
            if (GridMain != null)
            {
                GridMain.MouseMove += GridMain_MouseMove;
                GridMain.KeyDown += GridMain_KeyDown;
            }
            BorderMainPageBg = GetTemplateChild(PART_MainPageBg) as Border;
            if (BorderMainPageBg != null)
            {

            }
            PageHead = GetTemplateChild(PART_PageHead) as UMPPageHead;
            if (PageHead != null)
            {
                PageHead.PageHeadEvent += PageHead_PageHeadEvent;
                PageHead.SessionInfo = UMPApp.Session;
                PageHead.InitInfo();
            }
            MyWaiter = GetTemplateChild(PART_MyWaiter) as CustomWaiter;
            if (MyWaiter != null)
            {

            }
            StatusContent = GetTemplateChild(PART_StatusContent) as TextBlock;
            if (StatusContent != null)
            {

            }
            ButtonCancel = GetTemplateChild(PART_BtnCancel) as Button;
            if (ButtonCancel != null)
            {
                ButtonCancel.Click += ButtonCancel_Click;
            }

            BorderMask = GetTemplateChild(PART_MASK) as Border;
            if (BorderMask != null)
            {

            }

            IMPanel = GetTemplateChild(PART_IMPanel) as PopupPanel;
            if (IMPanel != null)
            {

            }
        }

        #endregion


        #region Event Handlers

        protected virtual void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            switch (e.Code)
            {
                case 120:
                    SendChangePasswordMessage();
                    break;
                case 201:
                    SendLogoutMessage();
                    break;
                case 202:
                    Close();
                    SendNavigateHomeMessage();
                    break;
                case PageHeadEventArgs.EVT_DEFAULT_PAGE:
                    SetDefaultPage();
                    break;
                case PageHeadEventArgs.EVT_OPENIM:
                    OpenIMPanel();
                    break;
            }
        }

        protected virtual void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //在派生类中处理取消按钮的取消操作

        }

        protected virtual void IMMainPage_StatusChangeEvent(int msgType, string strMsg)
        {
            UMPApp.WriteLog("IMMessage", string.Format("{0}\t{1}", msgType, strMsg));
            switch (msgType)
            {
                case IM_STATUS_LOGIN:

                    break;
                case IM_STATUS_LOGOFF:

                    break;
                case IM_STATUS_NEWMSG:
                    mMsgCount++;
                    SetIMMsgState();
                    break;
            }
        }

        //IM Chart 事件代码
        private const int IM_STATUS_LOGIN = 1;
        private const int IM_STATUS_LOGOFF = 2;
        private const int IM_STATUS_NEWMSG = 3;

        #endregion


        #region Public Method

        /// <summary>
        /// 在页面关闭的时候执行的操作，主要用来做一些清理操作
        /// </summary>
        public virtual void Close()
        {
            //关闭IM面板
            if (mIMMainPage != null)
            {
                try
                {
                    mIMMainPage.LogOff();
                }
                catch { }
                mIMMainPage = null;
            }
        }

        #endregion


        #region Others

        public void SetStatuMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (StatusContent != null)
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        StatusContent.Text = string.Empty;
                    }
                    else
                    {
                        StatusContent.Text = msg;
                    }
                }
            }));
        }

        protected virtual void SetDefaultPage()
        {
            SetDefaultPage(UMPApp.ModuleID);
        }

        protected void SetDefaultPage(int moduleID)
        {
            try
            {
                var session = UMPApp.Session;
                if (session == null) { return; }
                string strUserID = session.UserID.ToString();
                string strModule = moduleID.ToString();
                string strParamValue = string.Format("{0}{1}{2}", strUserID, ConstValue.SPLITER_CHAR_3, strModule);
                UserParamInfo param = new UserParamInfo();
                param.UserID = session.UserID;
                param.ParamID = ConstValue.UP_DEFAULT_PAGE;
                param.GroupID = ConstValue.UP_GROUP_PAGE;
                param.SortID = 0;
                param.DataType = DBDataType.NVarchar;
                param.ParamValue = strParamValue;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(param);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSSaveUserParamList;
                webRequest.Session = session;
                webRequest.ListData.Add(session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(session)
                    , WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                UMPApp.ShowInfoMessage(string.Format("{0}\r\n\r\n{1}",
                    UMPApp.GetLanguageInfo("COMN006", "Set default page end"),
                    UMPApp.GetLanguageInfo(string.Format("FO{0}", moduleID), moduleID.ToString())));
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void OpenIMPanel()
        {
            try
            {
                var session = UMPApp.Session;
                if (session == null) { return; }
                if (mIMMainPage == null)
                {
                    CreateIMPanel();
                }

                if (IMPanel == null) { return; }
                IMPanel.Title = string.Format("{0}", session.UserInfo.Account);
                IMPanel.Width = 800;
                IMPanel.Height = 550;
                IMPanel.IsModal = false;
                IMPanel.Content = mIMMainPage;
                IMPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateIMPanel()
        {
            try
            {
                var session = UMPApp.Session;
                if (session == null) { return; }

                List<string> listMsgs = new List<string>();
                mIMMainPage = new IMMainPage(session);
                mIMMainPage.StatusChangeEvent += IMMainPage_StatusChangeEvent;
                mIMMainPage.Width = 780;
                mIMMainPage.Height = 500;
                mIMMainPage.Login();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void SetIMMsgState()
        {
            try
            {
                var pageHead = PageHead;
                if (pageHead == null) { return; }

                if (mMsgCount > 0)
                {
                    pageHead.IsMsgCountVisible = true;
                    if (mMsgCount > 9)
                    {
                        pageHead.MsgCount = "9+";
                    }
                    else
                    {
                        pageHead.MsgCount = mMsgCount.ToString();
                    }
                }
                else
                {
                    pageHead.IsMsgCountVisible = false;
                    pageHead.MsgCount = mMsgCount.ToString();
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        public void ShowPage()
        {
            if (BorderMask != null)
            {
                BorderMask.Visibility = Visibility.Collapsed;
            }
        }

        #endregion


        #region NetPipe

        protected virtual void App_NetPipeEvent(WebRequest webRequest)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    switch (code)
                    {
                        case (int)RequestCode.SCIdleCheckStop:
                            StartStopIdleTimer(false);
                            break;
                        case (int)RequestCode.SCIdleCheckStart:
                            StartStopIdleTimer(true);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UMPApp.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        protected void SendLoadedMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSModuleLoaded;
            request.Session = UMPApp.Session;
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendThemeChangeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSThemeChange;
            request.Session = UMPApp.Session;
            request.Data = ThemeInfo.Name;
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendLanguageChangeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSLanguageChange;
            request.Session = UMPApp.Session;
            request.Data = LangTypeInfo.LangID.ToString();
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendLogoutMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSLogout;
            request.Session = UMPApp.Session;
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendChangePasswordMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSChangePassword;
            request.Session = UMPApp.Session;
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendNavigateHomeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSHome;
            request.Session = UMPApp.Session;
            UMPApp.SendNetPipeMessage(request);
        }

        protected void SendIdleCheckMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSIdleCheck;
            request.Session = UMPApp.Session;
            request.Data = mIdleCheckCount.ToString();
            UMPApp.SendNetPipeMessage(request);
        }

        #endregion


        #region IdleCheck 相关

        void mIdleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                mIdleCheckCount++;
                SendIdleCheckMessage();
            }
            catch { }
        }

        void GridMain_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                mIdleCheckCount = 0;
            }
            catch { }
        }

        void GridMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                mIdleCheckCount = 0;
            }
            catch { }
        }

        private void StartStopIdleTimer(bool isStart)
        {
            if (isStart)
            {
                mIdleCheckCount = 0;
                if (mIdleCheckTimer != null)
                {
                    mIdleCheckTimer.Start();
                }
            }
            else
            {
                if (mIdleCheckTimer != null)
                {
                    mIdleCheckTimer.Stop();
                }
                mIdleCheckCount = 0;
            }
        }

        #endregion


        #region ChangeTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , "CommonControls/UMPCommon.xaml");
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPControls;component/Themes/{0}/{1}",
                        "Default"
                        , "CommonControls/UMPCommon.xaml");
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPControls;component/Themes/{0}/{1}",
                        "Default"
                        , "CommonControls/UMPMainPage.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }

            if (PageHead != null)
            {
                PageHead.ChangeTheme();
                PageHead.InitInfo();
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            if (PageHead != null)
            {
                PageHead.ChangeLanguage();
            }
        }

        #endregion

    }
}
