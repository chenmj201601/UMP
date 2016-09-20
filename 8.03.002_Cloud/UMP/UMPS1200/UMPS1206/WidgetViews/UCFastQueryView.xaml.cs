//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5cff3258-065d-4f6c-bde4-06a5ac963cd9
//        CLR Version:              4.0.30319.42000
//        Name:                     UCFastQueryView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.WidgetViews
//        File Name:                UCFastQueryView
//
//        created by Charley at 2016/3/24 10:22:19
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UMPS1206.Common31021;
using UMPS1206.Models;
using UMPS1206.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;

namespace UMPS1206.WidgetViews
{
    /// <summary>
    /// UCFastQueryView.xaml 的交互逻辑
    /// </summary>
    public partial class UCFastQueryView : IWidgetView
    {

        #region Members

        public WidgetItem WidgetItem { get; set; }
        public IList<BasicDataInfo> ListBasicDataInfos { get; set; }
        public bool IsCenter { get; set; }
        public bool IsFull { get; set; }

        private bool mIsInited;
        private List<QueryCondition> mListQueryConditions;
        private ObservableCollection<FastQueryItem> mListFastQueryItems;

        #endregion


        public UCFastQueryView()
        {
            InitializeComponent();

            mListQueryConditions = new List<QueryCondition>();
            mListFastQueryItems = new ObservableCollection<FastQueryItem>();

            Loaded += UCFastQueryView_Loaded;
        }

        void UCFastQueryView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ChartFastQuery.ItemsSource = mListFastQueryItems;

                CommandBindings.Add(new CommandBinding(ItemClickCommand, ItemClickCommand_Executed,
                  (s, ce) => ce.CanExecute = true));

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadQueryConditions();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateFastQueryItem();
                    SetViewMode();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadQueryConditions()
        {
            try
            {
                mListQueryConditions.Clear();

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetUserQueryCondition;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<QueryCondition>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    QueryCondition info = optReturn.Data as QueryCondition;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tQueryCondition is null"));
                        return;
                    }
                    if (info.UseCount <= 0) { continue; }
                    mListQueryConditions.Add(info);
                }


                //QueryCondition info = new QueryCondition();
                //info.ID = 3020000000000000001;
                //info.Name = "Test1";
                //info.Description = info.Name;
                //info.UserID = CurrentApp.Session.UserID;
                //info.Creator = info.UserID;
                //info.CreateTime = DateTime.Now.AddHours(-5);
                //info.CreateType = "S";
                //info.SortID = 0;
                //info.LastQueryTime = DateTime.Now.AddMinutes(-30);
                //info.IsEnable = true;
                //info.UseCount = 5;
                //info.RecordCount = 210;
                //mListQueryConditions.Add(info);

                //info = new QueryCondition();
                //info.ID = 3020000000000000002;
                //info.Name = "Test2";
                //info.Description = info.Name;
                //info.UserID = CurrentApp.Session.UserID;
                //info.Creator = info.UserID;
                //info.CreateTime = DateTime.Now.AddHours(-15);
                //info.CreateType = "S";
                //info.SortID = 0;
                //info.LastQueryTime = DateTime.Now.AddMinutes(-120);
                //info.IsEnable = true;
                //info.UseCount = 3;
                //info.RecordCount = 510;
                //mListQueryConditions.Add(info);

                //info = new QueryCondition();
                //info.ID = 3020000000000000003;
                //info.Name = "Test3";
                //info.Description = info.Name;
                //info.UserID = CurrentApp.Session.UserID;
                //info.Creator = info.UserID;
                //info.CreateTime = DateTime.Now.AddHours(-25);
                //info.CreateType = "S";
                //info.SortID = 0;
                //info.LastQueryTime = DateTime.Now.AddMinutes(-10);
                //info.IsEnable = true;
                //info.UseCount = 21;
                //info.RecordCount = 320;
                //mListQueryConditions.Add(info);

                //info = new QueryCondition();
                //info.ID = 3020000000000000004;
                //info.Name = "Test4";
                //info.Description = info.Name;
                //info.UserID = CurrentApp.Session.UserID;
                //info.Creator = info.UserID;
                //info.CreateTime = DateTime.Now.AddHours(-45);
                //info.CreateType = "S";
                //info.SortID = 0;
                //info.LastQueryTime = DateTime.Now.AddMinutes(-330);
                //info.IsEnable = true;
                //info.UseCount = 32;
                //info.RecordCount = 5;
                //mListQueryConditions.Add(info);

                //info = new QueryCondition();
                //info.ID = 3020000000000000005;
                //info.Name = "Test5";
                //info.Description = info.Name;
                //info.UserID = CurrentApp.Session.UserID;
                //info.Creator = info.UserID;
                //info.CreateTime = DateTime.Now.AddHours(-2);
                //info.CreateType = "S";
                //info.SortID = 0;
                //info.LastQueryTime = DateTime.Now.AddMinutes(-10);
                //info.IsEnable = true;
                //info.UseCount = 13;
                //info.RecordCount = 260;
                //mListQueryConditions.Add(info);

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create Operations

        private void CreateFastQueryItem()
        {
            try
            {
                List<int> listUsage = new List<int>();
                for (int i = 0; i < mListQueryConditions.Count; i++)
                {
                    var info = mListQueryConditions[i];

                    int count = info.UseCount;
                    if (!listUsage.Contains(count))
                    {
                        listUsage.Add(count);
                    }
                }
                if (listUsage.Count <= 1) { return; }       //单记录统计不出，暂时屏蔽
                mListFastQueryItems.Clear();
                for (int i = 0; i < mListQueryConditions.Count; i++)
                {
                    var info = mListQueryConditions[i];

                    FastQueryItem item = new FastQueryItem();
                    item.QueryID = info.ID;
                    item.QueryName = info.Name;
                    item.UseCount = info.UseCount;
                    item.RecordCount = info.RecordCount;
                    item.Description = string.Format("{0}\r\n{1}:{2}", item.QueryName,
                        CurrentApp.GetLanguageInfo("1206003", "Count"), item.UseCount);
                    item.Background = GetBackground(item);
                    item.Info = info;
                    mListFastQueryItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void SetViewMode()
        {
            try
            {
                if (WidgetItem == null) { return; }
                if (WidgetItem.IsFull || WidgetItem.IsCenter)
                {
                    if (WidgetItem.IsCenter)
                    {
                        Height = 380;
                    }
                }
                else
                {
                    Height = 350;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private Brush GetBackground(FastQueryItem item)
        {
            Random r = new Random((int)(item.QueryID - 2020000000000000001) + (int)DateTime.Now.Ticks);
            return
                new SolidColorBrush(Color.FromRgb((byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255)));
        }

        #endregion


        #region ItemClickCommand

        private static RoutedUICommand mItemClickCommand = new RoutedUICommand();

        public static RoutedUICommand ItemClickCommand
        {
            get { return mItemClickCommand; }
        }

        private void ItemClickCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var item = e.Parameter as FastQueryItem;
                if (item == null) { return; }
                int appID = 3102;
                string strArgs = string.Format("/FQ:{0}", item.QueryID);
                string strIcon = @"Images\S0000\S0000006.png";
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACTaskNavigateApp;
                webRequest.ListData.Add(appID.ToString());
                webRequest.ListData.Add(strArgs);
                webRequest.ListData.Add(strIcon);
                CurrentApp.PublishEvent(webRequest);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Refresh

        public void Refresh()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadQueryConditions();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateFastQueryItem();
                    SetViewMode();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListFastQueryItems.Count; i++)
                {
                    var item = mListFastQueryItems[i];

                    item.Description = string.Format("{0}\r\n{1}:{2}", item.QueryName,
                        CurrentApp.GetLanguageInfo("1206003", "Count"), item.UseCount);
                }
            }
            catch { }
        }

        #endregion

    }
}
