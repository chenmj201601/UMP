using Common2400;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
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
using UMPS2400.Entries;
using UMPS2400.MainUserControls;
using UMPS2400.Service11012;
using UMPS2400.Service24021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// UC_AddPolicy.xaml 的交互逻辑
    /// </summary>
    public partial class UC_AddPolicy
    {
        #region 变量定义
        public UC_EncryptionPolicyManagement mainPage = null;
        //添加或是修改操作
        public int iAddOrModify = 0;
        //正在被修改的项
        public UMPEncryptionPolicy PolicyModifying = null;
        public PolicyInfoInList PolicyModifyingInList = null;
        //BackgroundWorker 用来做添加或修改的wcf交互 
        BackgroundWorker mBackgroundWorker;

        // 密钥自定义/自动生成的Combobox选项加载的子界面
        UC_EncryptionPolicyTypeU IPagePolicyTypeU = null;
        UC_EncryptionPolicyTypeC IPagePolicyTypeC = null;

        ObservableCollection<ViewColumnInfo> mListColumnItemsInDepends = new ObservableCollection<ViewColumnInfo>();
        ObservableCollection<ViewColumnInfo> mListColumnItemsInKeys = new ObservableCollection<ViewColumnInfo>();

        ObservableCollection<PolicyBindingInfo> lstPolicyBinds = new ObservableCollection<PolicyBindingInfo>();
        ObservableCollection<PolicyKeyInfo> lstPolicyKeys = new ObservableCollection<PolicyKeyInfo>();

        /// <summary>
        /// 正在编辑的策略 
        /// </summary>
        public UMPEncryptionPolicy policyModifying = null;

        private List<string> lstInstancePolicyInformation = new List<string>();

        PolicyUpdateEntry updateEntry = null;
        #endregion

        public UC_AddPolicy()
        {
            InitializeComponent();
            this.Loaded += UC_AddPolicy_Loaded;
            ComboType.SelectionChanged += ComboType_SelectionChanged;
            RadioBeginDate.Checked += RadioBeginDate_Checked;
            RadioEndDate.Checked += RadioEndDate_Checked;
            RadioBeginImmediately.Checked += RadioBeginImmediately_Checked;
            RadioNoEndDate.Checked += RadioNoEndDate_Checked;
            CheckEnabledComplexity.Checked += CheckEnabledComplexity_Checked;
            CheckEnabledComplexity.Unchecked += CheckEnabledComplexity_Unchecked;
            CheckMustContainUppercase.Checked += CheckEnabledComplexity_Checked;
            CheckMustContainUppercase.Unchecked += CheckEnabledComplexity_Unchecked;
            CheckMustContainLowercase.Checked += CheckEnabledComplexity_Checked;
            CheckMustContainLowercase.Unchecked += CheckEnabledComplexity_Unchecked;
            CheckMustContainDigital.Checked += CheckEnabledComplexity_Checked;
            CheckMustContainDigital.Unchecked += CheckEnabledComplexity_Unchecked;
            CheckMustContainSpecial.Checked += CheckEnabledComplexity_Checked;
            CheckMustContainSpecial.Unchecked += CheckEnabledComplexity_Unchecked;
            BtnApply.Click += BtnApply_Click;
            BtnCancel.Click += BtnCancel_Click;
            TextNumbersUppercase.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            TextNumbersLowercase.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            TextNumbersDigital.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            TextNumbersSpecial.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            TextMinLength.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            TextMaxLength.TextChanged += new TextChangedEventHandler(TextNumbers_TextChanged);
            StartDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(ButtonControl_SelectedDateChanged);
            EndDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(ButtonControl_SelectedDateChanged);
        }

        #region 初始化
        void UC_AddPolicy_Loaded(object sender, RoutedEventArgs e)
        {
            IPagePolicyTypeU = new UC_EncryptionPolicyTypeU();
            IPagePolicyTypeC = new UC_EncryptionPolicyTypeC();
            IPagePolicyTypeC.CurrentApp = CurrentApp;
            IPagePolicyTypeU.CurrentApp = CurrentApp;

            mBackgroundWorker = new BackgroundWorker();
            string strStatusMsg = CurrentApp.GetLanguageInfo("2402001", "Loading...");
            mainPage.ShowStausMessage(strStatusMsg, true);

            if (iAddOrModify == (int)OperationType.Modify)
            {
                updateEntry = new PolicyUpdateEntry();
                updateEntry.PolicyEndTime = PolicyModifyingInList.PolicyEndTimeNumber;
                updateEntry.PolicyName = policyModifying.PolicyName;
                lvDependencies.ItemsSource = lstPolicyBinds;
                lvEncryptionKey.ItemsSource = lstPolicyKeys;
            }
            mBackgroundWorker.DoWork += (s, de) =>
                {
                    if (iAddOrModify == (int)OperationType.Modify)
                    {
                        //在修改时 加载它的依赖项、生成的密钥
                        InitDependencies();
                        InitKeys();
                    }
                };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
                {
                    InitLanguage();
                    mainPage.ShowStausMessage(string.Empty, false);
                    ComboType.SelectedIndex = 1;
                    InitControls();
                    if (iAddOrModify == (int)OperationType.Add)
                    {
                        if (mainPage.HasKeyGenServer)
                        {
                            RadioBeginImmediately.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            RadioBeginImmediately.Visibility = Visibility.Hidden;
                        }
                    }
                    if (iAddOrModify == (int)OperationType.Modify)
                    {
                        CreateColumnsItemsInDepends();
                        CreateColumnsItemsInKeys();
                        BindDependenciesData();
                        BindKeysData();
                    }
                };
            mBackgroundWorker.RunWorkerAsync();
        }

        private void InitLanguage()
        {
            #region Tab1  常规
            TabOjbect01.Header = CurrentApp.GetLanguageInfo("2402H001", "General");
            LabelName.Content = CurrentApp.GetLanguageInfo("2402L003", "Name");
            LabelType.Content = CurrentApp.GetLanguageInfo("2402L004", "Key Type");
            LabelFrequency.Content = CurrentApp.GetLanguageInfo("2402L005", "Randomly generated / Custom");
            cmbItemC.Content = CurrentApp.GetLanguageInfo("2402002", "Periodic update key (randomly generated)");
            cmbItemU.Content = CurrentApp.GetLanguageInfo("2402003", "Custom (user input)");
            LabelDuration.Content = CurrentApp.GetLanguageInfo("2402L006", "Duration Time");
            RadioBeginDate.Content = CurrentApp.GetLanguageInfo("2402RB007", "Start Time");
            RadioEndDate.Content = CurrentApp.GetLanguageInfo("2402RB008", "End Time");
            RadioBeginImmediately.Content = CurrentApp.GetLanguageInfo("2402RB001", "Immediately start");
            RadioNoEndDate.Content = CurrentApp.GetLanguageInfo("2402RB002", "No end time");
            #endregion

            #region Tab2 复杂性
            TabOjbect02.Header = CurrentApp.GetLanguageInfo("2402H002", "Complexity");
            CheckMustContainUppercase.Content = CurrentApp.GetLanguageInfo("2402L008", "The lowest number of capital letters");
            CheckEnabledComplexity.Content = CurrentApp.GetLanguageInfo("2402L014", "A key to force the use of complexity");
            CheckMustContainLowercase.Content = CurrentApp.GetLanguageInfo("2402L009", "The lowest number of lowercase letters");
            CheckMustContainDigital.Content = CurrentApp.GetLanguageInfo("2402L010", "Lowest number");
            CheckMustContainSpecial.Content = CurrentApp.GetLanguageInfo("2402011", "The minimum number of special characters");
            LabelKeyLength.Content = CurrentApp.GetLanguageInfo("2402L012", "Key length range");
            #endregion

            #region Tab3   描述
            TabOjbect03.Header = CurrentApp.GetLanguageInfo("2402H003", "Description");
            #endregion

            #region Tab4 依存对象
            TabOjbect04.Header = CurrentApp.GetLanguageInfo("2402H004", "Dependent object");
            LabelDependObject.Content = CurrentApp.GetLanguageInfo("2402L013", "The following encryption is dependent on this strategy");
            //加载ListView的列  ViewID : 2402002
            CreateColumnsItemsInDepends();
            #endregion

            #region Tab5  加密密钥
            TabOjbect05.Header = CurrentApp.GetLanguageInfo("2402H005", "Encryption key");
            //加载ListView的列  ViewID：2402003
            CreateColumnsItemsInKeys();
            #endregion

            BtnApply.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("COM002", "Cancel");
        }

        /// <summary>
        /// 初始化依赖项列表 2402002
        /// </summary>
        private void InitDependencies()
        {
            //获得列
            string mViewID = "2402002";
            mListColumnItemsInDepends.Clear();
            mListColumnItemsInDepends = GetColumnsByViewID(mViewID);
        }

        /// <summary>
        /// 初始化策略绑定服务器的列表
        /// </summary>
        private void BindDependenciesData()
        {
            lstPolicyBinds.Clear();
            //获得内容
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetPolicyBinding;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(policyModifying.PolicyID);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                if (webReturn.ListData == null)
                {
                    return;
                }
                OperationReturn optReturn = null;
                foreach (string strBinding in webReturn.ListData)
                {
                    optReturn = XMLHelper.DeserializeObject<PolicyBindingInfo>(strBinding);
                    if (!optReturn.Result)
                    {
                        continue;
                    }
                    lstPolicyBinds.Add(optReturn.Data as PolicyBindingInfo);
                }

            }
            catch (Exception ex)
            {
                ShowException(string.Format("WSFail.\t{0}\t", ex.Message));
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        private void InitKeys()
        {
            string mViewID = "2402003";
            mListColumnItemsInKeys.Clear();
            mListColumnItemsInKeys = GetColumnsByViewID(mViewID);
        }

        private void BindKeysData()
        {
            lstPolicyKeys.Clear();
            //获得内容
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetPolicyKey;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(policyModifying.PolicyID);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                List<PolicyKeyInfo> lstBindings = new List<PolicyKeyInfo>();
                if (webReturn.ListData == null)
                {
                    return;
                }
                OperationReturn optReturn = null;
                foreach (string strKeys in webReturn.ListData)
                {
                    optReturn = XMLHelper.DeserializeObject<PolicyKeyInfo>(strKeys);
                    if (!optReturn.Result)
                    {
                        continue;
                    }
                    lstPolicyKeys.Add(optReturn.Data as PolicyKeyInfo);
                }

            }
            catch (Exception ex)
            {
                ShowException(string.Format("WSFail.\t{0}\t", ex.Message));
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 根据viewID获得ColumnInfo集合
        /// </summary>
        /// <param name="strViewID"></param>
        /// <returns></returns>
        private ObservableCollection<ViewColumnInfo> GetColumnsByViewID(string strViewID)
        {
            ObservableCollection<ViewColumnInfo> lstColumnsResult = new ObservableCollection<ViewColumnInfo>();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(strViewID);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return null;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return null;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                lstColumnsResult.Clear();
                listColumns.ForEach(obj => lstColumnsResult.Add(obj));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return null;
            }
            return lstColumnsResult;
        }

        /// <summary>
        /// 创建依赖项的listviewcolumn
        /// </summary>
        private void CreateColumnsItemsInDepends()
        {
            string mViewID = "2402002";
            GridView gv = CreateColumnsByViewID(mViewID, mListColumnItemsInDepends);
            lvDependencies.View = gv;
        }

        /// <summary>
        /// 创建加密密钥的listviewcolumn
        /// </summary>
        private void CreateColumnsItemsInKeys()
        {
            string mViewID = "2402003";
            GridView gv = CreateColumnsByViewID(mViewID, mListColumnItemsInKeys);
            lvEncryptionKey.View = gv;
        }

        private GridView CreateColumnsByViewID(string strViewID, ObservableCollection<ViewColumnInfo> lstColumns)
        {
            GridView gv = new GridView();
            try
            {
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < lstColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = lstColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        string str = CurrentApp.Session.LangTypeID.ToString();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", strViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", strViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "EnableIcon")
                        {
                            dt = Resources["EnableIconCellTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "StatusIcon")
                        {
                            dt = Resources["StatusIconCellTemplate"] as DataTemplate;
                        }
                        if (dt == null)
                        {
                            string strColName = columnInfo.ColumnName;
                            gvc.DisplayMemberBinding = new Binding(strColName);
                        }
                        else
                        {
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return gv;
        }

        /// <summary>
        /// 根据iAddOrModify显示或隐藏控件
        /// </summary>
        private void InitControls()
        {
            DateTime LocalDateTimeServerAdd01Days = new DateTime();
            DateTime LocalDateTimeMonthLastDay = new DateTime();

            LocalDateTimeServerAdd01Days = S2400App.GolbalCurrentEncryptionDBTime.AddDays(1);
            LocalDateTimeMonthLastDay = LocalDateTimeServerAdd01Days;
            int LIntDaysInMonth = DateTime.DaysInMonth(LocalDateTimeMonthLastDay.Year, LocalDateTimeMonthLastDay.Month);
            LocalDateTimeMonthLastDay = LocalDateTimeMonthLastDay.AddDays(LIntDaysInMonth - LocalDateTimeMonthLastDay.Day);

            if (iAddOrModify == (int)OperationType.Add)
            {
                //添加时 如果是用户输入 则显示Tab01 Tab03  如果是自动生成 则显示Tab01 Tab02 Tab03
                TabOjbect02.Visibility = Visibility.Collapsed;
                TabOjbect04.Visibility = Visibility.Collapsed;
                TabOjbect05.Visibility = Visibility.Collapsed;

                TextStartDateTime.Text = LocalDateTimeServerAdd01Days.ToString("yyyy-MM-dd") + " 00:00:00";
                StartDate.SelectedDate = LocalDateTimeServerAdd01Days;

                TextEndDateTime.Text = LocalDateTimeMonthLastDay.ToString("yyyy-MM-dd") + " 23:59:59";
                EndDate.SelectedDate = LocalDateTimeMonthLastDay;

                CheckMustContainDigital.IsChecked = true;
                TextNumbersDigital.Text = "2";
                CheckMustContainLowercase.IsChecked = true;
                TextNumbersLowercase.Text = "2";
                CheckMustContainUppercase.IsChecked = true;
                TextNumbersUppercase.Text = "2";
            }
            else if (iAddOrModify == (int)OperationType.Modify)
            {
                //修改时 加载策略内容 并显示相应控件

                TabOjbect04.Visibility = Visibility.Visible;
                TabOjbect05.Visibility = Visibility.Visible;

                #region Tab01
                TextName.Text = policyModifying.PolicyName;
                if (policyModifying.IsBinded)
                {
                    TextName.IsEnabled = false;
                }
                else
                {
                    TextName.IsEnabled = true;
                }
                ComboType.IsEnabled = false;
                switch (policyModifying.PolicyType)
                {
                    case "U":
                        ComboType.SelectedIndex = 1;
                        //UC_EncryptionPolicyTypeU的Load中会处理其界面中的控件初始化
                        TextEndDateTime.IsEnabled = false;
                        RadioEndDate.IsEnabled = false;
                        RadioNoEndDate.IsChecked = true;
                        RadioNoEndDate.IsEnabled = false;

                        break;
                    case "C":
                        ComboType.SelectedIndex = 0;
                        //UC_EncryptionPolicyTypeC的Load中会处理其界面中的控件初始化
                        TextEndDateTime.IsEnabled = true;
                        RadioEndDate.IsEnabled = true;
                        RadioNoEndDate.IsEnabled = true;
                        //把utc时间转成本地时间 再做对比
                        DateTime dt = CommonFunctions.StringToDateTime(policyModifying.DurationEnd);

                        if (dt.ToString("yyyy-MM-dd HH:mm:ss") == "2099-12-31 23:59:59")
                        {
                            TextEndDateTime.Text = LocalDateTimeMonthLastDay.ToString("yyyy-MM-dd") + " 23:59:59";
                            RadioEndDate.IsChecked = false;
                            RadioNoEndDate.IsChecked = true;
                        }
                        else
                        {
                            TextEndDateTime.Text = CommonFunctions.StringToDateTime(policyModifying.DurationEnd).ToString("yyyy-MM-dd HH:mm:ss");
                            RadioEndDate.IsChecked = true;
                            RadioNoEndDate.IsChecked = false;
                        }
                        break;
                }
                TextStartDateTime.Text = CommonFunctions.StringToDateTime(policyModifying.DurationBegin).ToString("yyyy-MM-dd HH:mm:ss");
                TextStartDateTime.IsEnabled = false;
                RadioBeginImmediately.Visibility = Visibility.Hidden;
                #endregion

                #region Tab02
                if (policyModifying.PolicyType == "C")
                {
                    TabOjbect02.Visibility = Visibility.Visible;
                    if (policyModifying.Complexityabled == "1")
                    {
                        CheckEnabledComplexity.IsChecked = true;
                        if (policyModifying.MustContainCapitals == "1")
                        {
                            CheckMustContainUppercase.IsChecked = true;
                            TextNumbersUppercase.Text = policyModifying.NumbersCapitals.ToString();
                        }
                        if (policyModifying.MustContainDigital == "1")
                        {
                            CheckMustContainDigital.IsChecked = true;
                            TextNumbersDigital.Text = policyModifying.NumbersDigital.ToString();
                        }
                        if (policyModifying.MustContainLower == "1")
                        {
                            CheckMustContainLowercase.IsChecked = true;
                            TextNumbersLowercase.Text = policyModifying.NumbersLower.ToString();
                        }
                        if (policyModifying.MustContainSpecial == "1")
                        {
                            CheckMustContainSpecial.IsChecked = true;
                            TextNumbersSpecial.Text = policyModifying.NumbersSpecial.ToString();
                        }
                    }
                    else
                    {
                        CheckEnabledComplexity.IsChecked = false;
                    }

                    TextMaxLength.Text = policyModifying.TheLongestLength.ToString();
                    TextMinLength.Text = policyModifying.ThesHortestLength.ToString();
                }
                else
                {
                    TabOjbect02.Visibility = Visibility.Collapsed;
                }
                #endregion

                #region Tab03
                TextDescription.Text = policyModifying.PolicyNotes;
                #endregion

                #region Tab04

                #endregion
            }
        }
        #endregion

        #region Override
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitLanguage();

            if (ComboType.SelectedIndex == 0)
            {
                //周期性更新密钥
                if (IPagePolicyTypeC != null)
                {
                    IPagePolicyTypeC.ChangeLanguage();
                }
            }
            else if (ComboType.SelectedIndex == 1)
            {
                if (IPagePolicyTypeU != null)
                {
                    IPagePolicyTypeU.ChangeLanguage();
                }
            }
        }

        #endregion

        #region 界面控件事件
        /// <summary>
        /// 密钥类型（自定义/周期性自动生成）的选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrSelectedType = string.Empty;
            try
            {
                ComboBoxItem ComboItemPolicyType = ComboType.SelectedItem as ComboBoxItem;
                if (ComboItemPolicyType == null) { return; }
                LStrSelectedType = ComboItemPolicyType.Tag.ToString();
                if (LStrSelectedType == "U")
                {
                    PolicyTypeIsU();
                }
                else
                {
                    PolicyTypeIsC();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// “开始”于radiobutton的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioBeginDate_Checked(object sender, RoutedEventArgs e)
        {
            TextStartDateTime.IsEnabled = true;
        }

        /// <summary>
        /// “结束于”RadioButton的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioEndDate_Checked(object sender, RoutedEventArgs e)
        {
            TextEndDateTime.IsEnabled = true;
        }

        /// <summary>
        /// “立即开始” RadioButton的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioBeginImmediately_Checked(object sender, RoutedEventArgs e)
        {
            TextStartDateTime.IsEnabled = false;
        }

        /// <summary>
        /// “没有结束时间”的RadioButton的选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioNoEndDate_Checked(object sender, RoutedEventArgs e)
        {
            TextEndDateTime.IsEnabled = false;
        }

        /// <summary>
        /// "强制使用复杂密钥"的Checked事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckEnabledComplexity_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox LCheckBoxSender = sender as CheckBox;
                string LStrSenderName = LCheckBoxSender.Name;

                if (LStrSenderName == "CheckEnabledComplexity")
                {
                    CheckMustContainUppercase.IsEnabled = true;
                    CheckMustContainLowercase.IsEnabled = true;
                    CheckMustContainDigital.IsEnabled = true;
                    CheckMustContainSpecial.IsEnabled = true;
                }
                if (LStrSenderName == "CheckMustContainUppercase")
                {
                    TextNumbersUppercase.IsEnabled = true;
                    if (string.IsNullOrEmpty(TextNumbersUppercase.Text))
                    {
                        TextNumbersUppercase.Text = "2";
                    }
                }
                if (LStrSenderName == "CheckMustContainLowercase")
                {
                    TextNumbersLowercase.IsEnabled = true;
                    if (string.IsNullOrEmpty(TextNumbersLowercase.Text))
                    {
                        TextNumbersLowercase.Text = "2";
                    }
                }
                if (LStrSenderName == "CheckMustContainDigital")
                {
                    TextNumbersDigital.IsEnabled = true;
                    if (string.IsNullOrEmpty(TextNumbersDigital.Text))
                    {
                        TextNumbersDigital.Text = "2";
                    }
                }
                if (LStrSenderName == "CheckMustContainSpecial")
                {
                    TextNumbersSpecial.IsEnabled = true;
                    if (string.IsNullOrEmpty(TextNumbersSpecial.Text))
                    {
                        TextNumbersSpecial.Text = "2";
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// "强制使用复杂密钥"的未选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckEnabledComplexity_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox LCheckBoxSender = sender as CheckBox;
                string LStrSenderName = LCheckBoxSender.Name;

                if (LStrSenderName == "CheckEnabledComplexity")
                {
                    CheckMustContainUppercase.IsEnabled = false;
                    CheckMustContainLowercase.IsEnabled = false;
                    CheckMustContainDigital.IsEnabled = false;
                    CheckMustContainSpecial.IsEnabled = false;
                }
                if (LStrSenderName == "CheckMustContainUppercase") { TextNumbersUppercase.IsEnabled = false; }
                if (LStrSenderName == "CheckMustContainLowercase") { TextNumbersLowercase.IsEnabled = false; }
                if (LStrSenderName == "CheckMustContainDigital") { TextNumbersDigital.IsEnabled = false; }
                if (LStrSenderName == "CheckMustContainSpecial") { TextNumbersSpecial.IsEnabled = false; }
            }
            catch { }
        }

        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainPage.PopupPanel.IsOpen = false;
        }

        /// <summary>
        /// 确认按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            string strError = string.Empty;
            bool bo = CheckData(ref strError);
            if (!bo)
            {
                string strErrorMsg = CurrentApp.GetLanguageInfo("2402Error" + strError, "Error" + strError);
                ShowException(strErrorMsg);
                return;
            }
            if (iAddOrModify == (int)OperationType.Add)
            {
                AddPolicy();
            }
            else if (iAddOrModify == (int)OperationType.Modify)
            {
                UpdatePolicy();
            }
        }

        private void AddPolicy()
        {
            Service24021Client client = null;
            UMPEncryptionPolicy policy = null;
            try
            {
                bool bo = GetPolicyObject(ref policy);
                if (!bo)
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402003")), policy.PolicyName);
                    CurrentApp.WriteOperationLog("2402003", ConstValue.OPT_RESULT_FAIL, msg);
                    ShowException(CurrentApp.GetLanguageInfo("2402Error001", "Select the encryption policy type"));
                    return;
                }
                List<string> lstParams = UMPEncryptionPolicy.ObjectToListStringForAdd(policy);
                if (mainPage.keyGenEntry != null)
                {
                    lstParams.Add(mainPage.keyGenEntry.HostAddress);
                    lstParams.Add(mainPage.keyGenEntry.HostPort);
                }
                else
                {
                    lstParams.Add("");
                    lstParams.Add("");
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.AddEncryptionPolicy;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData = lstParams;
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (webReturn.Result)
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO2402003"), policy.PolicyName);
                    CurrentApp.WriteOperationLog("2402003", ConstValue.OPT_RESULT_SUCCESS, msg);
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("COMN001", "Successful operation"));
                    mainPage.PopupPanel.IsOpen = false;
                    if (webReturn.Data == null)
                    {
                        return;
                    }
                    string strPolicyID = webReturn.Data as string;
                    PolicyInfoInList po = new PolicyInfoInList();
                    po.PolicyID = strPolicyID;
                    po.PolicyIsEnabled = policy.PolicyIsEnabled ? "1" : "0";
                    po.PolicyName = policy.PolicyName;
                    if (policy.PolicyType == "C")
                    {
                        string strOccursFrequency = policy.PolicyOccursFrequency;
                        switch (strOccursFrequency)
                        {
                            case "D":
                                po.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagD", "Day");
                                break;
                            case "W":
                                po.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagW", "Week");
                                break;
                            case "M":
                                po.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("2402ComboTagM", "Month");
                                break;
                            case "U":
                                po.PolicyOccursFrequency = policy.BeginDayofCycle + CurrentApp.GetLanguageInfo("2402ComboTagD", "Day");
                                break;
                        }
                    }
                    else
                    {
                        po.PolicyOccursFrequency = string.Empty;
                    }

                    long iTime = 0;
                    if (string.IsNullOrEmpty(policy.DurationBegin))
                    {
                        //如果开始时间为空 则表示立即执行
                        mainPage.GetAppServerCurrentTime();
                        bo = CommonFunctions.DateTimeToNumber(S2400App.GolbalCurrentEncryptionDBTime, ref iTime);
                        po.PolicyStartTime = S2400App.GolbalCurrentEncryptionDBTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        bo = CommonFunctions.DateTimeToNumber(DateTime.Parse(policy.DurationBegin), ref iTime);
                        po.PolicyStartTime = policy.DurationBegin;
                    }

                    po.PolicyStartTimeNumber = iTime;
                    po.PolicyType = policy.PolicyType == "U" ? CurrentApp.GetLanguageInfo("2402003", "Custom (user input)") : CurrentApp.GetLanguageInfo("2402002", "Periodic update key (randomly generated)");
                    bo = CommonFunctions.DateTimeToNumber(DateTime.Parse(policy.DurationEnd), ref iTime);
                    po.PolicyEndTimeNumber = iTime;
                    po.PolicyIsEnabled = "1";

                    if (policy.DurationEnd.ToString() != "2099-12-31 23:59:59")
                    {
                        po.PolicyEndTime = DateTime.Parse(policy.DurationEnd).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        po.PolicyEndTime = CurrentApp.GetLanguageInfo("2402RB002", "Never expires");
                    }
                    if (policy.PolicyType == "C")
                    {
                        po.IsStrongPwd = policy.Complexityabled == "1" ? CurrentApp.GetLanguageInfo("2402019", "Yes") : string.Empty;
                    }
                    else
                    {
                        po.IsStrongPwd = string.Empty;
                    }
                    mainPage.UpdatePolicyList(po, OperationType.Add);
                }
                else
                {
                    string msg = string.Empty;
                    if (policy != null)
                    {
                        msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402003")), policy.PolicyName);
                    }
                    else
                    {
                        msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402003")));
                    }
                    CurrentApp.WriteOperationLog("2402003", ConstValue.OPT_RESULT_FAIL, msg);
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402003")), policy.PolicyName);
                CurrentApp.WriteOperationLog("2402003", ConstValue.OPT_RESULT_FAIL, msg);
                ShowException(CurrentApp.GetLanguageInfo("COMN002", "Operation failure") + ",error:" + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        private void UpdatePolicy()
        {
            Service24021Client client = null;
            UMPEncryptionPolicy policy = null;
            try
            {
                mainPage.ShowStausMessage("", true);
                bool bo = GetPolicyObject(ref policy);
                if (!bo)
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402004")), policy.PolicyID);
                    CurrentApp.WriteOperationLog("2402004", ConstValue.OPT_RESULT_FAIL, msg);
                    ShowException(CurrentApp.GetLanguageInfo("2402Error001", "Select the encryption policy type"));
                    return;
                }
                List<string> lstParams = UMPEncryptionPolicy.ObjectToListStringForModify(policy);
                if (mainPage.keyGenEntry != null)
                {
                    lstParams.Add(mainPage.keyGenEntry.HostAddress);
                    lstParams.Add(mainPage.keyGenEntry.HostPort);
                }
                else
                {
                    lstParams.Add("");
                    lstParams.Add("");
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.ModifyPolicy;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData = lstParams;
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (webReturn.Result)
                {
                    WriteUpdatePolicyLog();
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("COMN001", "Successful operation"));
                    mainPage.PopupPanel.IsOpen = false;
                }
                else
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402004")), policy.PolicyID);
                    CurrentApp.WriteOperationLog("2402004", ConstValue.OPT_RESULT_FAIL, msg);
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                mainPage.ShowStausMessage("", false);
                // PolicyInfoInList po = new PolicyInfoInList();
                PolicyModifyingInList.PolicyName = policy.PolicyName;

                long iTime = 0;
                bo = CommonFunctions.DateTimeToNumber(DateTime.Parse(policy.DurationEnd), ref iTime);
                PolicyModifyingInList.PolicyEndTimeNumber = iTime;

                if (policy.DurationEnd.ToString() != "2099-12-31 23:59:59")
                {
                    PolicyModifyingInList.PolicyEndTime = DateTime.Parse(policy.DurationEnd).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    PolicyModifyingInList.PolicyEndTime = CurrentApp.GetLanguageInfo("2402RB002", "Never expires");
                }
                if (policy.PolicyType == "C")
                {
                    PolicyModifyingInList.IsStrongPwd = policy.Complexityabled == "1" ? CurrentApp.GetLanguageInfo("2402019", "Yes") : string.Empty;
                }
                else
                {
                    PolicyModifyingInList.IsStrongPwd = string.Empty;
                }
                mainPage.UpdatePolicyList(PolicyModifyingInList, OperationType.Modify);
            }
            catch (Exception ex)
            {
                string msg = string.Empty;
                if (policy != null)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402004")), policy.PolicyID);
                }
                else
                {
                    msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2402004")));
                }
                CurrentApp.WriteOperationLog("2402004", ConstValue.OPT_RESULT_FAIL, msg);
                mainPage.ShowStausMessage("", false);
                ShowException(CurrentApp.GetLanguageInfo("COMN002", "Operation failure") + ",error:" + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 检查数据完整性 
        /// </summary>
        /// strError 001:请选择加密策略类型
        /// 002 : 密码长度错误，长度范围是6-64
        /// 003 : 执行周期错误
        /// 004 : 开始时间错误
        /// 005 : 开始时间不能大于2099-12-31 23:59:59
        /// 000 : 密钥策略名字不能为空
        /// 006 : 结束时间错误
        /// 007 : 结束时间不能大于2099-12-31 23:59:59
        /// 008 : 开始时间不能大于结束时间 
        /// 009 : 密钥复杂性错误 
        /// 010 : 开始时间必须大于等于当前时间
        /// 011 : 结束时间必须大于等于当前时间
        /// 012 : 密码长度不能大于26
        /// <returns></returns>
        private bool CheckData(ref string strError)
        {
            bool boResult = false;
            if (string.IsNullOrEmpty(TextName.Text))
            {
                strError = "000";
                return false;
            }
            ComboBoxItem ComboItemPolicyType = ComboType.SelectedItem as ComboBoxItem;
            if (ComboItemPolicyType == null)
            {
                strError = "001";
                return false;
            }
            string strType = ComboItemPolicyType.Tag.ToString();
            //调用加密策略类型子窗口的检查函数 检查数据的合法性
            if (strType == "U")
            {
                boResult = IPagePolicyTypeU.CheckData(ref strError);
                if (!boResult)
                {
                    return false;
                }
            }
            else
            {
                boResult = IPagePolicyTypeC.CheckData(ref strError);
                if (!boResult)
                {
                    return false;
                }
            }
            //检查开始时间、结束时间
            DateTime LDTStartDatetime = new DateTime();
            DateTime LDTEndDatetime = new DateTime();
            if (!DateTime.TryParse(TextStartDateTime.Text, out LDTStartDatetime)) { strError = "004"; return false; }
            if (LDTStartDatetime > DateTime.Parse("2099-12-31 23:59:59")) { strError = "005"; return false; }
            //添加时才检查开始时间
            if (iAddOrModify == (int)OperationType.Add)
            {
                if (LDTStartDatetime < DateTime.Now) { strError = "010"; return false; }
            }

            //如果是周期类型 且非永不过期 需要检查结束时间 
            if (strType == "C")
            {
                if (RadioEndDate.IsChecked == true)
                {
                    if (!DateTime.TryParse(TextEndDateTime.Text, out LDTEndDatetime)) { strError = "006"; return false; }
                    if (LDTEndDatetime > DateTime.Parse("2099-12-31 23:59:59")) { strError = "007"; return false; }
                    if (RadioBeginDate.IsChecked == true)
                    {
                        if (LDTStartDatetime > LDTEndDatetime) { strError = "008"; return false; }
                    }
                    if (LDTEndDatetime < DateTime.Now) { strError = "011"; return false; }
                }
            }

            //检查复杂性
            if (CheckEnabledComplexity.IsChecked == true && CheckMustContainDigital.IsChecked == false && CheckMustContainLowercase.IsChecked == false
                && CheckMustContainSpecial.IsChecked == false && CheckMustContainUppercase.IsChecked == false)
            {
                strError = "009";
                return false;
            }

            //检查密码最大长度
            if (int.Parse(TextMaxLength.Text) > 26)
            {
                strError = "012";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从界面填的值 获得策略具体信息
        /// </summary>
        private bool GetPolicyObject(ref UMPEncryptionPolicy policy)
        {
            if (iAddOrModify == (int)OperationType.Add)
            {
                policy = new UMPEncryptionPolicy();
            }
            else
            {
                policy = policyModifying;
            }
            policy.PolicyName = TextName.Text;
            //策略类型
            ComboBoxItem ComboItemPolicyType = ComboType.SelectedItem as ComboBoxItem;
            if (ComboItemPolicyType == null) { return false; }
            policy.PolicyType = ComboItemPolicyType.Tag.ToString();
            //有效期  用户输入类型只有开始日期 永不过期 策略类型的有效期开始时间同用户输入类型 但有结束日期
            if (iAddOrModify == (int)OperationType.Add)
            {
                if (RadioBeginDate.IsChecked == true)
                {
                    policy.IsImmediately = 0;
                    policy.DurationBegin = TextStartDateTime.Text;
                }
                else if (RadioBeginImmediately.IsChecked == true)
                {
                    //如果“立即生效”选中 则在wcf中获取当前时间
                    policy.IsImmediately = 1;
                    policy.DurationBegin = string.Empty;
                }
            }

            //用户输入类型策略
            if (policy.PolicyType == "U")
            {
                policy = IPagePolicyTypeU.GetPolicyInTypeU(policy, updateEntry);
                //结束时间 永不过期
                policy.DurationEnd = "2099-12-31 23:59:59";
                policy.ThesHortestLength = 6;
                policy.TheLongestLength = 64;
                if (iAddOrModify == (int)OperationType.Modify)
                {
                    updateEntry.IsUpdateEndTime = false;
                }
            }
            else
            {
                policy = IPagePolicyTypeC.GetPolicyInTypeC(policy, updateEntry);
                #region 复杂性
                if (CheckEnabledComplexity.IsChecked == true)
                {
                    policy.Complexityabled = "1";
                    if (CheckMustContainUppercase.IsChecked == true)
                    {
                        policy.MustContainCapitals = "1";
                        policy.NumbersCapitals = int.Parse(TextNumbersUppercase.Text);
                    }
                    else
                    {
                        policy.MustContainCapitals = "0";
                        policy.NumbersCapitals = 0;
                    }
                    if (CheckMustContainLowercase.IsChecked == true)
                    {
                        policy.MustContainLower = "1";
                        policy.NumbersLower = int.Parse(TextNumbersLowercase.Text);
                    }
                    else
                    {
                        policy.MustContainLower = "0";
                        policy.NumbersLower = 0;
                    }
                    if (CheckMustContainDigital.IsChecked == true)
                    {
                        policy.MustContainDigital = "1";
                        policy.NumbersDigital = int.Parse(TextNumbersDigital.Text);
                    }
                    else
                    {
                        policy.MustContainDigital = "0";
                        policy.NumbersDigital = 0;
                    }
                    if (CheckMustContainSpecial.IsChecked == true)
                    {
                        policy.MustContainSpecial = "1";
                        policy.NumbersSpecial = int.Parse(TextNumbersSpecial.Text);
                    }
                    else
                    {
                        policy.MustContainSpecial = "0";
                        policy.NumbersSpecial = 0;
                    }
                }

                policy.ThesHortestLength = int.Parse(TextMinLength.Text);
                policy.TheLongestLength = int.Parse(TextMaxLength.Text);
                #endregion

                //结束时间
                long lEndTime = 0;
                if (RadioEndDate.IsChecked == true)
                {
                    policy.DurationEnd = TextEndDateTime.Text;
                }
                else if (RadioNoEndDate.IsChecked == true)
                {
                    policy.DurationEnd = "2099-12-31 23:59:59";
                }
                if (iAddOrModify == (int)OperationType.Modify)
                {
                    bool bo = CommonFunctions.DateTimeToNumber(DateTime.Parse(policy.DurationEnd), ref lEndTime);
                    if (lEndTime == updateEntry.PolicyEndTime)
                    {
                        updateEntry.IsUpdateEndTime = false;
                    }
                    else
                    {
                        updateEntry.IsUpdateEndTime = true;
                        updateEntry.EndTime = CommonFunctions.StringToDateTime(lEndTime.ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                    }
                }
            }
            policy.PolicyNotes = TextDescription.Text;
            return true;
        }
        #endregion

        #region 控件加载
        /// <summary>
        /// 加载加密类型为“用户输入”时的控件
        /// </summary>
        private void PolicyTypeIsU()
        {
            LabelFrequency.Content = CurrentApp.GetLanguageInfo("2402003", "Custom (user input)");
            IPagePolicyTypeU.IPageParent = this;
            KeyTypeFrame.NavigationService.Navigate(IPagePolicyTypeU);
            // IPagePolicyTypeU.ShowTypeInformation(InstanceListStrPolicyInformation);
            RadioBeginDate.IsChecked = true;

            RadioEndDate.IsEnabled = false;
            TextEndDateTime.IsEnabled = false;
            RadioNoEndDate.IsEnabled = false;
            RadioNoEndDate.IsChecked = true;

            TabOjbect02.Visibility = Visibility.Collapsed;
            CheckEnabledComplexity.IsChecked = false;
        }

        /// <summary>
        /// 加载加密类型为“自动生成”时的控件
        /// </summary>
        private void PolicyTypeIsC()
        {
            LabelFrequency.Content = CurrentApp.GetLanguageInfo("2402002", "Periodic update key (randomly generated)");
            IPagePolicyTypeC.IPageParent = this;
            KeyTypeFrame.NavigationService.Navigate(IPagePolicyTypeC);

            //  IPagePolicyTypeC.ShowTypeInformation();
            RadioEndDate.IsChecked = true;
            RadioEndDate.IsEnabled = true;
            TextEndDateTime.IsEnabled = true;
            RadioNoEndDate.IsEnabled = true;
            RadioNoEndDate.IsChecked = false;

            TabOjbect02.Visibility = Visibility.Visible;
            CheckEnabledComplexity.IsChecked = true;

            TextMaxLength.Text = "8";
        }

        /// <summary>
        /// 强制密钥复杂性中textbox的文本检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextNumbers_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReCountKeyMinLenght();
        }

        /// <summary>
        /// textbox检查输入文本正确性
        /// </summary>
        private void ReCountKeyMinLenght()
        {
            string LStrNumbersUppercase = "0", LStrNumbersLowercase = "0", LStrNumbersDigital = "0", LStrNumbersSpecial = "0";
            int LIntNumbersUppercase = 0, LIntNumbersLowercase = 0, LIntNumbersDigital = 0, LIntNumbersSpecial = 0;

            string LStrExistMin = string.Empty, LStrExistMax = string.Empty;
            int LIntExistMin = 0, LIntExistMax = 0;

            int LIntReCountLen = 0;

            if (CheckMustContainUppercase.IsChecked == true)
            {
                LStrNumbersUppercase = TextNumbersUppercase.Text.Trim();
                if (!int.TryParse(LStrNumbersUppercase, out LIntNumbersUppercase)) { LIntNumbersUppercase = 1; }
                if (LIntNumbersUppercase <= 1) { LIntNumbersUppercase = 1; TextNumbersUppercase.Text = "1"; }
            }
            if (CheckMustContainLowercase.IsChecked == true)
            {
                LStrNumbersLowercase = TextNumbersLowercase.Text.Trim();
                if (!int.TryParse(LStrNumbersLowercase, out LIntNumbersLowercase)) { LIntNumbersLowercase = 1; }
                if (LIntNumbersLowercase <= 1) { LIntNumbersLowercase = 1; TextNumbersLowercase.Text = "1"; }
            }
            if (CheckMustContainDigital.IsChecked == true)
            {
                LStrNumbersDigital = TextNumbersDigital.Text.Trim();
                if (!int.TryParse(LStrNumbersDigital, out LIntNumbersDigital)) { LIntNumbersDigital = 1; }
                if (LIntNumbersDigital <= 1) { LIntNumbersDigital = 1; TextNumbersDigital.Text = "1"; }
            }
            if (CheckMustContainSpecial.IsChecked == true)
            {
                LStrNumbersSpecial = TextNumbersSpecial.Text.Trim();
                if (!int.TryParse(LStrNumbersSpecial, out LIntNumbersSpecial)) { LIntNumbersSpecial = 1; }
                if (LIntNumbersSpecial <= 1) { LIntNumbersSpecial = 1; TextNumbersSpecial.Text = "1"; }
            }

            LIntReCountLen = LIntNumbersUppercase + LIntNumbersLowercase + LIntNumbersDigital + LIntNumbersSpecial;
            if (LIntReCountLen < 6) { LIntReCountLen = 6; }

            LStrExistMin = TextMinLength.Text.Trim();
            LStrExistMax = TextMaxLength.Text.Trim();

            if (!int.TryParse(LStrExistMin, out LIntExistMin)) { LIntExistMin = LIntReCountLen; TextMinLength.Text = LIntReCountLen.ToString(); }
            if (!int.TryParse(LStrExistMax, out LIntExistMax)) { LIntExistMax = LIntReCountLen; TextMaxLength.Text = LIntExistMin.ToString(); }

            if (LIntExistMax < LIntExistMin) { LIntExistMax = LIntExistMin; TextMaxLength.Text = LIntExistMax.ToString(); }

            if (LIntReCountLen > LIntExistMin) { TextMinLength.Text = LIntReCountLen.ToString(); }
            if (LIntReCountLen > LIntExistMax) { TextMaxLength.Text = LIntReCountLen.ToString(); }
        }

        private void ButtonControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrDateTime = string.Empty;
            string LStrSenderName = string.Empty;

            try
            {
                Microsoft.Windows.Controls.DatePicker LDateTimeSender = sender as Microsoft.Windows.Controls.DatePicker;
                LStrSenderName = LDateTimeSender.Name;
                if (LStrSenderName == "StartDate" && TextStartDateTime.IsEnabled == true)
                {
                    LStrDateTime = TextStartDateTime.Text.Substring(11);
                    TextStartDateTime.Text = LDateTimeSender.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + LStrDateTime;
                }
                if (LStrSenderName == "StartDate" && TextStartDateTime.IsEnabled == false)
                {
                    LStrDateTime = TextStartDateTime.Text.Substring(0, 10) + " 00:00:00";
                    LDateTimeSender.SelectedDate = Convert.ToDateTime(LStrDateTime);
                }
                if (LStrSenderName == "EndDate" && TextEndDateTime.IsEnabled == true)
                {
                    LStrDateTime = TextEndDateTime.Text.Substring(11);
                    TextEndDateTime.Text = LDateTimeSender.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + LStrDateTime;
                }
                if (LStrSenderName == "EndDate" && TextEndDateTime.IsEnabled == false)
                {
                    LStrDateTime = TextEndDateTime.Text.Substring(0, 10) + " 00:00:00";
                    LDateTimeSender.SelectedDate = Convert.ToDateTime(LStrDateTime);
                }
            }
            catch { }
        }
        #endregion

        #region 写修改策略的操作日志
        /// <summary>
        /// 写修改策略的操作日志(仅用于操作成功时)
        /// </summary>
        /// <param name="boOptResult"></param>
        /// <param name="PolicyBefore"></param>
        /// <param name="policyAfter"></param>
        private void WriteUpdatePolicyLog()
        {
            string strMsg = CurrentApp.Session.UserInfo.UserName;
            strMsg += Utils.FormatOptLogString("2402023") + " ; " + updateEntry.PolicyName + " ; ";
            if (updateEntry.IsUpdatePwd)
            {
                strMsg += Utils.FormatOptLogString("2402020") + " ; ";
            }
            if (updateEntry.IsResetCycle)
            {
                strMsg += Utils.FormatOptLogString("2402021") + " ; ";
            }
            if (updateEntry.IsUpdateEndTime)
            {
                strMsg += Utils.FormatOptLogString("2402022") + " : " + CommonFunctions.StringToDateTime(updateEntry.PolicyEndTime.ToString()) + " --- " + updateEntry.EndTime + " ; ";
            }
            strMsg += Utils.FormatOptLogString("2402024") + updateEntry.EffectTime + " ; ";
            CurrentApp.WriteOperationLog("2402004", ConstValue.OPT_RESULT_SUCCESS, strMsg);
        }
        #endregion

    }
}
