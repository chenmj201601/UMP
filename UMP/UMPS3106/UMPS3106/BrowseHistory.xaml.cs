using Common3106;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS3106.Wcf31061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS3106
{
    /// <summary>
    /// BrowseHistory.xaml 的交互逻辑
    /// </summary>
    public partial class BrowseHistory 
    {
        public TutorialRepertoryMainView ParentPage;
        public FilesItemInfo mFileInto;
        private ObservableCollection<FilesItemInfo> mListBrowseInfo;
        BackgroundWorker mWorker;
        public BrowseHistory()
        {
            InitializeComponent();
            mListBrowseInfo = new ObservableCollection<FilesItemInfo>();
            LvBrowse.ItemsSource = mListBrowseInfo;
            this.Loaded += BrowseHistory_Loaded;
        }

        private void BrowseHistory_Loaded(object sender, RoutedEventArgs e)
        {
            InitHistoryColumns();
            GetBrowseHistory();
        }

        private void InitHistoryColumns()
        {
            try
            {
                string[] lans = "3106T00001,3106T00003,3106T00046,3106T00047,3106T00048".Split(',');
                string[] cols = "RowNumber,FileName,UserName,BrowseTimes,BrowseTime".Split(',');
                int[] colwidths = { 60, 180, 100, 40, 160 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < cols.Count(); i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "Browse")
                    {
                        DataTemplate dt = Resources["CellBrowseTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvBrowse.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetBrowseHistory()
        {
            mListBrowseInfo.Clear();
            string strSql = string.Empty;
            if (mFileInto != null)
            {
                strSql = string.Format("SELECT * FROM T_31_059_{0} WHERE C001={1}  ORDER BY C002,C006", CurrentApp.Session.RentInfo.Token, mFileInto.FileID);
            }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.GetBrowseHistory;
                webRequest.ListData.Add(strSql);
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result) { return; }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FilesItemInfo filesItem = optReturn.Data as FilesItemInfo;
                    if (filesItem == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    filesItem.RowNumber = i + 1;                    
                    mListBrowseInfo.Add(filesItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


    }
}
