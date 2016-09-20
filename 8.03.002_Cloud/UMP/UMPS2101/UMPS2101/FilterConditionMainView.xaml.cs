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
using UMPS2101.Wcf11012;
using VoiceCyber.UMP.Encryptions;

namespace UMPS2101
{
    /// <summary>
    /// FilterConditionMainView.xaml 的交互逻辑
    /// </summary>
    public partial class FilterConditionMainView
    {
        #region Member
        private BackgroundWorker mWorker;
        static int m_maxrow = 0;
        //所有权限
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        public ObservableCollection<CStrategyType> mlststrategy;
        private ObservableCollection<AllFilterData> mListGetAllFilter;
        public List<CFilterCondition> mlstcfoperator;
        public AllFilterData mCurrentSelFilter;

        /// <summary>
        /// 当前用户操作权限
        /// </summary>
        public static ObservableCollection<OperationInfo> ListOperationInfos = new ObservableCollection<OperationInfo>();
        #endregion        
        public FilterConditionMainView()
        {
            InitializeComponent();
            mlststrategy = new ObservableCollection<CStrategyType>();
            mlstcfoperator = new List<CFilterCondition>();
            mListGetAllFilter = new ObservableCollection<AllFilterData>();

            Loaded += FilterConditionMainView_Loaded;
        }

        void FilterConditionMainView_Loaded(object sender, RoutedEventArgs e)
        {
            PageName = "UMP Filter Strategy";
            StylePath = "UMPS2101/MainPageStyle.xaml";
            cmbsttype.ItemsSource = mlststrategy;
            cmbsttype.DisplayMemberPath = "Show";
            cmbsttype.SelectedValuePath = "ID";

            LvStrategy.SelectionChanged += LvStrategy_SelectionChanged;
            LvStrategy.ItemsSource = mListGetAllFilter;

            ChangeTheme();
            DateStart.Value = DateTime.Now.Date + new TimeSpan(0, 0, 0);
            DateEnd.Value = DateTime.Now.AddYears(1).Date + new TimeSpan(0, 0, 0);

            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                LoadOperations();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();

                //触发Loaded消息
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                InitFilters();
                CreateUC();

                ChangeTheme();
                ChangeLanguage();
            };
            mWorker.RunWorkerAsync();
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

        public void SetMyWaiterVisibility(bool isShow)
        {
            //MyWaiter.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
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
                ShowExceptionMessage(ex.Message);
            }
        }
        public void ShowExceptionMessage(string msg)
        {
            MessageBox.Show(msg, CurrentApp.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
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
                webRequest.Session = CurrentApp.Session;
                //Service21011Client client = new Service21011Client();
                Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<AllFilterData>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                            return;
                        }
                        AllFilterData fdata = optReturn.Data as AllFilterData;
                        if (fdata != null)
                        {
                            fdata.StrategyName = DecryptString(fdata.StrategyName);
                            fdata.StrIsValid = fdata.IsValid == "1" ? CurrentApp.GetLanguageInfo("2101T00020", "Valid") : CurrentApp.GetLanguageInfo("2101T00021", "InValid");
                            mListGetAllFilter.Add(fdata);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="selStInfo"></param>
        public void SetDiscription(StrategyInfo selStInfo)
        {
            if (selStInfo != null)
                labDiscription.Content = CurrentApp.GetLanguageInfo("2101TD" + selStInfo.FieldName, selStInfo.FieldName);
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

        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
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
            lblsttype.Content = CurrentApp.GetLanguageInfo("2101T00009", "Strategy Type");
            lblstname.Content = CurrentApp.GetLanguageInfo("2101T00006", "Strategy Name");
            lblisvalid.Content = CurrentApp.GetLanguageInfo("2101T00007", "IsValid");
            lblstarttime.Content = CurrentApp.GetLanguageInfo("2101T00012", "Strategy StartTime");
            lblendtime.Content = CurrentApp.GetLanguageInfo("2101T00013", "Strategy EndTime");
            lbldiscription.Content = CurrentApp.GetLanguageInfo("2101T00015", "Remarks");
            rdbValid.Content = CurrentApp.GetLanguageInfo("2101T00020", "Valid");
            rdbInValid.Content = CurrentApp.GetLanguageInfo("2101T00021", "InValid");
            foreach (AllFilterData afd in mListGetAllFilter)
            {
                afd.StrIsValid = afd.IsValid == "1" ? CurrentApp.GetLanguageInfo("2101T00020", "Valid") : CurrentApp.GetLanguageInfo("2101T00021", "InValid");
            }
        }

        /// <summary>
        /// 绑定回删类型
        /// </summary>
        private void BindLogical()
        {
            mlststrategy.Clear();
            mlststrategy.Add(new CStrategyType() { ID = 1, Show = CurrentApp.GetLanguageInfo("2101T00022", "Archive Backup") });
            //mlststrategy.Add(new CStrategyType() { ID = 2, Show = App.GetLanguageInfo("2101T00023", "Delete Strategy") });
        }

        #region ChanhgeTheme

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
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS2101;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }
            try
            {
                string uri = string.Format("/UMPS2101;component/Themes/Default/UMPS2101/AvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("2" + ex.Message);
            }
        }

        #endregion

        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
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
                    panel.Title = CurrentApp.GetLanguageInfo("2101T00003", "Strategy List");
                }
                panel = GetPanleByContentID("PanelStrategySet");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("2101T00004", "Strategy Set");
                }
                panel = GetPanleByContentID("PanelConditionSet");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("2101T00005", "Condition Set");
                }
                #endregion

                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("2101T00001", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("2101T00002", "Other Position");

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
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
        #endregion

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
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00024", "Illegal Parameter"));
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
                                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00028", "Invalid Format.") + "\t" + "2015-06-01 08:12:20");
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
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00024", "Illegal Parameter"));
                    return;
                }
                #endregion
                AllFilterData allfd = new AllFilterData();
                allfd.FilterType = 1;
                allfd.StrategyType = cmbsttype.SelectedValue.ToString();
                allfd.StrategyCode = GetSerialID("257");
                if (allfd.StrategyCode == -1)
                { ShowExceptionMessage("GetSerialID Error."); return; }
                allfd.StrategyName = txtstname.Text;
                allfd.IsValid = rdbValid.IsChecked == true ? "1" : "0";
                allfd.IsDelete = "0";
                allfd.Creator = CurrentApp.Session.UserID;
                allfd.CreateTime = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.FilterNumber = 0;
                allfd.DateStart = Convert.ToDateTime(DateStart.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.DateEnd = Convert.ToDateTime(DateEnd.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                allfd.Remarks = txtdes.Text;
                allfd.listFilterCondition = mlstcfoperator;

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2101Codes.SubmitStrategies;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(allfd);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                webRequest.ListData.Add("0");//0 添加策略
                //Service21011Client client = new Service21011Client();
                Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_ALREADY_EXIST)
                        CurrentApp.ShowExceptionMessage(CurrentApp.GetLanguageInfo("2101T00025", "Filter Strategy exists."));
                    else
                        CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                    return;
                }
                else
                {
                    string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101001")), allfd.StrategyName);
                    CurrentApp.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                    //操作成功 刷新数据
                    InitFilters();
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00026", "Operation Succeed."));
                }
            }
            catch (Exception ex)
            { CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
        }

        /// <summary>
        ///  修改筛选策略
        /// </summary>
        private void UpdateFilterStrategy()
        {
            AllFilterData item = LvStrategy.SelectedItem as AllFilterData;
            if (item != null)
            {
                string messageBoxText = CurrentApp.GetLanguageInfo("2101T00030", "Confirm Update?");
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, CurrentApp.AppName, button, icon);
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
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00024", "Illegal Parameter"));
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
                                                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00028", "Invalid Format."));//+ "\t" + "2015-01-01 01:01:01"
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
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00024", "Illegal Parameter"));
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
                    allfd.Creator = CurrentApp.Session.UserID;
                    allfd.CreateTime = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                    //allfd.FilterNumber = 0;
                    allfd.DateStart = Convert.ToDateTime(DateStart.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                    allfd.DateEnd = Convert.ToDateTime(DateEnd.Text).ToUniversalTime().ToString("yyyyMMddHHmmss");
                    allfd.Remarks = txtdes.Text;
                    allfd.listFilterCondition = mlstcfoperator;

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2101Codes.SubmitStrategies;
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(allfd);
                    if (!optReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), optReturn.Message));
                        return;
                    }
                    webRequest.Data = optReturn.Data.ToString();
                    webRequest.ListData.Add("1");//1 修改策略
                    //Service21011Client client = new Service21011Client();
                    Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21011"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        if (webReturn.Code == Defines.RET_ALREADY_EXIST)
                            CurrentApp.ShowExceptionMessage(CurrentApp.GetLanguageInfo("2101T00025", "Filter Strategy exists."));
                        else
                            CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                        return;
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101002")), item.StrategyName + "->" + allfd.StrategyName);
                        CurrentApp.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                        //操作成功 刷新数据
                        InitFilters();
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00026", "Operation Succeed."));
                    }
                }
                catch (Exception ex)
                { CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
            }
            else
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00029", "Please select at least one Strategy."));
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
                string messageBoxText = CurrentApp.GetLanguageInfo("2101T00031", "Confirm Remove?");
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, CurrentApp.AppName, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return;
                }

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2101Codes.DeleteStrategy;
                    webRequest.ListData.Add(item.StrategyCode.ToString());//筛选策略编码
                    //Service21011Client client = new Service21011Client();
                    Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21011"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), webReturn.Message));
                        return;
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2101003")), item.StrategyName);
                        CurrentApp.WriteOperationLog("2101", ConstValue.OPT_RESULT_SUCCESS, strLog);
                        //操作成功 刷新数据
                        InitFilters();
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00026", "Operation Succeed."));
                    }
                }
                catch (Exception ex)
                { CurrentApp.ShowExceptionMessage(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("2101T00027", "Operation Field."), ex.Message)); }
            }
            else
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("2101T00029", "Please select at least one Strategy."));
            }
        }


        #region Other
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
            bool ret = false;
            try
            {
                int.Parse(strnum);
                ret = true;
            }
            catch { }
            return ret;
        }

        #region GetSerialID
        public long GetSerialID(string table)
        {
            if (string.IsNullOrWhiteSpace(table))
                return 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("21");
                webRequest.ListData.Add(table);
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
                return -1;
            }
        }
        #endregion

        #region 权限
        private void LoadOperations()
        {
            try
            {
                ListOperations.Clear();
                LoadOperations(CurrentApp.ModuleID);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// *********************VoiceCyber*************************************
        ///  Author           : Luoyihua
        ///  Created          : 2016-04-08 10:37:09
        /// <summary>
        /// Loads the operations.
        /// </summary>
        /// <param name="parentOptID">The parent opt identifier.</param>
        ///  Last Modified By : Luoyihua
        ///  Last Modified On : 2016-04-11 18:36:51
        ///  *********************VoiceCyber*************************************
        private void LoadOperations(long parentOptID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("21");
                webRequest.ListData.Add(parentOptID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                        //加载下级操作
                        LoadOperations(optInfo.ID);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init Operations"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
        #endregion


        public string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string DecryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
                return strSource;
            }
        }
    }
}
