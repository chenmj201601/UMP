using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// KeyWordList.xaml 的交互逻辑
    /// </summary>
    public partial class KeyWordList
    {

        #region Members

        public UserControl UC;
        public UCQueryCondition PageParent;
        public List<KeywordItem> mListSelectedObjects_KeyWords;

        private bool mIsInited;
        private BackgroundWorker mWorker;
        private KeywordItem mRootItem;
        private List<KeywordInfo> mListKeywordInfos;
        private List<KeywordItem> mListKeywordItems; 

        #endregion
    

        public KeyWordList()
        {
            InitializeComponent();

            mRootItem = new KeywordItem();
            mListSelectedObjects_KeyWords = new List<KeywordItem>();
            mListKeywordInfos = new List<KeywordInfo>();
            mListKeywordItems = new List<KeywordItem>();

            Loaded += KeyWordList_Loaded;
        }

        private void KeyWordList_Loaded(object sender, RoutedEventArgs e)
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
                KeyWordTree.ItemsSource = mRootItem.Children;
                mRootItem.Children.Clear();
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    LoadKeywordInfos();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();

                    CreateKeywordItems();
                    mRootItem.IsChecked = false;
                    mRootItem.IsExpanded = true;
                    if (mRootItem.Children.Count > 0)
                    {
                        mRootItem.Children[0].IsExpanded = true;
                    }

                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadKeywordInfos()
        {
            try
            {
                mListKeywordInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetKeyWordsInfo;
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
                WebHelper.SetServiceClient(client);
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

        #endregion


        #region Create

        private void CreateKeywordItems()
        {
            try
            {
                mRootItem.Children.Clear();
                mListKeywordItems.Clear();
                //所有关键词
                KeywordItem allItem = new KeywordItem();
                allItem.ObjID = 0;
                allItem.Name = CurrentApp.GetLanguageInfo("3102001", "All Keyword");
                allItem.Description = CurrentApp.GetLanguageInfo("3102001", "All Keyword");
                mRootItem.AddChild(allItem);
                mListKeywordItems.Add(allItem);
                var keywords = mListKeywordInfos.GroupBy(k => k.SerialNo);
                foreach (var keyword in keywords)
                {
                    KeywordItem item = new KeywordItem();
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
                    item.Name = strName;
                    item.Description = string.Format("{0} ({1}) ", strName, strDesc);
                    allItem.AddChild(item);
                    mListKeywordItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

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

        private void GetCheckedInspector(KeywordItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    KeywordItem item = parentItem.Children[i] as KeywordItem;
                    if (item != null)
                    {
                        if (item.IsChecked == true && item.ObjID > 0)
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

        #endregion


        #region Event Handlers

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetCheckedKeyWords();
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }

                var ucItem = UC as UCConditionItem;
                if (ucItem == null) { return; }
                ucItem.CurrentApp = CurrentApp;
                ucItem.DoOperation_(mListSelectedObjects_KeyWords);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");

                for (int i = 0; i < mListKeywordItems.Count; i++)
                {
                    var item = mListKeywordItems[i];
                    //所有关键词
                    if (item.ObjID == 0)
                    {
                        item.Name = CurrentApp.GetLanguageInfo("3102001", "All Keyword");
                        item.Description = CurrentApp.GetLanguageInfo("3102001", "All Keyword");
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
