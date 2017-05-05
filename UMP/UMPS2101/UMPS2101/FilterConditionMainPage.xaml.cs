using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Communications;
using System.Windows.Input;
using System.Timers;
using VoiceCyber.Wpf.AvalonDock.Layout;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS2101.UserControls;
using VoiceCyber.UMP.Common21011;
using System.Collections.Generic;
using UMPS2101.Wcf21011;
using VoiceCyber.Common;

namespace UMPS2101
{
    /// <summary>
    /// FilterConditionMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class FilterConditionMainPage
    {
        #region Member
        private int mIdleCheckCount;
        private int mIdleCheckInterval;
        private Timer mIdleCheckTimer;
        private BackgroundWorker mWorker;
        static int m_maxrow = 0;
        //所有权限
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();
        public ObservableCollection<CStrategyType> mlststrategy;
        private ObservableCollection<AllFilterData> mListGetAllFilter;
        public List<CFilterCondition> mlstcfoperator;
        public AllFilterData mCurrentSelFilter;
        #endregion        

        public FilterConditionMainPage()
        {
            InitializeComponent();
            mlststrategy = new ObservableCollection<CStrategyType>();
            mlstcfoperator = new List<CFilterCondition>();
            mListGetAllFilter = new ObservableCollection<AllFilterData>();

            Loaded += FilterConditionMainPage_Loaded;
        }

        void FilterConditionMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            cmbsttype.ItemsSource = mlststrategy;
            cmbsttype.DisplayMemberPath = "Show";
            cmbsttype.SelectedValuePath = "ID";

            LvStrategy.SelectionChanged += LvStrategy_SelectionChanged;
            LvStrategy.ItemsSource = mListGetAllFilter;

            ChangeTheme();
            DateStart.Value = DateTime.Now.Date + new TimeSpan(0, 0, 0);
            DateEnd.Value = DateTime.Now.AddYears(1).Date + new TimeSpan(0, 0, 0);



            MyWaiter.Visibility = Visibility.Visible;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                //从App里取到所有需要的权限
                InitOperation();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                MyWaiter.Visibility = Visibility.Collapsed;

                SendLoadedMessage();
                ChangeLanguage();
                InitFilters();
                CreateUC();
            };
            mWorker.RunWorkerAsync();
        }

        //从App里取到所有权限
        void InitOperation()
        {
            ListOperations = new ObservableCollection<OperationInfo>();
            if (App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_Filter).ToList().Count > 0)
            {
                ListOperations.Add(App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_Filter).First());
            }
            if (App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterConfig).ToList().Count > 0)
            {
                ListOperations.Add(App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterConfig).First());
            }
            if (App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterUpdate).ToList().Count > 0)
            {
                ListOperations.Add(App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterUpdate).First());
            }
            if (App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterDelete).ToList().Count > 0)
            {
                ListOperations.Add(App.ListOperationInfos.Where(p => p.ID == S2101Consts.OPT_FilterDelete).First());
            }
        }

        private void CreateOptButtons()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                Button btn;
                foreach (OperationInfo opt in ListOperations)
                {
                    if (opt.ID == S2101Consts.OPT_FilterConfig)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = opt;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelBasicOpts.Children.Add(btn);
                    }
                    else if (opt.ID == S2101Consts.OPT_FilterUpdate)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = opt;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelBasicOpts.Children.Add(btn);
                    }
                    else if (opt.ID == S2101Consts.OPT_FilterDelete)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = opt;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelBasicOpts.Children.Add(btn);
                    }
                }
            }
            catch
            { }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case S2101Consts.OPT_FilterConfig:
                            AddFilterStrategy();
                            break;
                        case S2101Consts.OPT_FilterUpdate:
                            UpdateFilterStrategy();
                            break;
                        case S2101Consts.OPT_FilterDelete:
                            DeleteFilterStrategy();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///  添加筛选策略
        /// </summary>
        private void AddFilterStrategy()
        {
            try
            {
                mlstcfoperator.Clear();
                #region 条件
                if (cmbsttype.SelectedItem == null || string.IsNullOrWhiteSpace(txtstname.Text) || !IsValidDate(DateStart.Text) || !IsValidDate(DateEnd.Text))
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("2101T00024", "Illegal Parameter"));
                    return;
                }
                foreach (var c in SPAllParameters.Children)
                {
                    if (c is UC_ConditionTextbox)
                    {
                        UC_ConditionTextbox tb = (UC_ConditionTextbox)c;
                        if (tb.Name.Contains("uc"))
                        {
                            try
                            {
                                ComboBox cmbcondition = tb.cmbcondition as ComboBox;
                                ComboBox cmbope = tb.cmbope as ComboBox;
                                TextBox txtval = tb.txtval as TextBox;
                                ComboBox cmblog = tb.cmblog as ComboBox;
                                if (!string.IsNullOrWhiteSpace(cmbcondition.Text) &&
                                    !string.IsNullOrWhiteSpace(cmbope.Text) &&
                                    !string.IsNullOrWhiteSpace(txtval.Text))
                                {
                                    StrategyInfo selStrategy = (cmbcondition.SelectionBoxItem as StrategyInfo);
                                    if (selStrategy.ConditionName.ToUpper() == "C004" || selStrategy.ConditionName.ToUpper() == "C005")
                                    {
                                        //时间格式判断
                                        if (!IsDateTimeValue(txtval.Text))
                                        {
                                            App.ShowInfoMessage(App.GetLanguageInfo("2101T00028", "Invalid Format.") + "\t" + "2015-06-01 08:12:20");
                                            return;
                                        }
                                    }
                                    int id = int.Parse(tb.Name.Substring(2, tb.Name.Length - 2));
                                    CFilterCondition tempfc = new CFilterCondition();
                                    tempfc.FilterTarget = 1;
                                    tempfc.ID = id;
                                    tempfc.ConditionName = (cmbcondition.SelectionBoxItem as StrategyInfo).ConditionName;
                                    tempfc.Operator = cmbope.Text;
                                    tempfc.isEnum = txtval.Text.Length > 1024 ? "1" : "0";
                                    tempfc.Value = txtval.Text;
                                    tempfc.Logical = cmblog.Text;
                                    mlstcfoperator.Add(tempfc);
                                }
                            }
                            catch
                            { }
                        }
                    }
                }
                if (mlstcfoperator.Count < 1)
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("2101T00024", "Illegal Parameter"));
                    return;
                }
                #endregion
                AllFilterData allfd = new AllFilterData();
                allfd.FilterType = 1;
                allfd.StrategyType = cmbsttype.SelectedValue.ToString();
                allfd.StrategyCode = App.GetSerialID("257");
                if (allfd.StrategyCode == -1)
                { App.ShowExceptionMessage("GetSerialID Error."); return; }
                allfd.StrategyName = txtstname.Text;
                allfd.IsValid = rdbValid.IsChecked == true ? "1" : "0";
                allfd.IsDelete = "0";
                allfd.Creator = App.Session.UserID;
                allfd.CreateTime = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.FilterNumber = 0;
                allfd.DateStart = Convert.ToDateTime(DateStart.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.DateEnd = Convert.ToDateTime(DateEnd.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.Remarks = txtdes.Text;
                allfd.listFilterCondition = mlstcfoperator;

                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2101Codes.SubmitStrategies;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(allfd);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                webRequest.ListData.Add("0");//0 添加策略
                //Service21011Client client = new Service21011Client();
                Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service21011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_ALREADY_EXIST)
                        App.ShowExceptionMessage(App.GetLanguageInfo("2101T00025", "Filter Strategy exists."));
                    else
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                    return;
                }
                else
                {
                    string strLog = string.Format("{0}{1}{2}", App.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101001")), allfd.StrategyName);
                    App.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                    //操作成功 刷新数据
                    InitFilters();
                    App.ShowInfoMessage(App.GetLanguageInfo("2101T00026", "Operation Succeed."));
                }
            }
            catch (Exception ex)
            { App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
        }

        /// <summary>
        ///  修改筛选策略
        /// </summary>
        private void UpdateFilterStrategy()
        {
            AllFilterData item = LvStrategy.SelectedItem as AllFilterData;
            if (item != null)
            {
                string messageBoxText = App.GetLanguageInfo("2101T00030", "Confirm Update?");
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, App.AppName, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return;
                }
                try
                {
                    mlstcfoperator.Clear();
                    #region 条件
                    if (cmbsttype.SelectedItem == null || string.IsNullOrWhiteSpace(txtstname.Text) || !IsValidDate(DateStart.Text) || !IsValidDate(DateEnd.Text))
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("2101T00024", "Illegal Parameter"));
                        return;
                    }
                    foreach (var c in SPAllParameters.Children)
                    {
                        if (c is UC_ConditionTextbox)
                        {
                            UC_ConditionTextbox tb = (UC_ConditionTextbox)c;
                            if (tb.Name.Contains("uc"))
                            {
                                try
                                {
                                    ComboBox cmbcondition = tb.cmbcondition as ComboBox;
                                    ComboBox cmbope = tb.cmbope as ComboBox;
                                    TextBox txtval = tb.txtval as TextBox;
                                    ComboBox cmblog = tb.cmblog as ComboBox;
                                    if (!string.IsNullOrWhiteSpace(cmbcondition.Text) &&
                                        !string.IsNullOrWhiteSpace(cmbope.Text) &&
                                        !string.IsNullOrWhiteSpace(txtval.Text))
                                    {
                                        StrategyInfo selStrategy = (cmbcondition.SelectionBoxItem as StrategyInfo);
                                        if (selStrategy.ConditionName.ToUpper() == "C004" || selStrategy.ConditionName.ToUpper() == "C005")
                                        {
                                            //时间格式判断
                                            if (!IsDateTimeValue(txtval.Text))
                                            {
                                                App.ShowInfoMessage(App.GetLanguageInfo("2101T00028", "Invalid Format."));//+ "\t" + "2015-01-01 01:01:01"
                                                return;
                                            }
                                        }
                                        int id = int.Parse(tb.Name.Substring(2, tb.Name.Length - 2));
                                        CFilterCondition tempfc = new CFilterCondition();
                                        tempfc.FilterTarget = 1;
                                        tempfc.ID = id;
                                        tempfc.ConditionName = (cmbcondition.SelectionBoxItem as StrategyInfo).ConditionName;
                                        tempfc.Operator = cmbope.Text;
                                        tempfc.isEnum = txtval.Text.Length > 1024 ? "1" : "0";
                                        tempfc.Value = txtval.Text;
                                        tempfc.Logical = cmblog.Text;
                                        mlstcfoperator.Add(tempfc);
                                    }
                                }
                                catch
                                { }
                            }
                        }
                    }
                    if (mlstcfoperator.Count < 1)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("2101T00024", "Illegal Parameter"));
                        return;
                    }
                    #endregion
                    AllFilterData allfd = new AllFilterData();
                    allfd.FilterType = 1;
                    allfd.StrategyCode = item.StrategyCode;
                    allfd.StrategyType = cmbsttype.SelectedValue.ToString();
                    allfd.StrategyName = txtstname.Text;
                    allfd.IsValid = rdbValid.IsChecked == true ? "1" : "0";
                    allfd.IsDelete = "0";
                    allfd.Creator = App.Session.UserID;
                    allfd.CreateTime = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                    //allfd.FilterNumber = 0;
                    allfd.DateStart = Convert.ToDateTime(DateStart.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                    allfd.DateEnd = Convert.ToDateTime(DateEnd.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                    allfd.Remarks = txtdes.Text;
                    allfd.listFilterCondition = mlstcfoperator;

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)S2101Codes.SubmitStrategies;
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(allfd);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), optReturn.Message));
                        return;
                    }
                    webRequest.Data = optReturn.Data.ToString();
                    webRequest.ListData.Add("1");//1 修改策略
                    //Service21011Client client = new Service21011Client();
                    Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service21011"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        if (webReturn.Code == Defines.RET_ALREADY_EXIST)
                            App.ShowExceptionMessage(App.GetLanguageInfo("2101T00025", "Filter Strategy exists."));
                        else
                            App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                        return;
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", App.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101002")), item.StrategyName + "->" + allfd.StrategyName);
                        App.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                        //操作成功 刷新数据
                        InitFilters();
                        App.ShowInfoMessage(App.GetLanguageInfo("2101T00026", "Operation Succeed."));
                    }
                }
                catch (Exception ex)
                { App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
            }
            else
            {
                App.ShowInfoMessage(App.GetLanguageInfo("2101T00029", "Please select at least one Strategy."));
            }
        }

        /// <summary>
        ///  删除筛选策略
        /// </summary>
        private void DeleteFilterStrategy()
        {
            AllFilterData item = LvStrategy.SelectedItem as AllFilterData;
            if (item != null)
            {
                string messageBoxText = App.GetLanguageInfo("2101T00031", "Confirm Remove?");
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, App.AppName, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return;
                }

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)S2101Codes.DeleteStrategy;
                    webRequest.ListData.Add(item.StrategyCode.ToString());//筛选策略编码
                    //Service21011Client client = new Service21011Client();
                    Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service21011"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                        return;
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", App.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101003")), item.StrategyName);
                        App.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                        //操作成功 刷新数据
                        InitFilters();
                        App.ShowInfoMessage(App.GetLanguageInfo("2101T00026", "Operation Succeed."));
                    }
                }
                catch (Exception ex)
                { App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
            }
            else
            {
                App.ShowInfoMessage(App.GetLanguageInfo("2101T00029", "Please select at least one Strategy."));
            }
        }

        /// <summary>
        /// 绑定回删类型
        /// </summary>
        private void BindLogical()
        {
            mlststrategy.Clear();
            mlststrategy.Add(new CStrategyType() { ID = 1, Show = App.GetLanguageInfo("2101T00022", "Archive Backup") });
            //mlststrategy.Add(new CStrategyType() { ID = 2, Show = App.GetLanguageInfo("2101T00023", "Delete Strategy") });
        }

        void LvStrategy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllFilterData item = LvStrategy.SelectedItem as AllFilterData;
            if (item != null)
            {
                mCurrentSelFilter = item;
                BindFilterConditions(item);
            }
        }

        /// <summary>
        /// 绑定具体筛选条件
        /// </summary>
        /// <param name="item"></param>
        private void BindFilterConditions(AllFilterData item)
        {
            cmbsttype.SelectedIndex = (item.FilterType - 1);
            txtstname.Text = item.StrategyName;
            if (item.IsValid == "1")
                rdbValid.IsChecked = true;
            else
                rdbInValid.IsChecked = true;
            try
            {
                DateTime dtstart = Convert.ToDateTime(item.DateStart.Insert(12, ":").Insert(10, ":").Insert(8, " ").Insert(6, "-").Insert(4, "-"));
                DateStart.Text = dtstart.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                DateTime dtend = Convert.ToDateTime(item.DateEnd.Insert(12, ":").Insert(10, ":").Insert(8, " ").Insert(6, "-").Insert(4, "-"));
                DateEnd.Text = dtend.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                txtdes.Text = item.Remarks;
            }
            catch { }
            SPAllParameters.Children.Clear();
            int count = item.listFilterCondition.Count;
            for (int i = 0; i < count; i++)
            {
                CreateUC(item.listFilterCondition[i]);
            }
        }

        private void CreateUC(CFilterCondition cfc)
        {
            UC_ConditionTextbox uc1 = new UC_ConditionTextbox();
            uc1.mParent = this;
            uc1.Name = "uc" + m_maxrow;
            uc1.cmblog.SelectionChanged += cmblog_SelectionChanged;
            uc1.SetValues(cfc);
            m_maxrow++;
            SPAllParameters.Children.Add(uc1);
        }

        private void CreateUC()
        {
            UC_ConditionTextbox uc1 = new UC_ConditionTextbox();
            uc1.mParent = this;
            uc1.Name = "uc" + m_maxrow;
            uc1.cmblog.SelectionChanged += cmblog_SelectionChanged;
            m_maxrow++;
            SPAllParameters.Children.Add(uc1);
        }

        private void cmblog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UC_ConditionTextbox cmblog = ((sender as ComboBox).Parent as Grid).Parent as UC_ConditionTextbox;
                string str_id = cmblog.Name.Substring(2, cmblog.Name.Length - 2);
                if (str_id == (m_maxrow - 1).ToString())
                    CreateUC();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="selStInfo"></param>
        public void SetDiscription(StrategyInfo selStInfo)
        {
            if (selStInfo != null)
                labDiscription.Content = App.GetLanguageInfo("2101TD" + selStInfo.FieldName, selStInfo.FieldName);
        }

        private void InitFilterColumns()
        {
            string[] lans = "2101T00006,2101T00007,2101T00008".Split(',');
            string[] cols = "StrategyName,StrIsValid,CreatorName".Split(',');
            int[] colwidths = { 125, 75, 100 };
            GridView columngv = new GridView();
            for (int i = 0; i < 3; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LvStrategy.View = columngv;
        }

        private void InitFilters()
        {
            try
            {
                try
                {
                    mListGetAllFilter.Clear();
                }
                catch { }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2101Codes.GetFilterWithCreator;
                webRequest.Session = App.Session;
                //Service21011Client client = new Service21011Client();
                Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service21011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<AllFilterData>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            App.ShowExceptionMessage(string.Format("{0}\t{1}", App.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                            return;
                        }
                        AllFilterData fdata = optReturn.Data as AllFilterData;
                        if (fdata != null)
                        {
                            fdata.StrategyName = App.DecryptString(fdata.StrategyName);
                            fdata.StrIsValid = fdata.IsValid == "1" ? App.GetLanguageInfo("2101T00020", "Valid") : App.GetLanguageInfo("2101T00021", "InValid");
                            mListGetAllFilter.Add(fdata);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = App.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }

        private void SetContentLanguage()
        {
            lblsttype.Content = App.GetLanguageInfo("2101T00009", "Strategy Type");
            lblstname.Content = App.GetLanguageInfo("2101T00006", "Strategy Name");
            lblisvalid.Content = App.GetLanguageInfo("2101T00007", "IsValid");
            lblstarttime.Content = App.GetLanguageInfo("2101T00012", "Strategy StartTime");
            lblendtime.Content = App.GetLanguageInfo("2101T00013", "Strategy EndTime");
            lbldiscription.Content = App.GetLanguageInfo("2101T00015", "Remarks");
            rdbValid.Content = App.GetLanguageInfo("2101T00020", "Valid");
            rdbInValid.Content = App.GetLanguageInfo("2101T00021", "InValid");
            foreach (AllFilterData afd in mListGetAllFilter)
            {
                afd.StrIsValid = afd.IsValid == "1" ? App.GetLanguageInfo("2101T00020", "Valid") : App.GetLanguageInfo("2101T00021", "InValid");
            }
        }

        private bool IsDateTimeValue(string strdate)
        {
            try
            {
                DateTime.Parse(strdate);
                return true;
            }
            catch
            { return false; }
        }

        protected override void Init()
        {
            try
            {
                PageHead.AppName = "UMP Filter Strategy";
                StylePath = "UMPS2101/MainPageStyle.xaml";
                base.Init();

                SendLoadedMessage();
                ChangeTheme();
                ChangeLanguage();

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #region 页头命令
        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);
            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        App.Session.ThemeInfo = themeInfo;
                        App.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        App.Session.LangTypeInfo = langType;
                        App.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                        mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Collapsed;
                            ChangeLanguage();
                            PopupPanel.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }
                    break;
                //展开或关闭侧边栏
                case 121:
                    OpenCloseLeftPanel();
                    break;
                case 120:
                    SendChangePasswordMessage();
                    break;
                case 201:
                    SendLogoutMessage();
                    break;
                case 202:
                    SendNavigateHomeMessage();
                    break;
            }
        }

        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }
        #endregion

        #region Langguage
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = App.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                CreateOptButtons();

                InitFilterColumns();
                SetContentLanguage();
                BindLogical();

                #region panel
                var panel = GetPanleByContentID("PanelStrategyList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("2101T00003", "Strategy List");
                }
                panel = GetPanleByContentID("PanelStrategySet");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("2101T00004", "Strategy Set");
                }
                panel = GetPanleByContentID("PanelConditionSet");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("2101T00005", "Condition Set");
                }
                #endregion

                //给换语言包
                ExpanderBasic.Header = App.GetLanguageInfo("2101T00001", "Basic Operations");
                ExpanderOther.Header = App.GetLanguageInfo("2101T00002", "Other Position");
               
            }
            catch (Exception ex)
            {

            }
        }

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }
        #endregion

        #region Themes
        public override void ChangeTheme()
        {
            base.ChangeTheme();
            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception ex)
                {
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/Themes/Default/UMPS2101/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }
            var page = PageHead;
            if (page != null)
            {
                page.ChangeTheme();
                page.InitInfo();
            }
        }
        #endregion

        #region other
        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    ChangeLanguage();
                                    if (PopupPanel != null)
                                    {
                                        PopupPanel.ChangeLanguage();
                                    }
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }
        private bool IsValidDate(string strdate)
        {
            bool ret = false;
            try
            {
                DateTime.Parse(strdate);
                ret = true;
            }
            catch { }
            return ret;
        }
        private bool IsValidNum(string strnum)
        { 
            bool ret=false;
            try {
                int.Parse(strnum);
                ret = true;
            }
            catch { }
            return ret;
        }
        #endregion
    }
}
