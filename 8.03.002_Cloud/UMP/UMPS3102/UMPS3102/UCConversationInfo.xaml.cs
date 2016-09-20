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
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;

namespace UMPS3102
{
    /// <summary>
    /// UCConversationInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UCConversationInfo
    {

        public QMMainView PageParent;

        //当前在播放的录音
        public RecordInfoItem PlayingRecord;
        //当前录音的会话信息
        private List<ConversationInfo> mListConversationInfo;

        //这里要用这个属性的List
        public ObservableCollection<ConversationInfoItem> mListConversationInfoItem;

        //private List<ListBoxItem> mListBoxItem;



        public UCConversationInfo()
        {
            InitializeComponent();
            //mListBoxItem = new List<ListBoxItem>();
            mListConversationInfo = new List<ConversationInfo>();
            mListConversationInfoItem = new ObservableCollection<ConversationInfoItem>();
            Loaded += UCConversationInfo_Loaded;
        }

        private void UCConversationInfo_Loaded(object sender, RoutedEventArgs e)
        {
            ConversationContentListBox.ItemsSource = mListConversationInfoItem;
            if (PlayingRecord == null)
            {
                return;
            }
            RefID.Content = PlayingRecord.RecordReference;
            InitConversationInfo();
            ChangeLanguage();
        }


        private void InitConversationInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetConversationInfo;
                webRequest.ListData.Add(PlayingRecord.RecordReference.ToString());
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
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<ConversationInfoItem> listTemp = new List<ConversationInfoItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ConversationInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ConversationInfo info = optReturn.Data as ConversationInfo;
                    ConversationInfoItem infoItem = new ConversationInfoItem(info);

                    if (infoItem.Direction == "0")
                    {
                        infoItem.Icon = "Images/0.ico";
                        if (PlayingRecord.Direction == 0)
                        {
                            infoItem.Extension = PlayingRecord.CallerID;
                        }
                        if (PlayingRecord.Direction == 1)
                        {
                            infoItem.Extension = PlayingRecord.CalledID;
                        }
                    }
                    if (infoItem.Direction == "1")
                    {
                        infoItem.Icon = "Images/1.ico";
                        if (PlayingRecord.Direction == 0)
                        {
                            infoItem.Extension = PlayingRecord.CalledID;
                        }
                        if (PlayingRecord.Direction == 1)
                        {
                            infoItem.Extension = PlayingRecord.CallerID;
                        }
                    }

                    if (infoItem.SerialID == "0")
                    {
                        infoItem.IsVisible = false;
                    }
                    else
                    {
                        infoItem.IsVisible = true;
                    }
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tConversationInfo is null"));
                        return;
                    }
                    listTemp.Add(infoItem);
                }
                listTemp = listTemp.OrderBy(t => t.Offset).ToList();
                for (int i = 0; i < listTemp.Count; i++)
                {
                    mListConversationInfoItem.Add(listTemp[i]);
                }
                //mListConversationInfoItem = listTemp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 将ListboxItem跳到当前录音播放的位置
        /// </summary>
        public void SkipToCurrent(double pos)
        {
            for (int i = 0; i < mListConversationInfoItem.Count; i++)
            {
                mListConversationInfoItem[i].Color = "FFFAFA";
                if (i == mListConversationInfoItem.Count - 1)
                {
                    if (pos >= mListConversationInfoItem[i].Offset)
                    {
                        mListConversationInfoItem[i].Color = "FFD700";
                        ConversationContentListBox.ScrollIntoView(mListConversationInfoItem[i]);
                    }
                }
                else
                {
                    if (pos >= mListConversationInfoItem[i].Offset && pos < mListConversationInfoItem[i + 1].Offset)
                    {
                        mListConversationInfoItem[i].Color = "FFD700";
                        ConversationContentListBox.ScrollIntoView(mListConversationInfoItem[i]);
                    }
                    else
                    {
                        if (mListConversationInfoItem[i].Color.Equals("FFD700"))
                        { 
                            mListConversationInfoItem[i].Color = "#FFFAFA"; 
                        }
                    }
                }
            }

        }

        #region ChangeLangugage
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                TxtRecordName.Text = CurrentApp.GetLanguageInfo("3102C3031401010000000009", "RecordLiuShuiHao");
            }
            catch { }
        }
        #endregion

    }
}
