using System;
using System.Collections.Generic;
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
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// RecordScoreDetail.xaml 的交互逻辑
    /// </summary>
    public partial class RecordScoreDetail
    {

        private List<ViewColumnInfo> mListRecordScoreDetailColumns;
        public QMMainView MainPage;
        public RecordInfoItem SelectRecordInfoItem;

        //每天录音的评分详情
        private List<RecordScoreDetailClass> mRecordScoreDetail;

        public RecordScoreDetail()
        {
            InitializeComponent();
            mListRecordScoreDetailColumns = new List<ViewColumnInfo>();
            mRecordScoreDetail = new List<RecordScoreDetailClass>();
            Loaded+=RecordScoreDetail_Loaded;
        }

        private void RecordScoreDetail_Loaded(object sender, RoutedEventArgs e)
        {
            InitRecordScoreDetailColumns();
            CreateDownloadColumns();
            InitRecordScoreDetail(SelectRecordInfoItem);
            LvRecordScoreDetail.ItemsSource = mRecordScoreDetail;

            ChangeLanguage();
        }

        private void InitRecordScoreDetailColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102008");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                mListRecordScoreDetailColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListRecordScoreDetailColumns.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateDownloadColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListRecordScoreDetailColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListRecordScoreDetailColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102008{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102008{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        gv.Columns.Add(gvc);
                    }
                }
                LvRecordScoreDetail.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRecordScoreDetail(RecordInfoItem Item)
        {
            try 
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRecordScoreDetail;
                webRequest.ListData.Add(SelectRecordInfoItem.SerialID.ToString());
                webRequest.ListData.Add(SelectRecordInfoItem.Agent.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<RecordScoreDetailClass> listRecordScoreDetail = new List<RecordScoreDetailClass>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordScoreDetailClass>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordScoreDetailClass item = optReturn.Data as RecordScoreDetailClass;
                    if (item != null)
                    {
                        item.Score = (decimal.Round(Convert.ToDecimal(item.Score), 3)).ToString();
                        if (item.IsLastScore == "Y")
                        {
                            item.IsLastScore = CurrentApp.GetLanguageInfo("3102TIP0021", "Yes");
                        }
                        else
                        {
                            item.IsLastScore = CurrentApp.GetLanguageInfo("3102TIP0020", "No");
                        }
                        listRecordScoreDetail.Add(item);
                    }
                }
                mRecordScoreDetail.Clear();
                for (int i = 0; i < listRecordScoreDetail.Count; i++)
                {
                    mRecordScoreDetail.Add(listRecordScoreDetail[i]);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("310211000", "Record Score Detail");
                }

                CreateDownloadColumns();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

    }
}
