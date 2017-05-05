using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Common5101;
using UMPS5101.Wcf51011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS5101
{
    /// <summary>
    /// EditKwInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class EditKwInfoPage
    {
        #region Menbers

        public MainView ParentPage;
        private readonly ObservableCollection<KwContentInfoParam> _mObservableKwConnectInfo;
        private List<KwContentInfoParam> _mlstConnectInfo;
        private List<KwContentInfoParam> _mlstConnectInfoTemp;
        private List<AssociationTableParam> _mlstAssociationTableParams; 
        private BackgroundWorker _mWorker;
        private long _mLongKwNum;

        #endregion

        public EditKwInfoPage()
        {
            InitializeComponent();
            _mObservableKwConnectInfo = new ObservableCollection<KwContentInfoParam>();
            _mlstAssociationTableParams = new List<AssociationTableParam>();
            _mLongKwNum = S5101App.GLongKeywordNum;
            _mlstConnectInfo = new List<KwContentInfoParam>();
            _mlstConnectInfoTemp = new List<KwContentInfoParam>();
            Loaded += UCCustomSetting_Loaded;
        }

        private void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            InitListView();
        }

        private void Init()
        {
            ListInfoLoaded();
        }

        private void ListInfoLoaded()
        {
            _mObservableKwConnectInfo.Clear();
            try
            {
                _mWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {

                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    KwConnectListView.ItemsSource = _mObservableKwConnectInfo;
                    GetAllKwCon(SetSql_5108());
                    _mWorker.Dispose();
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception e)
            {
                ShowException( e.Message );
            }
        }

        private void InitListView()
        {
            try
            {
                string[] lans = "5101T00036,5101T00037".Split(',');
                string[] cols = "BEnable,StrKwContent".Split(',');
                int[] colwidths = { 70, 400 };
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    DataTemplate dt;
                    var gvc = new GridViewColumn();
                    if (cols[i] == "BEnable")
                    {
                        dt = Resources["CellCheckBoxTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                KwConnectListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            TbSelectContent.Text = CurrentApp.GetLanguageInfo("5101T00065", "Select Connect");
            ButClose.Content = CurrentApp.GetLanguageInfo("5101T00034", "Close");
        }

        private string SetSql_5108()
        {
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    sql = string.Format("select * from T_51_008 where C002 = 0 or C002 = {0}", _mLongKwNum);
                    break;
                case S5101App.GOracle:
                    sql = string.Format("select * from T_51_008 where C002 = 0 or C002 = {0}", _mLongKwNum);
                    break;
            }
            return sql;
        }

        private void GetAllKwCon(string strSql)
        {
            try
            {
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptSelectAssignKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}: ,{1}",
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KwContentInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }
                    _mlstConnectInfo.Add(param);
                    _mlstConnectInfoTemp.Add(param);
                }
                SetKwContentTable();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetKwContentTable()
        {
            _mObservableKwConnectInfo.Clear();
            foreach (var param in _mlstConnectInfo)
            {
                param.IsCheckedTrue = param.IntBindingKw != 0;
                _mObservableKwConnectInfo.Add(param);
            }
        }

        private void ButClose_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbSelectContent1.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                    MessageBoxButton.OK);
                    return;
                }

                _mlstAssociationTableParams.Clear();
                if (_mObservableKwConnectInfo.Count > 0)
                {
                    foreach (var param in _mObservableKwConnectInfo)
                    {
                        AssociationTableParam assparam = new AssociationTableParam();
                        assparam.IntEnable = param.IsCheckedTrue ? 1 : 0;
                        assparam.LongKwConNum = param.LongKwContentNum;
                        assparam.LongKwNum = param.IsCheckedTrue ? _mLongKwNum : 0;
                        _mlstAssociationTableParams.Add(assparam);
                    }
                }

                if(!Update51008())
                    return;

                var parant = Parent as PopupPanel;
                if (parant != null)
                {
                    parant.IsOpen = false;
                }
            }
            catch (Exception ex )
            {
                ShowException(ex.Message);
            }
        }

        private bool Update51008()
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptUpdate51008;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mlstAssociationTableParams);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void TbSelectContent1_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TbSelectContent1.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                    MessageBoxButton.OK);
                }

                _mlstConnectInfo.Clear();
                foreach (var kwContentInfoParam in _mlstConnectInfoTemp)
                {
                    if (kwContentInfoParam.StrKwContent.IndexOf(TbSelectContent1.Text) >= 0)
                    {
                        _mlstConnectInfo.Add(kwContentInfoParam);
                    }
                }
                SetKwContentTable();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
