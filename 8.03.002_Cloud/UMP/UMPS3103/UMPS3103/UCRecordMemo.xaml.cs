using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS3103.Models;
using UMPS3103.Wcf11011;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;

namespace UMPS3103
{
    /// <summary>
    /// UCRecordMemo.xaml 的交互逻辑
    /// </summary>
    public partial class UCRecordMemo
    {
        public TaskScoreForm PageParent;
        public TaskInfoDetail RecordInfoItem;

        private ObservableCollection<BasicUserItem> mListUsers;
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
                TxtSerialID.Text = RecordInfoItem.RecoredReference.ToString();
                TxtStartRecordTime.Text = RecordInfoItem.TaskName;
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
                item.Display = CurrentApp.GetLanguageInfo("3103T00048", "Save");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00048", "Save");
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
                                //PageParent.ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                                //PageParent.ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, false);
                            }
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
                webRequest.Code = (int)S3103Codes.GetRecordMemoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(RecordInfoItem.RecoredReference.ToString());
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
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
                webRequest.Code = (int)S1101Codes.GetResourceProperty;
                webRequest.ListData.Add("102");
                webRequest.ListData.Add("Account");
                webRequest.ListData.Add(listUserID.Count.ToString());
                for (int i = 0; i < listUserID.Count; i++)
                {
                    webRequest.ListData.Add(listUserID[i]);
                }
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                //WebHelper.SetServiceClient(client);
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

        private void SaveRecordMemo()
        {
            try
            {
                if (string.IsNullOrEmpty(TxtMemoContent.Text))
                {
                   ShowException(CurrentApp.GetLanguageInfo("3103T00130", "Memo content is empty."));
                    return;
                }
                RecordMemoItem item = mListRecordMemos.FirstOrDefault(m => m.UserID == CurrentApp.Session.UserID);
                if (item == null)
                {
                    RecordMemoInfo info = new RecordMemoInfo();
                    info.ID = 0;
                    info.RecordSerialID = RecordInfoItem.RecoredReference;
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
                webRequest.Code = (int)S3103Codes.SaveRecordMemoInfo;
                webRequest.ListData.Add(RecordInfoItem.RecoredReference.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(strMemoInfo);
                Service31031Client client = new Service31031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), RecordInfoItem.RecoredReference);
                CurrentApp.WriteOperationLog(S3103Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                CurrentApp.WriteLog(string.Format("Save RecordMemo end.\t{0}", webReturn.Data));
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00129", "Save RecordMemo end."));
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
            try
            {
                base.ChangeLanguage();

                TabMyMemo.Header = CurrentApp.GetLanguageInfo("3103T00049", "My Remarks:");
                TabOtherMemo.Header = CurrentApp.GetLanguageInfo("3103T00050", "Other Remarks:");
            }
            catch { }
        }
    }
}
