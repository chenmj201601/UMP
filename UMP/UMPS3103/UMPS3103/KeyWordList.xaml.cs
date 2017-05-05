using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3103
{
    /// <summary>
    /// KeyWordList.xaml 的交互逻辑
    /// </summary>
    public partial class KeyWordList
    {
        public UserControl UC;
        public QueryRecordInTask PageParent;
       
        // private ObjectItem mRootItem;

        public ObjectItemTask mRootItem;

        public List<ObjectItemTask> mListSelectedObjects_KeyWords;
        public List<KeywordInfo> mListKeywordInfos;
        private BackgroundWorker mWorker;
        public  List<GetKeyWordInfo> mListGeyKeyWordInfo;

        public KeyWordList()
        {
            InitializeComponent();
            mRootItem = new ObjectItemTask();
            mListSelectedObjects_KeyWords = new List<ObjectItemTask>();
            mListKeywordInfos = new List<KeywordInfo>();

            Loaded += KeyWordList_Loaded;
        }

        private void KeyWordList_Loaded(object sender, RoutedEventArgs e)
        {
            mListGeyKeyWordInfo = new List<GetKeyWordInfo>();
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
            KeyWordTree.ItemsSource = mRootItem.Children;
            mRootItem.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) => { InitKeyWords(); };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                CreateKeywordItems();
                mRootItem.IsChecked = false;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();
        }

        private void InitKeyWords()
        {
            InitKeyWords(mRootItem, "-1");
        }

        private void InitKeyWords(ObjectItemTask parentItem, string parentID)
        {
            try
            {
                mListKeywordInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.GetKeyWords;

             //   Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31031"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
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
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<KeywordInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    KeywordInfo info = optReturn.Data as KeywordInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("KeywordInfo is null."));
                        return;
                    }
                    mListKeywordInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadKeywordInfos", string.Format("End.\t{0}", mListKeywordInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateKeywordItems()
        {
            try
            {
                mRootItem.Children.Clear();
                //所有关键词
                ObjectItemTask allItem = new ObjectItemTask();
                allItem.ObjID = 0;
                allItem.Name = string.Format("All Keyword");
                allItem.Description = string.Format("All Keyword");
                mRootItem.AddChild(allItem);
                var keywords = mListKeywordInfos.GroupBy(k => k.SerialNo);
                foreach (var keyword in keywords)
                {
                    ObjectItemTask item = new ObjectItemTask();
                    long serialNo = 0;
                    string strName = string.Empty;
                    string strDesc = string.Empty;
                    foreach (var content in keyword)
                    {
                        serialNo = content.SerialNo;
                        strName = content.Name;
                        strDesc += string.Format("{0};", content.Content);
                        item.ListKeywordInfos.Add(content);
                   
                    }
                    item.ObjID = serialNo;
                    item.Name = string.Format("{0} ({1}) ", strName, strDesc);
                    item.Description = strName;
                    allItem.AddChild(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void GetCheckedKeyWords()
        {
            try
            {
                mListSelectedObjects_KeyWords.Clear();
                GetCheckedInspector(mRootItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetCheckedInspector(ObjectItemTask parentItem)
        {
           
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItemTask item = parentItem.Children[i] as ObjectItemTask;
                    if (item != null)
                    {
                        if (item.IsChecked == true&&item.ObjID>0)
                        {
                            mListSelectedObjects_KeyWords.Add(item);
                           
                        }
                        GetCheckedInspector(item);
                    }                  
                }                                           
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }           
        }

        #region 其他
        private void AddChildObject(ObjectItemTask parentItem, ObjectItemTask item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));//这句话得理解下 
        }
        #endregion
        string strValue01=string.Empty;
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedKeyWords();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
            for (int j = 0; j < mListSelectedObjects_KeyWords.Count; j++)
            {
                GetKeyWordInfo Ginfo = new GetKeyWordInfo();
                Ginfo.Description = mListSelectedObjects_KeyWords[j].Description;
                Ginfo.Name = mListSelectedObjects_KeyWords[j].Name;
                Ginfo.ObjID = mListSelectedObjects_KeyWords[j].ObjID;
                mListGeyKeyWordInfo.Add(Ginfo);
            }
            getName(strValue01);
           
        }

        public void getName(string strValue01)
        {
            if (mListGeyKeyWordInfo != null && mListGeyKeyWordInfo.Count > 0)
            {
                for (int i = 0; i < mListGeyKeyWordInfo.Count; i++)
                {
                    var item = mListGeyKeyWordInfo[i];
                    strValue01 += string.Format("'{0}';", item.Description);
                }
            }
            strValue01 = strValue01.TrimEnd(';');
        }
    
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }


    }
}
