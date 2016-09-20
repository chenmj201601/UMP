using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Common3603;
using UMPS3603.Wcf36031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;


namespace UMPS3603
{
    /// <summary>
    /// PaperInfo.xaml 的交互逻辑
    /// </summary>
    public partial class PaperInfo
    {
        public ExamProductionView ParentPage;
        private BackgroundWorker _mWorker;
        private readonly ObservableCollection<CPaperParam> _mObservableCollectionPaperParams;
        private readonly List<CPaperParam> _mListPaperInfos;
        private CPaperParam _mPaperParamTemp;

        public PaperInfo()
        {
            _mWorker = new BackgroundWorker();
            _mObservableCollectionPaperParams = new ObservableCollection<CPaperParam>();
            _mListPaperInfos = new List<CPaperParam>();
            _mPaperParamTemp = null;
            
            InitializeComponent();
            Loaded += PapersListInfo_Loaded;
            Loaded += UCCustomSetting_Loaded;
            
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            SearchInfoName.Visibility = Visibility.Collapsed;
            Button.Content = "<";
            DtStartTime.Value = DateTime.Today;
            DtEndTime.Value = DateTime.Now;
            ChangeLanguage();
            InitPaperListTable();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            ChkPaperName.Content = CurrentApp.GetLanguageInfo("3603T00027", "Paper Name");
            ChkPaperNum.Content = CurrentApp.GetLanguageInfo("3603T00028", "Paper Num");
            ChkPaperType.Content = CurrentApp.GetLanguageInfo("3603T00029", "Paper Type");
            ChkIntegrity.Content = CurrentApp.GetLanguageInfo("3603T00030", "Integrity");
            ComBoxIntegrity1.Content = CurrentApp.GetLanguageInfo("3603T00072", "Y");
            ComBoxIntegrity2.Content = CurrentApp.GetLanguageInfo("3603T00073", "N");
            TxStartTime.Text = CurrentApp.GetLanguageInfo("3603T00085", "Create Time");
            TxEndTime.Text = CurrentApp.GetLanguageInfo("3603T00032", "To");
            ButSearchPaper.Content = CurrentApp.GetLanguageInfo("3603T00033", "Search");
            BtnAddPaper.Content = CurrentApp.GetLanguageInfo("3603T00034", "Add");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3603T00035", "Close");
            PaperInfoTable.Title = CurrentApp.GetLanguageInfo("3603T00036", "Paper");
            CmbPaperType.SelectedIndex = -1;
        }

        private void InitPaperType(PaperType paperType)
        {
            TypeItem1.Content = CurrentApp.GetLanguageInfo("3603T00103", "1");
            TypeItem2.Content = CurrentApp.GetLanguageInfo("3603T00104", "2");
            TypeItem3.Content = CurrentApp.GetLanguageInfo("3603T00105", "3");
            TypeItem4.Content = CurrentApp.GetLanguageInfo("3603T00106", "4");
            TypeItem5.Content = CurrentApp.GetLanguageInfo("3603T00107", "5");
            CmbPaperType.SelectedIndex = (int) paperType;
        }

        void InitPaperListTable()
        {
            try
            {
                string[] lans = "3603T00037,3603T00038,3603T00039,3603T00040,3603T00041,3603T00042,3603T00043,3603T00044,3603T00045,3603T00046,3603T00047".Split(',');
                string[] cols = "LongNum,StrName,StrDescribe,StrType,IntQuestionNum,IntScores,IntPassMark,IntTestTime,StrUsed,IntAudit,StrIntegrity".Split(',');
                int[] colwidths = {150,150, 150, 100, 60, 180, 80, 100, 100, 100, 100 };
                var columnGridView = new GridView();

                for (int i = 0; i < lans.Length; i++)
                {
                    var gvc = new GridViewColumn
                    {
                        Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]),
                        Width = colwidths[i],
                        DisplayMemberBinding = new Binding(cols[i])
                    };
                    columnGridView.Columns.Add(gvc);
                }
                PaperInfoListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PapersListInfo_Loaded(object sender, RoutedEventArgs e)
        {
            _mObservableCollectionPaperParams.Clear();
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
                    //GetUmpSetupPath();
                    //InitCategoryTreeInfo();
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    PaperInfoListTable.ItemsSource = _mObservableCollectionPaperParams;
                    GetAllPaperInfos();
                    _mWorker.Dispose();
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void GetAllPaperInfos()
        {
            try
            {
                _mListPaperInfos.Clear();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetPapers;

                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00015", "Insert data failed"));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                foreach (string strDate in webReturn.ListData)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CPaperParam>(strDate);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamPapers = optReturn.Data as CPaperParam;
                    if (cExamPapers == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mListPaperInfos.Add(cExamPapers);
                }
                SetObservableCollection(_mListPaperInfos);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PaperInfoTable_IsSelectedChanged(object sender, EventArgs e)
        {

        }

        void PapersListTable_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mPaperParamTemp = null;
                S3603App.GPaperInfoParamo = new PaperInfoParam();
                var item = PaperInfoListTable.SelectedItem as CPaperParam;
                if (item != null)
                {
                    _mPaperParamTemp = item;
                    S3603App.GPaperInfoParamo.PaperParam = _mPaperParamTemp;
                    S3603App.GPaperInfoParamo.OptType = S3603Codes.OptAddInfo;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PapersListTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mPaperParamTemp = null;
                var itema = PaperInfoListTable.SelectedItem as CPaperParam;
                if (itema != null)
                {
                    S3603App.GQueryModify = false;
                    S3603App.GPaperParam = new CPaperParam();
                    S3603App.GPaperParam = itema;
                    _mPaperParamTemp = itema;
                    S3603App.GPaperInfoParamo.PaperParam = _mPaperParamTemp;
                    TestPaperPage newPage = new TestPaperPage
                    {
                        ExamProductionParentPage = ParentPage,
                        CurrentApp = CurrentApp
                    };
                    ParentPage.PopupPaperInfo.Content = newPage;
                    ParentPage.PopupPaperInfo.Title = CurrentApp.GetLanguageInfo("3603T00055", "Paper Information");
                    ParentPage.PopupPaperInfo.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddPaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_mPaperParamTemp.IntIntegrity == 0)
                {
                    MessageBox.Show(
                        CurrentApp.GetLanguageInfo("3603T00048",
                            "Paper is incomplete and cannot be added to the test!"),
                        CurrentApp.GetLanguageInfo("3603T00049", "Warning"),
                        MessageBoxButton.OKCancel);
                    return;
                }
                if (_mPaperParamTemp != null)
                {
                    ParentPage.OpenTestInfo();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SetObservableCollection(List<CPaperParam> listPaperInfos )
        {
            _mObservableCollectionPaperParams.Clear();
            foreach (var param in listPaperInfos)
            {
                param.StrIntegrity = param.IntIntegrity == 1 ? CurrentApp.GetLanguageInfo("3603T00072", "Y") : CurrentApp.GetLanguageInfo("3603T00073", "N");

                switch (param.CharType)
                {
                    case '1':
                        param.StrType = CurrentApp.GetLanguageInfo("3603T00067", "1");
                        break;
                    case '2':
                        param.StrType = CurrentApp.GetLanguageInfo("3603T00068", "2");
                        break;
                    case '3':
                        param.StrType = CurrentApp.GetLanguageInfo("3603T00069", "3");
                        break;
                    case '4':
                        param.StrType = CurrentApp.GetLanguageInfo("3603T00070", "4");
                        break;
                    case '5':
                        param.StrType = CurrentApp.GetLanguageInfo("3603T00071", "5");
                        break;
                }

                param.StrUsed = param.IntUsed == 1 ? CurrentApp.GetLanguageInfo("3603T00074", "Y") : CurrentApp.GetLanguageInfo("3603T00075", "N");

                _mObservableCollectionPaperParams.Add(param);
            }
        }

        private string SetSearchInfo()
        {
            string sql = string.Format("SELECT * FROM T_36_023_{0} WHERE ", CurrentApp.Session.RentInfo.Token);

            if (ChkPaperNum.IsChecked == false)
            {
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        sql += string.Format("c013 >= '{0}' and c013 <= '{1}' ", DtStartTime.Value.ToString(), DtEndTime.Value.ToString());
                        break;
                    case 3:
                        sql += string.Format(" c013 between to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", DtStartTime.Value.ToString(), DtEndTime.Value.ToString());
                        break;
                }
            }
            
            if (ChkPaperName.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(TbSearchPaper.Text))
                {
                    sql += string.Format("and C002 like '%{0}%'", TbSearchPaper.Text);
                }
            }

            if (ChkPaperNum.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(TbSearchPaperNum.Text))
                {
                    sql += string.Format("C001 = '{0}'", TbSearchPaperNum.Text);
                }
            }

            if (ChkPaperType.IsChecked == true)
            {
                if (CmbPaperType.SelectedIndex > -1)
                {
                    sql += string.Format("and C004 = '{0}'", CmbPaperType.SelectedIndex + 1);
                }
            }

            if (ChkIntegrity.IsChecked == true)
            {
                sql += string.Format("and C018 = '{0}'", ComBoxIntegrity.SelectedIndex != 0 ? "0" : "1");
            }

            return sql;
        }

        private void PaperSearch_Click(object sender, RoutedEventArgs e)
        {
            string strLog;
            try
            {
                if (!CheckText())
                {
                    return;
                }

                string sql = SetSearchInfo();
                _mListPaperInfos.Clear();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptSearchPapers;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(sql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00033", "Search"));
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00033"),
                        Utils.FormatOptLogString("3603T00089"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(CurrentApp.GetLanguageInfo("3603T00065", "Insert data failed"));
                    return;
                }
                #region 写操作日志
                strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("3603T00033"),
                    Utils.FormatOptLogString("3603T00090"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00090", "Search Success"));

                if (webReturn.ListData.Count <= 0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00077", "Did not find the information!"));
                    return;
                }
                foreach (string strDate in webReturn.ListData)
                {
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(strDate);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamPapers = optReturn.Data as CPaperParam;
                    if (cExamPapers == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mListPaperInfos.Add(cExamPapers);
                }
                SetObservableCollection(_mListPaperInfos);
            }
            catch (Exception ex)
            {
                #region 写操作日志
                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00033"),
                    Utils.FormatOptLogString("3603T00089"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }

        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (SearchInfoName.Visibility == Visibility.Visible)
            {
                SearchInfoName.Visibility = Visibility.Collapsed;
                Button.Content = "<";
            }
            else
            {
                SearchInfoName.Visibility = Visibility.Visible;
                Button.Content = ">";
            }
        }

        private void PaperName_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkPaperName, TbSearchPaper);
        }

        private void SearchPaperNum_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkPaperNum, TbSearchPaperNum);
            DtStartTime.IsEnabled = ChkPaperNum.IsChecked == false;
            DtEndTime.IsEnabled = ChkPaperNum.IsChecked == false;

            ChkPaperName.IsEnabled = ChkPaperNum.IsChecked == false;
            ChkPaperName.IsChecked = false;
            SetCheckBox(ChkPaperName, TbSearchPaper);

            ChkPaperType.IsEnabled = ChkPaperNum.IsChecked == false;
            ChkPaperType.IsChecked = false;
            SetCheckBox(ChkPaperType, CmbPaperType);

            ChkIntegrity.IsEnabled = ChkPaperNum.IsChecked == false;
            ChkIntegrity.IsChecked = false;
            SetCheckBox(ChkIntegrity, ComBoxIntegrity);
        }

        private void PaperType_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkPaperType, CmbPaperType);
            InitPaperType(PaperType.TypeNull);
        }

        private void IntegrityName_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkIntegrity, ComBoxIntegrity);
        }

        private void SetCheckBox( CheckBox checkBox, TextBox textBox )
        {
            if (checkBox.IsChecked == true)
            {
                textBox.IsReadOnly = false;
                textBox.Background = Brushes.White;
            }
            else
            {
                textBox.IsReadOnly = true;
                textBox.Background = Brushes.LightGray;
            }
        }

        private void SetCheckBox( CheckBox checkBox, ComboBox comboBox)
        {
            comboBox.IsEnabled = checkBox.IsChecked == true;
        }

        private bool CheckText()
        {
            try
            {
                if (TbSearchPaperNum.Text.Length > 20)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00099", "Paper serial number is too long!"));
                    return false;
                }
                if (TbSearchPaper.Text.Length > 128)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00100", "Paper name cannot exceed 128 characters!"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void TbSearchPaper_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbSearchPaper.Text.Length > 128)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00100", "Paper name cannot exceed 128 characters!"));
            }
        }

        private void TbSearchPaperNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbSearchPaperNum.Text.Length > 20)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00099", "Paper serial number is too long!"));
            }
        } 
    }
}
