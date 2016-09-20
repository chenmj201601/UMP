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
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.UMP.Common;
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
        public UserControl UC;

        public UCQueryCondition PageParent;

        private ObjectItem mRootItem;

        public List<ObjectItem> mListSelectedObjects_KeyWords;

        private BackgroundWorker mWorker;

        public KeyWordList()
        {
            InitializeComponent();
            mRootItem = new ObjectItem();
            mListSelectedObjects_KeyWords = new List<ObjectItem>();
            

            Loaded += KeyWordList_Loaded;
        }

        private void KeyWordList_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
            KeyWordTree.ItemsSource = mRootItem.Children;
            mRootItem.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) => { InitKeyWords(); };
            mWorker.RunWorkerCompleted += (s, re) => 
            {
                mWorker.Dispose();
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

        private void InitKeyWords(ObjectItem parentItem, string parentID)
        {
            try 
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetKeyWordsInfo;
                //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                //webRequest.ListData.Add(parentID);
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
                for (int i = 0; i < webReturn.ListData.Count; i++) 
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}", strName);
                    AddChildObject(parentItem, item);
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

        private void GetCheckedInspector(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.IsChecked == true)
                        {
                            mListSelectedObjects_KeyWords.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 其他
        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));//这句话得理解下 
        }
        #endregion

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedKeyWords();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            var ucItem = UC as UCConditionItem;
            ucItem.CurrentApp = CurrentApp;
            if (ucItem != null)
            {
                ucItem.DoOperation_(mListSelectedObjects_KeyWords);
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


    }
}
