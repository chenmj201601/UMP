//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    70a6ba37-57cf-4829-acc9-f0819c54bc3f
//        CLR Version:              4.0.30319.18444
//        Name:                     UCRecordBookmark
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCRecordBookmark
//
//        created by Charley at 2014/12/9 17:01:29
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS3102.Codes;
using UMPS3102.Converters;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3102
{
    /// <summary>
    /// UCRecordBookmark.xaml 的交互逻辑
    /// </summary>
    public partial class UCRecordBookmark
    {
        #region Members

        public QMMainView PageParent;
        public RecordInfoItem RecordInfoItem;
        public List<SettingInfo> ListUserSettingInfos;
        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public Service03Helper Service03Helper;
        public List<RecordEncryptInfo> ListEncryptInfo;

        private ObservableCollection<ViewColumnInfo> mListBookmarkColumns;
        private List<RecordBookmarkItem> mListAllBookmarkItems;
        private ObservableCollection<RecordBookmarkItem> mListUserBookmarkItems;
        private ObservableCollection<BasicUserItem> mListUsers;
        private ObservableCollection<BookmarkRankItem> mListBookmarkRankItems;

        private RecordBookmarkItem mCurrentBookmarkItem;
        private Brush mDrawingBrush = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0));
        private DrawingVisual mVisual;
        private bool mIsDrawing;
        private Point mTopLeft;
        private Point mBottonRight;
        private double mTotalDuration;
        private int mCircleMode;

        #endregion


        public UCRecordBookmark()
        {
            InitializeComponent();

            mListBookmarkColumns = new ObservableCollection<ViewColumnInfo>();
            mListAllBookmarkItems = new List<RecordBookmarkItem>();
            mListUserBookmarkItems = new ObservableCollection<RecordBookmarkItem>();
            mListUsers = new ObservableCollection<BasicUserItem>();
            mListBookmarkRankItems = new ObservableCollection<BookmarkRankItem>();


            Loaded += UCRecordBookmark_Loaded;
            VoicePlayBox.PlayerEvent += mUCPlayBox_PlayerEvent;
            ComboListUsers.SelectionChanged += ComboListUsers_SelectionChanged;
            ListBoxBookmarkLines.SelectionChanged += ListBoxBookmarkLines_SelectionChanged;
            ListViewBookmarks.SelectionChanged += ListViewBookmarks_SelectionChanged;
            DrawingPanel.MouseLeftButtonDown += DrawingPanel_MouseLeftButtonDown;
            DrawingPanel.MouseLeftButtonUp += DrawingPanel_MouseLeftButtonUp;
            DrawingPanel.MouseMove += DrawingPanel_MouseMove;
            DrawingPanel.SizeChanged += DrawingPanel_SizeChanged;
            TxtBookmarkTitle.TextChanged += TxtBookmarkTitle_TextChanged;
            TxtBookmarkContent.TextChanged += TxtBookmarkContent_TextChanged;
            ComboBookmarkRanks.SelectionChanged += ComboBookmarkRanks_SelectionChanged;
            ListViewBookmarks.MouseDoubleClick += ListViewBookmarks_MouseDoubleClick;
            Unloaded += UCRecordBookmark_Unloaded;
            mCircleMode = 0;
        }

        void UCRecordBookmark_Loaded(object sender, RoutedEventArgs e)
        {
            VoicePlayBox.CurrentApp = CurrentApp;
            ComboListUsers.ItemsSource = mListUsers;
            ListViewBookmarks.ItemsSource = mListUserBookmarkItems;
            ListBoxBookmarkLines.ItemsSource = mListUserBookmarkItems;
            ComboBookmarkRanks.ItemsSource = mListBookmarkRankItems;
            //Btn_Start.IsEnabled = false;
            //Btn_Stop.IsEnabled = false;

            InitBookmarkColumns();
            LoadBookmarkRandItems();
            LoadAllBookmarkInfos();
            InitBookmarkUsers();

            Init();
            CreatePlayBox();
            CreateBookmarkColumns();
            CreateToolbarButtons();

            ChangeLanguage();
        }

        private void UCRecordBookmark_Unloaded(object sender, RoutedEventArgs e)//界面关闭之后执行或者重新打开之前执行
        {
            //如果还在录音  就直接结束录音
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                ThisState = 0;
            }
        }

        #region Init and Load

        private void InitBookmarkColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102006");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListBookmarkColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListBookmarkColumns.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadBookmarkRandItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetBookmarkRankList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                mListBookmarkRankItems.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strBookmarkRank = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<BookmarkRankInfo>(strBookmarkRank);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BookmarkRankInfo info = optReturn.Data as BookmarkRankInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBookmarkRankInfo is null"));
                        return;
                    }
                    BookmarkRankItem item = new BookmarkRankItem(info);
                    mListBookmarkRankItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Init()
        {
            if (RecordInfoItem != null)
            {
                mTotalDuration = RecordInfoItem.Duration * 1000.0;
            }
        }

        private void LoadAllBookmarkInfos()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRecordBookmarkList;
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                mListAllBookmarkItems.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strBookmark = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RecordBookmarkInfo>(strBookmark);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordBookmarkInfo info = optReturn.Data as RecordBookmarkInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBookmarkInfo is null"));
                        return;
                    }
                    RecordBookmarkItem item = new RecordBookmarkItem(info);
                    mListAllBookmarkItems.Add(item);
                }
                //这里我是将List里面的数据按偏移量排序
                mListAllBookmarkItems = mListAllBookmarkItems.OrderBy(i => i.Offset).ToList();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBookmarkUsers()
        {
            try
            {
                List<string> listUserID = new List<string>();
                for (int i = 0; i < mListAllBookmarkItems.Count; i++)
                {
                    string strUserID = mListAllBookmarkItems[i].MarkerID.ToString();
                    if (!listUserID.Contains(strUserID))
                    {
                        listUserID.Add(strUserID);
                    }
                }
                mListUsers.Clear();
                if (listUserID.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetResourceProperty;
                    webRequest.ListData.Add("102");
                    webRequest.ListData.Add("Account");
                    webRequest.ListData.Add(listUserID.Count.ToString());
                    for (int i = 0; i < listUserID.Count; i++)
                    {
                        webRequest.ListData.Add(listUserID[i]);
                    }
                    Service11012Client client = new Service11012Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service11012"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("Fail.\tListData is null"));
                        return;
                    }
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        string strInfo = webReturn.ListData[i];
                        string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length < 2) { continue; }
                        long id = Convert.ToInt64(arrInfo[0]);
                        string account = arrInfo[1];
                        if (id == CurrentApp.Session.UserID) { continue; }
                        BasicUserItem item = new BasicUserItem();
                        item.UserID = id;
                        item.Name = account;
                        mListUsers.Add(item);
                    }
                }
                var userItem = mListUsers.FirstOrDefault(u => u.UserID == CurrentApp.Session.UserID);
                if (userItem == null)
                {
                    userItem = new BasicUserItem();
                    userItem.UserID = CurrentApp.Session.UserID;
                    userItem.Name = CurrentApp.Session.UserInfo.Account;
                    mListUsers.Add(userItem);
                }
                ComboListUsers.SelectedItem = userItem;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitUserBookmarkItems()
        {
            try
            {
                var userItem = ComboListUsers.SelectedItem as BasicUserItem;
                if (userItem != null)
                {
                    mListUserBookmarkItems.Clear();
                    for (int i = 0; i < mListAllBookmarkItems.Count; i++)
                    {
                        RecordBookmarkItem item = mListAllBookmarkItems[i];
                        if (item.MarkerID == userItem.UserID)
                        {
                            SetBookmarkItem(item);
                            mListUserBookmarkItems.Add(item);
                        }
                    }
                    if (mListUserBookmarkItems.Count > 0)
                    {
                        mListUserBookmarkItems[0].IsSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region EventHandlers

        void ComboListUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitUserBookmarkItems();
        }

        void ListViewBookmarks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFormValues();
        }

        void ListBoxBookmarkLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFormValues();
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var item = btn.DataContext as ToolButtonItem;
                    if (item != null)
                    {

                        switch (item.Name)
                        {
                            case "BT" + "Save":
                                if (ThisState == 1)
                                {
                                    ShowException("请先结束录音再保存");
                                    return;
                                }
                                if (Btn_Start.IsEnabled == false &&
                                    Btn_Play_Stop.IsEnabled == true &&
                                    Btn_Stop.IsEnabled == false &&
                                    Btn_Play.IsEnabled == false)
                                {
                                    ShowException("标签录音正在播放~请等待标签录音播放结束");
                                    return;
                                }
                                //if (mCurrentBookmarkItem.IsHaveBookMarkRecord == "1")
                                //{
                                //    ShowException("改条");
                                //    return;
                                //}
                                SaveBookmark();
                                SetBookMarkTitle_To_T_21_001();
                                UploadRecordToUMPService();
                                //if (File.Exists(string.Format(@"{0}", mVoiceName)))
                                //{
                                //    //删除当前文件
                                //    File.Delete(string.Format(@"{0}", mVoiceName));
                                //}
                                //mVoiceName = null;
                                //mCurrentBookmarkItem.IsSaveToDB = true;
                                //for (int i = 0; i < mListAllBookmarkItems.Count; i++)
                                //{
                                //    RecordBookmarkItem item_ = mListAllBookmarkItems[i];
                                //    item_.IsSaveToDB = true;
                                //}
                                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("004", "Save Bookmark end"));
                                break;
                            case "BT" + "Delete":
                                DeleteBookmark();
                                //删除标记不需要对数据库进行修改，保存才会修改数据库里的内容
                                //SetBookMarkTitle_To_T_21_001();
                                break;
                            case "BT" + "CircleMode":
                                SetCircleMode();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        void ComboBookmarkRanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
            if (item != null)
            {
                if (item.Teminal == "K")
                {
                    //MessageBox.Show("这是关键词不能改");
                    return;
                }
                if (item.MarkerID == CurrentApp.Session.UserID)
                {
                    var rank = ComboBookmarkRanks.SelectedItem as BookmarkRankItem;
                    if (rank != null)
                    {
                        item.RankID = rank.ID;
                        SetBookmarkItem(item);
                    }
                }
            }
        }

        void TxtBookmarkContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
            if (item != null)
            {
                if (item.MarkerID == CurrentApp.Session.UserID)
                {
                    item.Content = TxtBookmarkContent.Text;
                }
            }
        }

        void TxtBookmarkTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
            if (item != null)
            {
                if (item.MarkerID == CurrentApp.Session.UserID)
                {
                    item.Title = TxtBookmarkTitle.Text;
                }
            }
        }

        private void BookmarkItemLeft_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                var thumb = sender as Thumb;
                if (thumb != null)
                {
                    var item = thumb.Tag as RecordBookmarkItem;
                    //如果是关键词 就不能拖动
                    if (item.Teminal == "K")
                    {
                        return;
                    }
                    if (item != null)
                    {
                        double left = item.CanvasLeft;
                        double width = item.LineWidth;
                        double changed = e.HorizontalChange;
                        double newLeft = left + changed;
                        double newWidth = width - changed;
                        if (newLeft > 0 && newLeft < left + width && newWidth > 0)
                        {
                            item.CanvasLeft = newLeft;
                            item.LineWidth = newWidth;

                            var totalWidth = DrawingSurface.ActualWidth;
                            var totalDuration = mTotalDuration;
                            double offset = (item.CanvasLeft / totalWidth) * totalDuration;
                            double duration = (item.LineWidth / totalWidth) * totalDuration;
                            item.Offset = (int)offset;
                            item.Duration = (int)duration;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BookmarkItemRight_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                var thumb = sender as Thumb;
                if (thumb != null)
                {
                    var item = thumb.Tag as RecordBookmarkItem;
                    //如果是关键词 就不能拖动
                    if (item.Teminal == "K")
                    {
                        return;
                    }
                    if (item != null)
                    {
                        double width = item.LineWidth;
                        double changed = e.HorizontalChange;
                        double newWidth = width + changed;
                        if (newWidth > 0)
                        {
                            item.LineWidth = newWidth;

                            var totalWidth = DrawingSurface.ActualWidth;
                            var totalDuration = mTotalDuration;
                            double offset = (item.CanvasLeft / totalWidth) * totalDuration;
                            double duration = (item.LineWidth / totalWidth) * totalDuration;
                            item.Offset = (int)offset;
                            item.Duration = (int)duration;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListViewBookmarks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
            if (item != null)
            {
                mCurrentBookmarkItem = item;
                mCircleMode = 1;
                //if (mUCPlayBox != null)
                //{
                //    mUCPlayBox.CircleMode = mCircleMode;
                //    mUCPlayBox.StartPosition = item.Offset;
                //    mUCPlayBox.StopPostion = item.Offset + item.Duration;
                //}
                VoicePlayBox.CircleMode = mCircleMode;
                VoicePlayBox.StartPosition = item.Offset;
                VoicePlayBox.StopPostion = item.Offset + item.Duration;
                //VoicePlayBox.PlayerEvent += mUCPlayBox_PlayerEvent; 
                VoicePlayBox.Play(false);
                CreateToolbarButtons();
            }
        }

        void mUCPlayBox_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            try
            {
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;
                switch (code)
                {
                    case MediaPlayerEventCodes.MEDIAOPENED:
                        mTotalDuration = TimeSpan.Parse(param.ToString()).TotalMilliseconds;
                        InitUserBookmarkItems();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DrawingPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitUserBookmarkItems();
        }

        #endregion

        #region Public Functions

        public void Close()
        {
            if (VoicePlayBox != null)
            {
                VoicePlayBox.Close();
            }
        }

        #endregion

        #region Others

        private void CreateBookmarkColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListBookmarkColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListBookmarkColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102006{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102006{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        Binding binding;
                        binding = new Binding(columnInfo.ColumnName);
                        if (columnInfo.ColumnName == "Offset"
                            || columnInfo.ColumnName == "Duration")
                        {
                            binding.Converter = new MilliSecondToTimeConverter();
                        }
                        gvc.DisplayMemberBinding = binding;
                        gv.Columns.Add(gvc);
                    }
                }
                ListViewBookmarks.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreatePlayBox()
        {
            if (RecordInfoItem == null) { return; }
            VoicePlayBox.MainPage = PageParent;
            VoicePlayBox.ListSftpServers = ListSftpServers;
            VoicePlayBox.ListUserSettingInfos = ListUserSettingInfos;
            VoicePlayBox.ListDownloadParams = ListDownloadParams;
            VoicePlayBox.Service03Helper = Service03Helper;
            VoicePlayBox.ListEncryptInfo = ListEncryptInfo;
            VoicePlayBox.RecordInfoItem = RecordInfoItem;
            VoicePlayBox.IsAutoPlay = true;
            VoicePlayBox.CircleMode = mCircleMode;
            if (mCurrentBookmarkItem != null)
            {
                VoicePlayBox.StartPosition = mCurrentBookmarkItem.Offset;
                VoicePlayBox.StopPostion = mCurrentBookmarkItem.Offset + mCurrentBookmarkItem.Duration;
            }
            //VoicePlayBox.Play(true);      //由于是自动播放，无需再调用Play方法
        }

        private void CreateToolbarButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "BT" + "Delete";
                item.Display = CurrentApp.GetLanguageInfo("3102B011", "Delete");
                item.Tip = CurrentApp.GetLanguageInfo("3102B011", "Delete");
                item.Icon = "Images/remove.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "Save";
                item.Display = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Tip = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Icon = "Images/save.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "CircleMode";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP005{0}", mCircleMode), mCircleMode.ToString());
                item.Tip = CurrentApp.GetLanguageInfo(string.Format("3102TIP005{0}", mCircleMode), mCircleMode.ToString());
                item.Icon = mCircleMode == 1 ? "Images/singlecircle.png" : mCircleMode == 2 ? "Images/listcircle.png" : "Images/nocircle.png";
                listBtns.Add(item);

                PanelToolButton.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    ToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += ToolButton_Click;
                    PanelToolButton.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DrawVisualElement(Point p1, Point p2)
        {
            if (mVisual == null)
            {
                return;
            }
            using (DrawingContext dc = mVisual.RenderOpen())
            {
                dc.DrawRectangle(mDrawingBrush, null, new Rect(p1, p2));
            }
        }

        private void CreateBookmark(BookmarkLineInfo line)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                OperationReturn optReturn = GetBookmarkInfoID();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                long serialID = Convert.ToInt64(optReturn.Data);
                RecordBookmarkInfo info = new RecordBookmarkInfo();
                info.SerialID = serialID;
                info.RecordRowID = RecordInfoItem.RowID;
                info.RecordID = RecordInfoItem.SerialID;
                info.Offset = line.Begin;
                info.Duration = line.Duration;
                info.Title = string.Format("Title:{0}", info.Offset);
                info.Content = string.Empty;
                info.State = "1";
                info.MarkerID = CurrentApp.Session.UserID;
                info.MarkTime = DateTime.Now;
                info.RankID = 0;
                info.Teminal = "U";
                RecordBookmarkItem item = new RecordBookmarkItem(info);
                item.CanvasLeft = line.Left;
                item.LineWidth = line.Width;
                item.Background = Brushes.White;
                mListUserBookmarkItems.Add(item);
                mListAllBookmarkItems.Add(item);
                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //当选择了bookmark标签（从波形图上选择，以及在标签列表里选择）
        private void SetFormValues()
        {
            var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
            if (item != null)
            {
                Btn_Start.IsEnabled = true;
                mCurrentBookmarkItem = item;
                TxtBookmarkTitle.Text = item.Title;
                TxtBookmarkContent.Text = item.Content;
                if (mCurrentBookmarkItem.IsHaveBookMarkRecord == "1")
                {
                    //如果这个字段表示为“1”，表示当前bookmark里面有音频标签
                    Btn_Play.Visibility = Visibility.Visible;
                    Btn_Play_Stop.Visibility = Visibility.Visible;
                    TxtVoiceName.Text = string.Format(@"{0}{1}{2}.wav", mCurrentBookmarkItem.RecordID, mCurrentBookmarkItem.SerialID, CurrentApp.Session.UserID);
                    TimeLengthName.Text = mCurrentBookmarkItem.BookmarkTimesLength;
                }
                else
                {
                    //Btn_Start.Visibility = Visibility.Visible;
                    Btn_Play.Visibility = Visibility.Hidden;
                    Btn_Play_Stop.Visibility = Visibility.Hidden;
                    TxtVoiceName.Text = "";
                    TimeLengthName.Text = "";
                }
                var rank = mListBookmarkRankItems.FirstOrDefault(r => r.ID == item.RankID);
                if (rank != null)
                {
                    ComboBookmarkRanks.SelectedItem = rank;
                }
                else
                {
                    ComboBookmarkRanks.SelectedItem = null;
                }
            }
        }

        private void SetBookmarkItem(RecordBookmarkItem item)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                double totalWidth = DrawingSurface.ActualWidth;
                double totalDuration = mTotalDuration;
                double left = (item.Offset / totalDuration) * totalWidth;
                double width = (item.Duration / totalDuration) * totalWidth;
                item.CanvasLeft = left;
                item.LineWidth = width;

                var rank = mListBookmarkRankItems.FirstOrDefault(r => r.ID == item.RankID);
                if (rank != null)
                {
                    string color = rank.Color;
                    try
                    {
                        string r = color.Substring(0, 2);
                        string g = color.Substring(2, 2);
                        string b = color.Substring(4, 2);
                        Color temp = Color.FromRgb((byte)Convert.ToInt32(r, 16), (byte)Convert.ToInt32(g, 16),
                            (byte)Convert.ToInt32(b, 16));
                        item.Background = new SolidColorBrush(temp);
                    }
                    catch { item.Background = Brushes.White; }
                }
                else
                {
                    item.Background = Brushes.White;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn GetBookmarkInfoID()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("304");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                optReturn.Data = webReturn.Data;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion

        #region OperationsBtn_Play

        private void DeleteBookmark()
        {
            try
            {
                var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
                if (item != null)
                {
                    if (item.MarkerID == CurrentApp.Session.UserID)
                    {
                        var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                            CurrentApp.GetMessageLanguageInfo("009", "Confirm delete this Bookmark?"),
                            item.Title),
                            CurrentApp.AppName, MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            mListUserBookmarkItems.Remove(item);
                            mListAllBookmarkItems.Remove(item);
                            if (mListUserBookmarkItems.Count > 0)
                            {
                                mListUserBookmarkItems[0].IsSelected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveBookmark()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtBookmarkTitle.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N018", "Input is empty"));
                    return;
                }
                if (TxtBookmarkTitle.Text.Length > 128)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N036", "Save the content is too long"));
                    return;
                }
                if (TxtBookmarkContent.Text.Length > 1024)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N036", "Save the content is too long"));
                    return;
                }
                if (RecordInfoItem == null) { return; }
                List<RecordBookmarkInfo> listBookmarks = new List<RecordBookmarkInfo>();
                for (int i = 0; i < mListAllBookmarkItems.Count; i++)
                {
                    RecordBookmarkItem item = mListAllBookmarkItems[i];
                    if (item.MarkerID == CurrentApp.Session.UserID)
                    {
                        if (item.BookmarkInfo != null)
                        {
                            item.SetValues();
                            listBookmarks.Add(item.BookmarkInfo);
                        }
                    }
                }
                if (listBookmarks.Count == 0)//代表了所有的关于这条录音的标记全部删除
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3102Codes.DeleteUsersRecordBookMark;
                    webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                    Service31021Client client = new Service31021Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31021"));
                    //Service31021Client client = new Service31021Client();
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    #region 写操作日志

                    string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102001RecordReference"), RecordInfoItem.SerialID);
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion
                }
                if (listBookmarks.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3102Codes.SaveRecordBookmarkInfo;
                    webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                    webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    int count = listBookmarks.Count;
                    webRequest.ListData.Add(count.ToString());
                    OperationReturn optReturn;
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(listBookmarks[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31021Client client = new Service31021Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    #region 写操作日志

                    string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102001RecordReference"), RecordInfoItem.SerialID);
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion

                    //CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("004", "Save Bookmark end"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetBookMarkTitle_To_T_21_001()
        {
            try
            {
                //新加的一个 这里我只是当一个零时的变量
                List<RecordBookmarkItem> mListAllBookmarkItems_new;
                mListAllBookmarkItems_new = new List<RecordBookmarkItem>();

                if (RecordInfoItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRecordBookmarkList;
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                mListAllBookmarkItems_new.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strBookmark = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RecordBookmarkInfo>(strBookmark);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordBookmarkInfo info = optReturn.Data as RecordBookmarkInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBookmarkInfo is null"));
                        return;
                    }
                    RecordBookmarkItem item = new RecordBookmarkItem(info);
                    mListAllBookmarkItems_new.Add(item);
                }
                string AllBookMarkTitle = string.Empty;
                for (int i = 0; i < mListAllBookmarkItems_new.Count; i++)
                {
                    if (AllBookMarkTitle.Length > 500)
                    {
                        break;
                    }
                    if (i < mListAllBookmarkItems_new.Count - 1)
                    {
                        AllBookMarkTitle += mListAllBookmarkItems_new[i].MarkerID + ConstValue.SPLITER_CHAR_2 + mListAllBookmarkItems_new[i].Title + ConstValue.SPLITER_CHAR;
                    }
                    else
                    {
                        AllBookMarkTitle += mListAllBookmarkItems_new[i].MarkerID + ConstValue.SPLITER_CHAR_2 + mListAllBookmarkItems_new[i].Title;
                    }
                }

                int BookMarkedTimes = mListAllBookmarkItems_new.Count;
                string BookMarkedTimes_str = Convert.ToString(BookMarkedTimes);

                //下面是获得记录表的名字（考虑到分表的情况）
                QueryStateInfo queryStateInfo = new QueryStateInfo();
                queryStateInfo.RowID = 0;
                var tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo == null)
                {
                    tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.VoiceID);
                    if (tableInfo == null)
                    {
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        //按录音服务器查询,没有实现，暂时还是按普通方式来
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token);
                    }
                }
                else
                {
                    DateTime baseTime = RecordInfoItem.StartRecordTime;
                    string partTable;
                    partTable = baseTime.ToString("yyMM");
                    queryStateInfo.TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                        CurrentApp.Session.RentInfo.Token, partTable);
                }

                SetBookMarkTitle_To_T_21_001(RecordInfoItem.SerialID.ToString(), AllBookMarkTitle, queryStateInfo.TableName, BookMarkedTimes_str);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetBookMarkTitle_To_T_21_001(string SerialID, string BookMarkTitleValue, string TableName, string BookMarkedTimes)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.SaveBookMarkTitleToT_21_001;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SerialID);
                webRequest.ListData.Add(BookMarkTitleValue);
                webRequest.ListData.Add(TableName);
                webRequest.ListData.Add(BookMarkedTimes);
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetCircleMode()
        {
            try
            {
                if (mCircleMode == 0)
                {
                    var item = ListViewBookmarks.SelectedItem as RecordBookmarkItem;
                    if (item == null
                        || VoicePlayBox.StartPosition < 0
                        || VoicePlayBox.StopPostion <= VoicePlayBox.StartPosition)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3102N055", "未选中标签，不能进行单循环播放"));
                        return;
                    }
                    mCircleMode = 1;
                }
                else if (mCircleMode == 1)
                {
                    mCircleMode = 0;
                }
                else
                {
                    mCircleMode = 0;
                }
                //if (mUCPlayBox != null)
                //{
                //    mUCPlayBox.CircleMode = mCircleMode;
                //}
                VoicePlayBox.CircleMode = mCircleMode;
                CreateToolbarButtons();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Drawing EventHandlers

        void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (mIsDrawing)
                {
                    Point point = e.GetPosition(DrawingSurface);
                    if (point.X > 0.0)
                    {
                        if (point.Y > 0.0 && point.Y < 120)
                        {
                            DrawVisualElement(mTopLeft, point);
                            mBottonRight = point;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DrawingPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var userItem = ComboListUsers.SelectedItem as BasicUserItem;
                if (userItem != null && userItem.UserID == CurrentApp.Session.UserID)
                {
                    mVisual = new DrawingVisual();
                    DrawingSurface.AddVisual(mVisual);
                    var point = e.GetPosition(DrawingSurface);
                    mIsDrawing = true;
                    mTopLeft = point;
                    DrawingPanel.CaptureMouse();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DrawingPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var userItem = ComboListUsers.SelectedItem as BasicUserItem;
                if (userItem != null && userItem.UserID == CurrentApp.Session.UserID)
                {
                    mIsDrawing = false;
                    if (mVisual != null)
                    {
                        DrawingSurface.DeleteVisual(mVisual);
                    }
                    DrawingPanel.ReleaseMouseCapture();
                    Point point = e.GetPosition(DrawingSurface);
                    if (point.X > 0.0)
                    {
                        if (point.Y > 0.0 && point.Y < 120)
                        {
                            DrawVisualElement(mTopLeft, point);
                            mBottonRight = point;
                        }
                    }
                    //不小于10才算有效
                    if (Math.Abs(mBottonRight.X - mTopLeft.X) < 10)
                    {
                        return;
                    }
                    BookmarkLineInfo line = new BookmarkLineInfo();
                    double beginX = Math.Min(mTopLeft.X, mBottonRight.X);
                    double endX = Math.Max(mTopLeft.X, mBottonRight.X);
                    line.Left = beginX;
                    line.Width = (endX - beginX);
                    double width = DrawingSurface.ActualWidth;
                    line.TotalWidth = width;
                    double begin = (beginX / width) * mTotalDuration;
                    double end = (endX / width) * mTotalDuration;
                    line.Begin = (int)begin;
                    line.Duration = ((int)end) - line.Begin;
                    CreateBookmark(line);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region ChangeLangugage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                Btn_Start.ToolTip = CurrentApp.GetLanguageInfo("3102TIP0124", "Start Record");
                Btn_Stop.ToolTip = CurrentApp.GetLanguageInfo("3102TIP0125", "Stop Record");
                Btn_Play.ToolTip = CurrentApp.GetLanguageInfo("3102TIP0126", "Play");
                Btn_Play_Stop.ToolTip = CurrentApp.GetLanguageInfo("3102TIP0127", "Stop Play");

                CreateBookmarkColumns();
                CreateToolbarButtons();

                LbBookmarkTitle.Text = CurrentApp.GetLanguageInfo("COL3102006Title", "Title");
                LbBookmarkRank.Text = CurrentApp.GetLanguageInfo("COL3102006Rank", "Rank");
                LbBookmarkContent.Text = CurrentApp.GetLanguageInfo("COL3102006Content", "Content");
                LbBookmarkVoiceContent.Text = CurrentApp.GetLanguageInfo("COL3102006VoiceBM", "录音标签");
                LTextLength.Text = CurrentApp.GetLanguageInfo("COL3102006VoiceBMTimeLenth", "录音标签时长(s)");
            }
            catch { }
        }

        #endregion

        #region BOOKMARK 录音文本保存
        private IWaveIn waveIn;
        private WaveFileWriter writer;
        string mVoiceName;
        private string mVoiceChunName;
        private Thread thread;


        MemoryStream mtempStream;
        private WaveStream mWaveStream;
        private WaveOut mWaveOut;
        /// <summary>
        /// 1 表示正在录音，0表示未录音
        /// </summary>
        private int ThisState = 0;
        /// <summary>
        /// 是否是手动停止的 1表示是手动停止 0表示不是
        /// </summary>
        private int IsHandStop = 0;
        /// <summary>
        /// 当前录音时长
        /// </summary>
        private int mTimeLenth = 0;


        /// <summary>
        /// 开始录音
        /// </summary>
        private void StartRecording()
        {
            try
            {
                ThisState = 1;
                if (waveIn != null) return;
                waveIn = new WaveIn { WaveFormat = new WaveFormat(8000, 1) };//设置码率
                string thisPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}/", CurrentApp.AppName));
                //string thisPath = @"C:\";
                if (mCurrentBookmarkItem == null)
                {
                    return;
                }
                //string datetimenow = DateTime.Now.ToString("yyyyMMddHHmmss");
                ////路径+文件名
                string voiceName = string.Format(@"{0}{1}{2}{3}.wav",
                    thisPath, mCurrentBookmarkItem.RecordID, mCurrentBookmarkItem.SerialID, mCurrentBookmarkItem.MarkerID);
                ////文件名
                mVoiceChunName = string.Format(@"{0}{1}{2}.wav",
                    mCurrentBookmarkItem.RecordID, mCurrentBookmarkItem.SerialID, mCurrentBookmarkItem.MarkerID);
                //如果文件在本地存在
                if (File.Exists(string.Format(@"{0}", voiceName)))
                {
                    //删除当前文件
                    File.Delete(string.Format(@"{0}", voiceName));
                }
                mVoiceName = voiceName;
                mtempStream = new MemoryStream();
                writer = new WaveFileWriter(voiceName, waveIn.WaveFormat);

                //将录音先写入缓存 不再写入文件夹
                //writer = new WaveFileWriter(mtempStream, waveIn.WaveFormat);

                waveIn.DataAvailable += waveIn_DataAvailable;
                waveIn.RecordingStopped += OnRecordingStopped;

                waveIn.StartRecording();
                if (IsHandStop == 0)
                {
                    if (thread != null)
                    {
                        try
                        {
                            thread.Abort();
                        }
                        catch (Exception ex)
                        {

                        }
                        thread = null;
                    }
                    thread = new Thread(new ThreadStart(() =>
                    {
                        int count = 0;
                        while (count <= 3000 && IsHandStop == 0)
                        {
                            //挂起100毫秒
                            Thread.Sleep(100);
                            count++;
                        }
                        if (IsHandStop == 0)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                if (IsHandStop == 0)
                                {
                                    StopRecordingThings(); //这个方法调用了UI里的控件
                                }

                            }));
                        }
                        //Thread.Sleep(5 * 1000);
                        ////调用主线程  停止录音{因为停止录音的时候 也需要访问UI里面的  Dispatcher可以理解为调度主线程，把程序抛给主线程}
                        //Dispatcher.Invoke(new Action(() =>
                        //{
                        //    if (IsHandStop == 0)
                        //    {
                        //        StopRecordingThings(); //这个方法调用了UI里的控件
                        //    }

                        //}));
                    }));
                    thread.Start();
                }
                mCurrentBookmarkItem.BookMarkVoicePath = voiceName;
                mCurrentBookmarkItem.BookMarkVoiceName = string.Format(@"{0}{1}{2}.wav",
                    mCurrentBookmarkItem.RecordID, mCurrentBookmarkItem.SerialID, mCurrentBookmarkItem.MarkerID);
                Btn_Stop.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Btn_Stop.IsEnabled = false;
                ShowException(ex.ToString());
            }
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        private void StopRecording()
        {
            try
            {
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    waveIn.Dispose();
                    ThisState = 0;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        /// <summary>
        /// 录音中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);

                int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);//录音时间获取
                mTimeLenth = secondsRecorded;
                //if (IsHandStop == 0)
                //{
                //    Thread thread = new Thread(new ThreadStart(() =>
                //    {
                //        Thread.Sleep(2 * 1000);
                //        //调用主线程  停止录音{因为停止录音的时候 也需要访问UI里面的  Dispatcher可以理解为调度主线程，把程序抛给主线程}
                //        Dispatcher.Invoke(new Action(() =>
                //        {
                //            StopRecordingThings(); //这个方法调用了UI里的控件
                //        }));
                //    }));
                //    thread.Start();
                //}
                //if (secondsRecorded >= 2)
                //{
                //    //StopRecording();
                //    StopRecordingThings();
                //    CurrentApp.ShowInfoMessage("录音时间过长自动停止录音");
                //    return;
                //}
            }));
        }


        /// <summary>
        /// 停止录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {

                if (waveIn != null) // 关闭录音对象
                {
                    waveIn.Dispose();
                    waveIn = null;
                }
                if (writer != null)//关闭文件流
                {
                    //一个文件流只有关闭之后才能访问或者播放
                    writer.Close();
                    writer = null;
                }
                if (e.Exception != null)
                {
                    MessageBox.Show(String.Format("出现问题 {0}", e.Exception.Message));
                }
            }));
        }

        /// <summary>
        /// 将录音上传到UMP服务器
        /// </summary>
        private void UploadRecordToUMPService()
        {
            for (int i = 0; i < mListUserBookmarkItems.Count; i++)
            {
                if (string.IsNullOrEmpty(mListUserBookmarkItems[i].BookMarkVoicePath))
                {
                    continue;
                }
                else
                {
                    string ServerPath = System.IO.Path.Combine(string.Format(GetBookMarkRecordPath() + "{0}", mListUserBookmarkItems[i].BookMarkVoiceName));

                    //上传到服务器上的文件夹的路径
                    //string ServerPath_ = System.IO.Path.Combine(string.Format(@"C:\Program Files\VoiceCyber\UMP\BookMarkRecord\{0}", mVoiceChunName));
                    //if (string.IsNullOrEmpty(mVoiceChunName))
                    //{
                    //    return;
                    //}

                    //先去服务器上看看有没有该录音文件 有就删除
                    if (string.IsNullOrEmpty(mListUserBookmarkItems[i].BookMarkVoiceName))
                    {
                        return;
                    }
                    else
                    {
                        UMPDeleteOperation(mListUserBookmarkItems[i].BookMarkVoiceName);
                    }
                    ////本地要被上传的文件路径
                    byte[] FileArrayRead = System.IO.File.ReadAllBytes(mListUserBookmarkItems[i].BookMarkVoicePath);

                    //byte[] tempbyte = new byte[mtempStream.Length]; 现在不用内存流，直接写到本地
                    mtempStream.Read(FileArrayRead, 0, (int)mtempStream.Length);
                    WebReturn webReturn = null;
                    UpRequest upRquest = new UpRequest();
                    List<byte> listFileByte = new List<byte>();
                    Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                    string aae = client.Endpoint.Address.Uri.AbsolutePath;
                    int count = 0;
                    foreach (byte tempByte in FileArrayRead)
                    {
                        listFileByte.Add(tempByte);
                        count += 1;
                        if (count == 1048576)
                        {
                            //OperationReturn optReturn;
                            upRquest.SvPath = ServerPath;
                            upRquest.ListByte = listFileByte.ToArray();
                            //WebRequest webRequest = new WebRequest();
                            //optReturn = XMLHelper.SeriallizeObject(upRquest);
                            //if (!optReturn.Result)
                            //{
                            //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            //    return;
                            //}
                            //webRequest.Code = (int)S3102Codes.UpLoadFiles;
                            //webRequest.ListData.Add(optReturn.Data.ToString());
                            webReturn = client.UMPUpOperation(upRquest);
                            listFileByte.Clear();
                            count = 0;
                        }
                    }
                    if (listFileByte.Count > 0)
                    {
                        //OperationReturn optReturn;
                        upRquest.SvPath = ServerPath;
                        upRquest.ListByte = listFileByte.ToArray();
                        //WebRequest webRequest = new WebRequest();
                        //optReturn = XMLHelper.SeriallizeObject(upRquest);
                        //if (!optReturn.Result)
                        //{
                        //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        //    return;
                        //}
                        //webRequest.Code = (int)S3102Codes.UpLoadFiles;
                        //webRequest.ListData.Add(optReturn.Data.ToString());
                        webReturn = client.UMPUpOperation(upRquest);
                        bool a = webReturn.Result;
                    }
                    //上传完之后再关闭 这个close 是关闭当前流并释放相关资源
                    //writer.Close();
                    //writer = null;
                }
            }

        }

        private string GetBookMarkRecordPath()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S3102Codes.GetBookMarkRecordPath;
            webRequest.Session = CurrentApp.Session;
            Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                               WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
            WebReturn webReturn = client.DoOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return "0";
            }
            return webReturn.Data;
        }

        private void UMPDeleteOperation(string Filename)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.UMPDeleteOperation;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(Filename);
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        void PlayServerVoice(string mAudioUrl)
        {
            WaveStream reader;
            if (mAudioUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {

                if (mAudioUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                {
                    reader = new Mp3NetworkStream(mAudioUrl);
                }
                else
                {
                    reader = new NetWorkWaveReader(mAudioUrl);
                    if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                       reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    {
                        reader = WaveFormatConversionStream.CreatePcmStream(reader);
                        reader = new BlockAlignReductionStream(reader);
                    }
                }
            }
            else
            {
                if (mAudioUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                {
                    reader = new Mp3FileReader(mAudioUrl);
                }
                else
                {
                    reader = new WaveFileReader(mAudioUrl);
                    if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                        reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    {
                        reader = WaveFormatConversionStream.CreatePcmStream(reader);
                        reader = new BlockAlignReductionStream(reader);
                    }
                }
            }
            if (mWaveStream != null) { mWaveStream.Dispose(); }
            mWaveStream = reader;
            if (CreateWaveOut())
            {
                if (mWaveOut != null)
                {
                    mWaveOut.Play();
                }
            }

        }

        private bool CreateWaveOut()
        {
            //OperationReturn optReturn;
            if (mWaveStream == null)
            {
                return false;
            }
            if (mWaveOut != null)
            {
                try
                {
                    mWaveOut.Stop();
                    mWaveOut.Dispose();
                    mWaveOut = null;
                }
                catch { }
            }
            try
            {
                mWaveOut = new WaveOut();
                //PlaybackStopped是一个播放停止事件 当音频已播完 或者当音频强制停止 都会进入这个事件
                mWaveOut.PlaybackStopped += mWaveOut_PlaybackStopped;
                //WaveStream类继承了IWaveProvider这个接口  这个接口是 直译是 波的提供者
                mWaveOut.Init(mWaveStream);
                //mWaveOut.ChannelMode = WaveOutChannelMode.Default;
                //mWaveOut.Volume = (float)1;
                return true;
            }
            catch (Exception ex)
            {
                //optReturn = new OperationReturn();
                //optReturn.Code = Defines.RET_FAIL;
                //optReturn.Message = ex.Message;
                //ShowException(optReturn);
                return false;
            }
        }

        void mWaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            try
            {
                if (mWaveStream != null)
                {
                    mWaveStream.Close();
                    mWaveStream.Dispose();
                    mWaveStream = null;
                }
                Btn_Play_Stop.IsEnabled = false;
                Btn_Play.IsEnabled = true;
                Btn_Stop.IsEnabled = false;
                Btn_Start.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            IsHandStop = 0;
            int aaa = WaveIn.DeviceCount;
            if (aaa == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N056", "No Device !"));
                return;
            }
            StartRecording();
            //if (IsHandStop == 0)
            //{
            //    Thread thread = new Thread(new ThreadStart(() =>
            //    {
            //        Thread.Sleep(2 * 1000);
            //        调用主线程  停止录音{因为停止录音的时候 也需要访问UI里面的  Dispatcher可以理解为调度主线程，把程序抛给主线程}
            //        Dispatcher.Invoke(new Action(() =>
            //        {
            //            StopRecordingThings(); //这个方法调用了UI里的控件
            //        }));
            //    }));
            //    thread.Start();
            //}
            //DingShiQi();
            Btn_Start.IsEnabled = false;
            Btn_Play.IsEnabled = false;
            Btn_Play_Stop.IsEnabled = false;
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            try
            {
                StopRecordingThings();
                IsHandStop = 1;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        //播放录音的事件
        private void Button_Click_Play(object sender, RoutedEventArgs e)
        {
            try
            {
                //根据当前选中的标签  来获得他里面的内容
                if (mCurrentBookmarkItem.IsHaveBookMarkRecord == "1")
                {

                    if (mCurrentBookmarkItem.BookmarkInfo.IsHaveBookMarkRecord == "0" || mCurrentBookmarkItem.BookmarkInfo.IsHaveBookMarkRecord == null)
                    {
                        string thisPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP/{0}/", CurrentApp.AppName));
                        string voiceName = string.Format(@"{0}{1}",
                   thisPath, TxtVoiceName.Text.ToString());
                        //播放刚录的音频
                        PlayServerVoice(voiceName);
                        //mWaveStream.Dispose();
                    }
                    else
                    {
                        //如果当前没有录的话，那么就播放服务器上的音频
                        //首先获得服务器上的文件的地址，然后播放
                        //string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port - 1), string.Format(@"BookMarkRecord\{0}", TxtVoiceName.Text));
                        string ServerPath = System.IO.Path.Combine(string.Format("{0}://{1}:{2}", CurrentApp.Session.AppServerInfo.Protocol, CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port), string.Format(@"BookMarkRecord\{0}", TxtVoiceName.Text));//测试的时候用这个  因为测试是用的http  端口是8081，放在UMP上面是用的HTTPS  端口是8082
                        ServerPath = ServerPath.Replace("\\", "/");
                        PlayServerVoice(ServerPath);

                    }
                }
                Btn_Start.IsEnabled = false;
                Btn_Play_Stop.IsEnabled = true;
                Btn_Stop.IsEnabled = false;
                Btn_Play.IsEnabled = false;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private void Button_Click_Play_Stop(object sender, RoutedEventArgs e)
        {
            if (mWaveOut != null)
            {
                mWaveOut.Stop();
                mWaveOut = null;
            }
            //if (mWaveStream != null)
            //{
            //    mWaveStream.Close();
            //    mWaveStream.Dispose();
            //    mWaveStream = null;
            //}
            //Btn_Play_Stop.IsEnabled = false;
            //Btn_Play.IsEnabled = true;
            //Btn_Stop.IsEnabled = false;
            //Btn_Start.IsEnabled = true;
        }

        private void StopRecordingThings()
        {
            try
            {
                TxtVoiceName.Text = mVoiceChunName;
                StopRecording();

                //int a = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);
                TimeLengthName.Text = mTimeLenth.ToString();
                //RecordTimeLength = a.ToString();
                //MessageBox.Show(a.ToString());
                //停止录音之后  要将当前选中的标签的IsHaveBookMarkRecord标记为“1”
                mCurrentBookmarkItem.IsHaveBookMarkRecord = "1";
                mCurrentBookmarkItem.BookmarkTimesLength = mTimeLenth.ToString();
                Btn_Play.Visibility = System.Windows.Visibility.Visible;
                Btn_Play_Stop.Visibility = Visibility.Visible;
                Btn_Stop.IsEnabled = false;
                Btn_Start.IsEnabled = true;
                Btn_Play.IsEnabled = true;
                Btn_Play_Stop.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //if (File.Exists(string.Format(@"{0}", mVoiceName)))
            //{
            //    //删除当前文件
            //    File.Delete(string.Format(@"{0}", mVoiceName));
            //}
            //mVoiceName = null;
        }

        ////删除本地录音标签
        //private void DeleteLocalBookMarkRecord()
        //{
        //    if (File.Exists(string.Format(@"{0}", mVoiceName)))
        //    {
        //        //删除当前文件
        //        File.Delete(string.Format(@"{0}", mVoiceName));
        //    }
        //    mVoiceName = null;
        //}

        #endregion

    }
}
