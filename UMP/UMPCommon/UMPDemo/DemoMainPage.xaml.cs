//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    79b9079e-0861-42fb-a4cd-658cd6fa753f
//        CLR Version:              4.0.30319.18444
//        Name:                     DemoMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPDemo
//        File Name:                DemoMainPage
//
//        created by Charley at 2014/11/27 14:23:46
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPDemo
{
    /// <summary>
    /// DemoMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class DemoMainPage
    {

        #region Memebers

        private BackgroundWorker mWorker;
        private ObservableCollection<OperationInfo> mListBasicOperations;

        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        #endregion


        public DemoMainPage()
        {
            InitializeComponent();

            Unloaded += DemoMainPage_Unloaded;
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            TreeViewObjects.TreeObjectEvent += TreeViewObjects_TreeObjectEvent;
            MediaPlayer.PlayerEvent += MediaPlayer_PlayerEvent;
        }

        void DemoMainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            App.ShowExceptionMessage("Test");
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "DemoMainPage";
                StylePath = "UMPDemo/DemoMainPage.xaml";

                base.Init();

                ChangeTheme();
                if (MyWaiter != null)
                {
                    MyWaiter.Visibility = Visibility.Visible;
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    InitOperations();
                    InitBasicOperations();

                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    //if (MyWaiter != null)
                    //{
                    //    MyWaiter.Visibility = Visibility.Collapsed;
                    //}

                    //触发Loaded消息
                    SendLoadedMessage();

                    //CreateTreeObjects();
                    CreateOptButtons();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitOperations()
        {
            try
            {
                OperationInfo optInfo = new OperationInfo();
                optInfo.ID = 10001;
                optInfo.ParentID = 0;
                optInfo.Display = "Get Objects";
                ListOperations.Add(optInfo);
                optInfo = new OperationInfo();
                optInfo.ID = 10002;
                optInfo.ParentID = 0;
                optInfo.Display = "Get Checked Objects";
                ListOperations.Add(optInfo);
                optInfo = new OperationInfo();
                optInfo.ID = 10003;
                optInfo.ParentID = 0;
                optInfo.Display = "Play Media";
                ListOperations.Add(optInfo);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitBasicOperations()
        {
            List<OperationInfo> listOptInfos = new List<OperationInfo>();
            for (int i = 0; i < ListOperations.Count; i++)
            {
                listOptInfos.Add(ListOperations[i]);
            }
            listOptInfos = listOptInfos.OrderBy(o => o.SortID).ToList();
            mListBasicOperations.Clear();
            for (int i = 0; i < listOptInfos.Count; i++)
            {
                mListBasicOperations.Add(listOptInfos[i]);
            }
        }

        #endregion


        #region EventHandler

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        App.Session.ThemeInfo = themeInfo;
                        App.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        App.Session.LangTypeInfo = langType;
                        App.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        CustomWaiter.Visibility = Visibility.Visible;
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (s, de) => App.InitLanguageInfos();
                        mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Hidden;
                            CustomWaiter.Visibility = Visibility.Hidden;
                            ChangeLanguage();
                            PopupPanel.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }
                    break;
                //展开或关闭侧边栏
                case 121:
                    OpenCloseLeftPanel();
                    break;
            }
        }

        void TreeViewObjects_TreeObjectEvent(object sender, TreeObjectEventArgs e)
        {
            switch (e.Code)
            {
                case 100:
                    MyWaiter.Visibility = Visibility.Hidden;
                    break;
                case 999:
                    App.ShowExceptionMessage(e.Message);
                    break;
            }
        }

        void MediaPlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            var args = e.NewValue;
            if (args == null) { return; }
            App.WriteLog("Test", string.Format("Code:{0}\t{1}", args.Code, args.Data));
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case 10001:
                        GetObjects();
                        break;
                    case 10002:
                        GetCheckedObjects();
                        break;
                    case 10003:
                        PlayMedia();
                        break;
                }
            }
        }

        #endregion


        #region Others

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }

        private void CreateTreeObjects()
        {
            try
            {
                TreeViewObjects.SessionInfo = App.Session;
                TreeViewObjects.Init();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void GetObjects()
        {
            try
            {
                List<TreeObjectItem> listObjects = new List<TreeObjectItem>();
                TreeViewObjects.GetObjects(ref listObjects);
                for (int i = 0; i < listObjects.Count; i++)
                {
                    TreeObjectItem item = listObjects[i];
                    string strInfo;
                    string strType = item.ObjType.ToString();
                    string strID = item.ObjID.ToString();
                    string strName = item.Name;
                    strInfo = string.Format("Type:{0}\tID:{1}\tName:{2}", strType, strID, strName);
                    TxtMsg.AppendText(string.Format("{0}\r\n", strInfo));
                    TxtMsg.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetCheckedObjects()
        {
            try
            {
                List<TreeObjectItem> listObjects = new List<TreeObjectItem>();
                TreeViewObjects.GetCheckedObjects(ref listObjects);
                for (int i = 0; i < listObjects.Count; i++)
                {
                    TreeObjectItem item = listObjects[i];
                    string strInfo;
                    string strType = item.ObjType.ToString();
                    string strID = item.ObjID.ToString();
                    string strName = item.Name;
                    strInfo = string.Format("Type:{0}\tID:{1}\tName:{2}", strType, strID, strName);
                    TxtMsg.AppendText(string.Format("{0}\r\n", strInfo));
                    TxtMsg.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void PlayMedia()
        {
            try
            {
                //string audioPath = @"D:\temp\1504150700000000025.wav";
                string videoPath = @"test.vls";
                MediaPlayer.MediaType = 2;
                //MediaPlayer.AudioUrl = audioPath;
                MediaPlayer.VideoUrl = videoPath;
                MediaPlayer.Play();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region ChangTheme

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
                        , StylePath);
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
                    string uri = string.Format("/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }
        }

        #endregion


        #region ChangLanguage

        #endregion


        #region NetPipeMessage

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    if (PopupPanel != null)
                                    {
                                        PopupPanel.ChangeLanguage();
                                    }
                                    if (PageHead != null)
                                    {
                                        PageHead.SessionInfo = App.Session;
                                        PageHead.InitInfo();
                                    }

                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                                PageHead.SessionInfo = App.Session;
                                PageHead.InitInfo();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        #endregion
    }
}
