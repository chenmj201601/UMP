//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    66f3ae69-c11c-4eaa-9249-af2c56c01ce2
//        CLR Version:              4.0.30319.18444
//        Name:                     UCRecordMemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCRecordMemo
//
//        created by Charley at 2014/11/14 14:35:18
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
    /// UCRecordMemo.xaml 的交互逻辑
    /// </summary>
    public partial class UCRecordMemo
    {
        public QMMainView PageParent;
        public RecordInfoItem RecordInfoItem;

        private ObservableCollection<BasicUserItem> mListUsers;
        
        //这个是录音备注的List，里面存有选中录音的所有备注的内容
        private List<RecordMemoItem> mListRecordMemos;
        
        private RecordMemoItem mThisRecordMemo;

        public UCRecordMemo()
        {
            InitializeComponent();

            mListUsers = new ObservableCollection<BasicUserItem>();
            mListRecordMemos = new List<RecordMemoItem>();

            Loaded += UCRecordMemo_Loaded;
        }

        void UCRecordMemo_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxMemoUsers.SelectionChanged += ListBoxMemoUsers_SelectionChanged;

            ListBoxMemoUsers.ItemsSource = mListUsers;

            CreateToolButton();
            if (RecordInfoItem != null)
            {
                TxtSerialID.Text = RecordInfoItem.SerialID.ToString();
                TxtStartRecordTime.Text = RecordInfoItem.StartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            LoadRecordMemoData();
            InitMemoUsers();
            SetMemoValue();

            ChangeLanguage();
        }

        private void CreateToolButton()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "BT" + "Save";
                item.Display = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Tip = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Icon = "Images/save.png";
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

        private void ToolButton_Click(object sender, RoutedEventArgs e)
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
                            SaveRecordMemo();
                            if (PageParent != null)
                            {
                                PageParent.ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                                PageParent.ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, false);
                            }
                            SetMemoInfoToT_21_001();
                            break;
                    }
                }
            }
        }

        void ListBoxMemoUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var basicUserItem = ListBoxMemoUsers.SelectedItem as BasicUserItem;
            if (basicUserItem != null)
            {
                var memoItem = mListRecordMemos.FirstOrDefault(m => m.UserID == basicUserItem.UserID);
                if (memoItem != null)
                {
                    TxtOtherMemoContent.Text = memoItem.Content;
                }
            }
        }

        private void LoadRecordMemoData()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetRecordMemoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mListRecordMemos.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordMemoInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordMemoInfo info = optReturn.Data as RecordMemoInfo;
                    if (info != null)
                    {
                        RecordMemoItem item = new RecordMemoItem(info);
                        if (item.UserID == CurrentApp.Session.UserID)
                        {
                            mThisRecordMemo = item;
                        }
                        mListRecordMemos.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitMemoUsers()
        {
            try
            {
                List<string> listUserID = new List<string>();
                for (int i = 0; i < mListRecordMemos.Count; i++)
                {
                    string strUserID = mListRecordMemos[i].UserID.ToString();
                    if (!listUserID.Contains(strUserID))
                    {
                        listUserID.Add(strUserID);
                    }
                }
                if (listUserID.Count <= 0) { return; }
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
                mListUsers.Clear();
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
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        //这个只是将备注其存到T_31_046备注表
        private void SaveRecordMemo()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtMemoContent.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N037" ,string.Format("Memo content is empty")));
                    return;
                }
                if (TxtMemoContent.Text.ToString().Length > 1024)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N036", "Save the content is too long"));
                    return;
                }
                RecordMemoItem item = mListRecordMemos.FirstOrDefault(m => m.UserID == CurrentApp.Session.UserID);
                if (item == null)
                {
                    RecordMemoInfo info = new RecordMemoInfo();
                    info.ID = 0;
                    info.RecordSerialID = RecordInfoItem.SerialID;
                    info.UserID = CurrentApp.Session.UserID;
                    info.MemoTime = DateTime.Now;
                    info.Content = TxtMemoContent.Text;
                    info.State = "1";
                    info.LastModifyUserID = CurrentApp.Session.UserID;
                    info.LastModifyTime = DateTime.Now;
                    info.Source = "U";
                    item = new RecordMemoItem(info);
                }
                else
                {
                    RecordMemoInfo info = item.RecordMemoInfo;
                    info.Content = TxtMemoContent.Text;
                    info.LastModifyUserID = CurrentApp.Session.UserID;
                    info.LastModifyTime = DateTime.Now;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(item.RecordMemoInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strMemoInfo = optReturn.Data.ToString();

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveRecordMemoInfo;
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(strMemoInfo);
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
                CurrentApp.WriteLog(string.Format("Save RecordMemo end.\t{0}", webReturn.Data));

                #region 写操作日志

                string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102001RecordReference"), item.RecordSerialID);
                strLog += string.Format("{0}", item.Content);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("003", "Save RecordMemo end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //现在我新建一个方法 将备注存到T_21_001的C072字段里面
        private void SetMemoInfoToT_21_001()
        {
            try 
            {
                //这个是存完备注之后，在到备注表里面读取最后的备注再存到下面这里面
                List<RecordMemoItem> mListRecordMemos_new;
                mListRecordMemos_new = new List<RecordMemoItem>();

                if (RecordInfoItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetRecordMemoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mListRecordMemos_new.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordMemoInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordMemoInfo info = optReturn.Data as RecordMemoInfo;
                    if (info != null)
                    {
                        RecordMemoItem item = new RecordMemoItem(info);
                        mListRecordMemos_new.Add(item);
                    }
                }
                string TempMemo = string.Empty;
                for (int i = 0; i < mListRecordMemos_new.Count; i++)
                {
                    if (TempMemo.Length>500)
                    {
                        break;
                    }
                    if (i < mListRecordMemos_new.Count-1)
                    {
                        TempMemo += mListRecordMemos_new[i].UserID + ConstValue.SPLITER_CHAR_2 + mListRecordMemos_new[i].Content + ConstValue.SPLITER_CHAR;
                    }
                    else
                    {
                        TempMemo += mListRecordMemos_new[i].UserID + ConstValue.SPLITER_CHAR_2 + mListRecordMemos_new[i].Content;
                    }
                }

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
                SetMemoInfoToT_21_001(RecordInfoItem.SerialID.ToString(), TempMemo, queryStateInfo.TableName);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetMemoInfoToT_21_001(string SerialID, string MemoValue, string TableName)
        {
            try 
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.SaveMemoInfoToT_21_001;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SerialID);
                webRequest.ListData.Add(MemoValue);
                webRequest.ListData.Add(TableName);
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
                ShowException(ex.Message);
            }
        }

        private void SetMemoValue()
        {
            try
            {
                if (mThisRecordMemo == null) { return; }
                TxtMemoContent.Text = mThisRecordMemo.Content;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            TabMyMemo.Header = CurrentApp.GetLanguageInfo("31021010", "My Memo");
            TabOtherMemo.Header = CurrentApp.GetLanguageInfo("31021011", "Other Memo");
        }
    }
}
