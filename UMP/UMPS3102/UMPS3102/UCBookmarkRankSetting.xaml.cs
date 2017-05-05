//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dcbfa77e-e944-42d2-b462-cf4cb2e6102b
//        CLR Version:              4.0.30319.18444
//        Name:                     UCBookmarkRankSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCBookmarkRankSetting
//
//        created by Charley at 2014/12/12 10:36:38
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;

namespace UMPS3102
{
    /// <summary>
    /// UCBookmarkRankSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCBookmarkRankSetting
    {
        private ObservableCollection<BookmarkRankItem> mListBookmarkRankItems; 

        public UCBookmarkRankSetting()
        {
            InitializeComponent();

            mListBookmarkRankItems = new ObservableCollection<BookmarkRankItem>();

            Loaded += UCBookmarkRankSetting_Loaded;
            ListBoxBookmarkRanks.SelectionChanged += ListBoxBookmarkRanks_SelectionChanged;
            BtnAdd.Click += BtnAdd_Click;
            BtnRemove.Click += BtnRemove_Click;
            BtnColorPicker.SelectedColorChanged += BtnColorPicker_SelectedColorChanged;
            TxtName.TextChanged += TxtName_TextChanged;
        }

        void UCBookmarkRankSetting_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxBookmarkRanks.ItemsSource = mListBookmarkRankItems;

            LoadBookmarkRandItems();
        }

        #region Init and Load

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
                    item.OrderID = mListBookmarkRankItems.Count;
                    mListBookmarkRankItems.Add(item);
                }
                SetOrderID();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            try
            {
                var item = ListBoxBookmarkRanks.SelectedItem as BookmarkRankItem;
                if (item != null)
                {
                    item.Color = BtnColorPicker.SelectedColor.ToString().Substring(3);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtName_TextChanged(object sender, TextChangedEventArgs e)//文本框内容更改时发生的事件~
        {
            try
            {
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    return;
                }

                if (TxtName.Text.ToString().Length > 32)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N036", string.Format("Save the content is too long")));
                    return;
                }
                var item = ListBoxBookmarkRanks.SelectedItem as BookmarkRankItem;
                if (item != null)
                {
                    item.Name = TxtName.Text;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = ListBoxBookmarkRanks.SelectedItem as BookmarkRankItem;
                if (item != null)
                {
                    mListBookmarkRankItems.Remove(item);
                    SetOrderID();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    //注释原有代码
                    //ShowException(string.Format("Name is empty"));
                    //return;


                    //============更改了提示框的，并且为提示信息添加语言=======================================
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N018", string.Format("Input is null"))); 
                    return;
                    //===============================by 俞强波==================================================
                }
                if(TxtName.Text.ToString().Length>32)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N036", string.Format("Save the content is too long")));
                    return;
                }
                OperationReturn optReturn = GetBookmarkRankID();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                long id = Convert.ToInt64(optReturn.Data);
                BookmarkRankInfo info=new BookmarkRankInfo();
                info.ID = id;
                info.Name = TxtName.Text;
                info.Color = BtnColorPicker.SelectedColor.ToString().Substring(3);
                info.RankType = "BM";
                BookmarkRankItem item = new BookmarkRankItem(info);
                item.OrderID = mListBookmarkRankItems.Count;
                mListBookmarkRankItems.Add(item);
                ListBoxBookmarkRanks.SelectedItem = item;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListBoxBookmarkRanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ListBoxBookmarkRanks.SelectedItem as BookmarkRankItem;
                if (item != null)
                {
                    if (TxtName.Text.ToString().Length > 32)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N036", string.Format("Save the content is too long")));
                        return;
                    }

                    LbSortID.Text = item.OrderID.ToString();
                    TxtName.Text = item.Name;
                    BtnColorPicker.SelectedColor = Utils.GetColorFromRgbString(item.Color);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        public OperationReturn SaveConfig()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<BookmarkRankInfo> listItems = new List<BookmarkRankInfo>();
                for (int i = 0; i < mListBookmarkRankItems.Count; i++)
                {
                    BookmarkRankItem item = mListBookmarkRankItems[i];
                    item.SetValues();
                    if (item.BookmarkRankInfo != null)
                    {
                        listItems.Add(item.BookmarkRankInfo);
                    }
                }
                if (listItems.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3102Codes.SaveBookmarkRankInfo;
                    webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    int count = listItems.Count;
                    webRequest.ListData.Add(count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(listItems[i]);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31021Client client = new Service31021Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                }
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


        #region Others

        private OperationReturn GetBookmarkRankID()
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
                webRequest.ListData.Add("309");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                //webRequest.ListData.Add(DateTime.Now.ToString("20151120123025"));

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

        private void SetOrderID()
        {
            try
            {
                for (int i = 0; i < mListBookmarkRankItems.Count; i++)
                {
                    mListBookmarkRankItems[i].OrderID = i;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
    }
}
