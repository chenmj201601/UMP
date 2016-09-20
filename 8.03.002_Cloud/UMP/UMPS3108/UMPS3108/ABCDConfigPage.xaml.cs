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
using UMPS3108.Models;
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Communications;
using UMPS3108.Wcf31081;
using VoiceCyber.UMP.Common;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31081;
//using System.Drawing;

namespace UMPS3108
{
    /// <summary>
    /// ABCDConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class ABCDConfigPage
    {
        public SCMainView PageGrandParent;
        public OrgPage ParentPage;
        public ObjectItem OrgItem;
        public string OperationCode;
        private BackgroundWorker mWorder;

        private ObservableCollection<StatisticParamModel> ListABCD;
        //private ObservableCollection<StatisticParam> mListABCD;

        private List<StatisticalParamItem> ListABCDItems;
        private ObservableCollection<StatisticalParamItemModel> mListABCDItems;

        private StatisticParamModel Statistic;

        private List<string> ListTransParams;

        private ObservableCollection<string> ComboxListWeekUnit;
        private ObservableCollection<string> ComboxListMonUnit;
        private ObservableCollection<string> ComboxListUpdateUnit;
        private ObservableCollection<string> ComboxListCycleUnit;
        private ObservableCollection<string> ComboxListYearUnit;
        private ObservableCollection<string> ComboxListCycleTime;

        public ABCDConfigPage()
        {
            InitializeComponent();

            ListABCD = new ObservableCollection<StatisticParamModel>();
            ListABCDItems = new List<StatisticalParamItem>();
            //mListABCD = new ObservableCollection<StatisticParam>();
            mListABCDItems = new ObservableCollection<StatisticalParamItemModel>();
            Statistic = new StatisticParamModel();

            ListTransParams = new List<string>();
            ComboxListWeekUnit = new ObservableCollection<string>();
            ComboxListMonUnit = new ObservableCollection<string>();
            ComboxListUpdateUnit = new ObservableCollection<string>();
            ComboxListYearUnit = new ObservableCollection<string>();
            ComboxListCycleUnit = new ObservableCollection<string>();
            ComboxListCycleTime = new ObservableCollection<string>();

            Loaded += ABCDConfigPage_Loaded;

            this.ListBoxStatisticItems.ItemsSource = mListABCDItems;
            this.CombABCD.ItemsSource = ListABCD;
            this.CycleUnit.ItemsSource = ComboxListCycleUnit;
            this.UpdateUnit.ItemsSource = ComboxListUpdateUnit;
            this.CycleTime.ItemsSource = ComboxListCycleTime;
            //    this.CBWeek.ItemsSource = ComboxListWeekUnit;
            //    this.CBMon.ItemsSource = ComboxListMonUnit;
        }

        void ABCDConfigPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.BtnClose.Click += BtnClose_Click;
            this.BtnConfirm.Click += BtnConfirm_Click;
            this.CombABCD.SelectionChanged += CombABCD_SelectionChanged;
            this.CycleUnit.SelectionChanged += CycleUnit_SelectionChanged;
            this.ParallelWay.Click += ParallelWay_Click;
            this.DrillWay.Click += DrillWay_Click;

            Init();
            ChangeLanguage();
        }

        void DrillWay_Click(object sender, RoutedEventArgs e)
        {
            if (this.DrillWay.IsChecked == true)
            {
                this.StartCheck.IsEnabled = false;
                this.StopCheck.IsEnabled = false;
                //this.StartCheck.IsChecked = false;
                //this.StopCheck.IsChecked = false;
            }
        }

        void ParallelWay_Click(object sender, RoutedEventArgs e)
        {
            if (this.ParallelWay.IsChecked == true)
            {
                this.StartCheck.IsEnabled = true;
                this.StopCheck.IsEnabled = true;
                //this.StartCheck.IsChecked = false;
                this.StopCheck.IsChecked = false;
            }
        }

        private void CycleUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int Code = this.CycleUnit.SelectedIndex;
            //选择不同，加载的内容不一样。
            switch (Code)
            {
                case 0:
                    //如果是天，则隐藏
                    this.CycleDay.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    //如果是周，加载周几
                    this.CycleDay.Visibility = Visibility.Visible;
                    this.CycleDay.ItemsSource = ComboxListWeekUnit;
                    break;
                case 2:
                    //如果是月，加载几号
                    this.CycleDay.Visibility = Visibility.Visible;
                    this.CycleDay.ItemsSource = ComboxListMonUnit;
                    break;
                case 3:
                    //如果是年，循环加载1-355
                    this.CycleDay.Visibility = Visibility.Visible;
                    for (int i = 1; i <= 365; i++)
                    {
                        ComboxListYearUnit.Add(i.ToString());
                    }
                    this.CycleDay.ItemsSource = ComboxListYearUnit;
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            ListABCD.Clear();
            //mListABCD.Clear();
            ComboxListWeekUnit.Clear();
            ComboxListUpdateUnit.Clear();
            if (PageGrandParent != null)
            {
                PageGrandParent.SetBusy(true, string.Empty);
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) =>
            {
                //初始化加载combox还有小项的list
                InitListABCD();
                //加载ListABCDItems的值
                //InitListABCDItem();
                InitPageCombox();
            };
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                ControlPage();
                mWorder.Dispose();
                if (PageGrandParent != null)
                {
                    PageGrandParent.SetBusy(false, string.Empty);
                }
            };
            mWorder.RunWorkerAsync();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //点击确定后，对数据库进行写入
                CreateConditionResultList();
                SaveConfig();
                ParentPage.InitListBox();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CombABCD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Statistic = this.CombABCD.SelectedItem as StatisticParamModel;
                if (Statistic != null)
                {
                    OperationCode = Statistic.StatisticalParamID.ToString();
                    InitListABCDItem();
                    InitmListABCDItems();//获取大项下的所有小项。

                    StatisticConfigDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Close()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void InitListABCD()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetABCDList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("1");
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitListABCD Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitListABCD Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticParam StatisticP = optReturn.Data as StatisticParam;
                    string Name = StatisticP.StatisticalParamID.ToString();
                    Name = Name.Substring(Name.Length - 1);
                    StatisticP.StatisticalParamName = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}", Name), StatisticP.StatisticalParamName);
                    StatisticP.Description = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}", Name), StatisticP.StatisticalParamName);
                    if (StatisticP != null)
                    {
                        Statistic = new StatisticParamModel(StatisticP);
                        //ListABCD.Add(Statistic);
                        Dispatcher.Invoke(new Action(() => { ListABCD.Add(Statistic); }));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitListABCDItem()
        {
            if (OperationCode.Length < 19) { return; }
            ListABCDItems.Clear();
            //mListABCDItems.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetABCDItemList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(OperationCode);
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitListABCDItem Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControledUsers Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalParamItem StatisticP = optReturn.Data as StatisticalParamItem;
                    StatisticP.StatisticalParamItemName = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", StatisticP.StatisticalParamItemID), StatisticP.StatisticalParamItemName);
                    StatisticP.Description = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", StatisticP.StatisticalParamItemID), StatisticP.StatisticalParamItemName);
                    ListABCDItems.Add(StatisticP);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitmListABCDItems()
        {
            try
            {
                if (mListABCDItems == null)
                {
                    return;
                }
                mListABCDItems.Clear();

                //string ItemID = Statistic.StatisticalParamID.ToString();
                //List<StatisticalParamItem> ItemParamTemplateList = (from item in ListABCDItems
                //                                                  where item.StatisticalParamID.ToString() == ItemID
                //                                                  orderby item.SortID
                //                                                  select item).ToList();
                if (ListABCDItems == null || ListABCDItems.Count == 0) { return; }

                for (int i = 0; i < ListABCDItems.Count; i++)
                {
                    StatisticalParamItemModel ItemModel = new StatisticalParamItemModel(ListABCDItems[i], CurrentApp);
                    Dispatcher.Invoke(new Action(() => { mListABCDItems.Add(ItemModel); }));
                }

                InitStatisticConfig();//获取大项，如果绑定了机构的话。更新现有大项(52+26)
                if (Statistic == null) { return; }
                if (Statistic.StatisticKey != 0)
                {
                    try
                    {
                        //(44)
                        WebRequest webRequest = new WebRequest();
                        webRequest.Code = (int)S3108Codes.GetOrgItemRelation;
                        webRequest.Session = CurrentApp.Session;
                        webRequest.ListData.Add(Statistic.StatisticKey.ToString());
                        Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                        WebHelper.SetServiceClient(client);
                        //Service31081Client client = new Service31081Client();
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("InitmListABCDItems Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i += 2)
                        {
                            string Code = webReturn.ListData[i];
                            string Value = webReturn.ListData[i + 1];

                            for (int j = 0; j < mListABCDItems.Count; j++)
                            {
                                if (Code == mListABCDItems[j].StatisticalParamItemID.ToString())
                                {
                                    string[] StrValue = mListABCDItems[j].Value.Split('?');
                                    mListABCDItems[j].Value = string.Format("{0}?{1}?{2}", Value, StrValue[1], StrValue[2]);
                                    mListABCDItems[j].IsUsed = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            
            
        }

        private void InitStatisticConfig()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetABCDInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(OrgItem.ItemID.ToString());
                //webRequest.ListData.Add(Statistic.StatisticKey.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitStatisticConfig Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitStatisticConfig Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticParam SP = optReturn.Data as StatisticParam;
                    int index;
                    for (index = 0; index < ListABCD.Count(); index++)
                    {
                        if (ListABCD[index].StatisticalParamID == SP.StatisticalParamID)
                        {
                            ListABCD[index].StatisticKey = SP.StatisticKey;
                            ListABCD[index].OrgID = SP.OrgID;
                            ListABCD[index].StatisticType = SP.StatisticType;
                            ListABCD[index].IsApplyAll = SP.IsApplyAll;
                            ListABCD[index].StartTime = Convert.ToDateTime(SP.StartTime);
                            ListABCD[index].EndTime = Convert.ToDateTime(SP.EndTime);
                            ListABCD[index].RowNum = SP.RowNum;
                            ListABCD[index].CycleTime = SP.CycleTime;
                            ListABCD[index].CycleTimeParam = SP.CycleTimeParam;
                            ListABCD[index].UpdateTime = SP.UpdateTime;
                            ListABCD[index].UpdateUnit = SP.UpdateUnit;
                            break;
                        }
                    }
                    //Statistic.StatisticKey = SP.StatisticKey;
                    //Statistic.OrgID = SP.OrgID;
                    //Statistic.StatisticType = SP.StatisticType;
                    //Statistic.IsApplyAll = SP.IsApplyAll;
                    //Statistic.StartTime = SP.StartTime;
                    //Statistic.EndTime = SP.EndTime;
                    //Statistic.RowNum = SP.RowNum;
                    //Statistic.CycleTime = SP.CycleTime;
                    //Statistic.CycleTimeParam = SP.CycleTimeParam;
                    //Statistic.UpdateTime = SP.UpdateTime;
                    //Statistic.UpdateUnit = SP.UpdateUnit;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPageCombox()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ComboxListWeekUnit.Clear();
                ComboxListMonUnit.Clear();
                ComboxListUpdateUnit.Clear();
                ComboxListCycleUnit.Clear();
                ComboxListCycleTime.Clear();
                //CycleUnitWeek
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC009", "周一"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC010", "周二"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC011", "周三"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC012", "周四"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC013", "周五"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC014", "周六"));
                ComboxListWeekUnit.Add(CurrentApp.GetLanguageInfo("310802PC015", "周天"));
                //CycleUnitWeek
                ComboxListMonUnit.Add("1"); ComboxListMonUnit.Add("2");
                ComboxListMonUnit.Add("3"); ComboxListMonUnit.Add("4");
                ComboxListMonUnit.Add("5"); ComboxListMonUnit.Add("6");
                ComboxListMonUnit.Add("7"); ComboxListMonUnit.Add("8");
                ComboxListMonUnit.Add("9"); ComboxListMonUnit.Add("10");
                ComboxListMonUnit.Add("11"); ComboxListMonUnit.Add("12");
                ComboxListMonUnit.Add("13"); ComboxListMonUnit.Add("14");
                ComboxListMonUnit.Add("15"); ComboxListMonUnit.Add("16");
                ComboxListMonUnit.Add("17"); ComboxListMonUnit.Add("18");
                ComboxListMonUnit.Add("19"); ComboxListMonUnit.Add("20");
                ComboxListMonUnit.Add("21"); ComboxListMonUnit.Add("22");
                ComboxListMonUnit.Add("23"); ComboxListMonUnit.Add("24");
                ComboxListMonUnit.Add("25"); ComboxListMonUnit.Add("26");
                ComboxListMonUnit.Add("27"); ComboxListMonUnit.Add("28");
                ComboxListMonUnit.Add(CurrentApp.GetLanguageInfo("310802PC016", "每月倒数第三天"));
                ComboxListMonUnit.Add(CurrentApp.GetLanguageInfo("310802PC017", "每月倒数第二天"));
                ComboxListMonUnit.Add(CurrentApp.GetLanguageInfo("310802PC018", "每月倒数第一天"));
                //UpdateUnit
                ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC004", "年a"));
                ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC003", "月"));
                //ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC005", "工作月"));
                //ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC006", "工作周"));
                ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC002", "周"));
                ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC001", "天"));
                //ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC007", "小时"));
                //ComboxListUpdateUnit.Add(CurrentApp.GetLanguageInfo("310802PC008", "分钟"));

                ComboxListCycleUnit.Add(CurrentApp.GetLanguageInfo("310802P013", "Day"));
                ComboxListCycleUnit.Add(CurrentApp.GetLanguageInfo("310802P014", "Week"));
                ComboxListCycleUnit.Add(CurrentApp.GetLanguageInfo("310802P015", "Month"));
                ComboxListCycleUnit.Add(CurrentApp.GetLanguageInfo("310802P016", "Year"));

                this.CycleUnit.SelectedIndex = 0;

                ComboxListCycleTime.Add("00:00:00");
                ComboxListCycleTime.Add("01:00:00");
                ComboxListCycleTime.Add("02:00:00");
                ComboxListCycleTime.Add("03:00:00");
                ComboxListCycleTime.Add("04:00:00");
                ComboxListCycleTime.Add("05:00:00");
                ComboxListCycleTime.Add("06:00:00");
                ComboxListCycleTime.Add("07:00:00");
                ComboxListCycleTime.Add("08:00:00");
                ComboxListCycleTime.Add("09:00:00");
                ComboxListCycleTime.Add("10:00:00");
                ComboxListCycleTime.Add("11:00:00");
                ComboxListCycleTime.Add("12:00:00");
                ComboxListCycleTime.Add("13:00:00");
                ComboxListCycleTime.Add("14:00:00");
                ComboxListCycleTime.Add("15:00:00");
                ComboxListCycleTime.Add("16:00:00");
                ComboxListCycleTime.Add("17:00:00");
                ComboxListCycleTime.Add("18:00:00");
                ComboxListCycleTime.Add("19:00:00");
                ComboxListCycleTime.Add("20:00:00");
                ComboxListCycleTime.Add("21:00:00");
                ComboxListCycleTime.Add("22:00:00");
                ComboxListCycleTime.Add("23:00:00");
            }));
        }

        private void StatisticConfigDisplay()
        {
            try
            {
                if (Statistic.OrgID != 0)
                {
                    //绑值 Statistic
                    this.UpdateTime.Text = Statistic.UpdateTime.ToString();
                    this.UpdateUnit.SelectedIndex = Statistic.UpdateUnit - 1;
                    switch (Statistic.CycleTimeParam[0])
                    {
                        case "D":
                            this.CycleUnit.SelectedIndex = 0;
                            this.CycleDay.Visibility = Visibility.Hidden;
                            break;
                        case "W":
                            this.CycleUnit.SelectedIndex = 1;
                            this.CycleDay.Visibility = Visibility.Visible;
                            this.CycleDay.SelectedIndex = int.Parse(Statistic.CycleTimeParam[2]) - 1;
                            break;
                        case "M":
                            this.CycleUnit.SelectedIndex = 2;
                            this.CycleDay.Visibility = Visibility.Visible;
                            int monnum = int.Parse(Statistic.CycleTimeParam[2]);
                            if (monnum < 0)
                            {
                                monnum += 32;
                            }
                            this.CycleDay.SelectedIndex = monnum - 1;
                            break;
                        case "Y":
                            this.CycleUnit.SelectedIndex = 3;
                            this.CycleDay.Visibility = Visibility.Visible;
                            this.CycleDay.Text = Statistic.CycleTimeParam[2];
                            break;
                        default:
                            break;
                    }

                    //this.Time.Text = Convert.ToDateTime(Statistic.CycleTimeParam[1]).ToLocalTime().ToString();//转UTC时间。31-052.C009运行周期
                    this.CycleTime.Text = Statistic.CycleTimeParam[1];
                    //this.ST.Text = Convert.ToDateTime(Statistic.StartTime.ToString()).ToLocalTime().ToString();
                    //this.ET.Text = Convert.ToDateTime(Statistic.EndTime.ToString()).ToLocalTime().ToString();
                    if (Statistic.StatisticType == 1)
                    {
                        this.ParallelWay.IsChecked = true;
                    }
                    else
                    {
                        this.DrillWay.IsChecked = true;
                    }
                    if (Statistic.IsApplyAll == 1)
                    {
                        this.StartCheck.IsChecked = true;
                    }
                    else
                    {
                        this.StopCheck.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private StatisticParam GetValueFromPage()
        {
            StatisticParam SP = new StatisticParam();
            if (!CheckValues())
            {
                //ShowInformation(CurrentApp.GetLanguageInfo("310802T001", "填入参数不全"));
                return SP;
            }
            SP.StatisticalParamID = Statistic.StatisticalParamID;
            SP.StatisticalParamName = Statistic.StatisticalParamName;
            SP.TableType = Statistic.TableType;
            SP.Description = Statistic.Description;
            SP.RowNum = Statistic.RowNum;
            SP.CycleTime = Statistic.CycleTime;
            SP.StatisticKey = Statistic.StatisticKey;
            SP.IsUsed = Statistic.IsUsed;
            SP.IsCombine = Statistic.IsCombine;
            SP.OrgID = Convert.ToInt64(OrgItem.ItemID);
            //SP.StartTime = Convert.ToDateTime(this.ST.Text).ToUniversalTime();
            //SP.EndTime = Convert.ToDateTime(this.ET.Text).ToUniversalTime();
            SP.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SP.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SP.UpdateTime = Convert.ToInt32(this.UpdateTime.Text);
            SP.UpdateUnit = (this.UpdateUnit.SelectedIndex + 1);
            if (this.DrillWay.IsChecked == true)
            {
                SP.StatisticType = 2;
            }
            else { SP.StatisticType = 1; }
            if (this.StartCheck.IsChecked == true)
            {
                SP.IsApplyAll = 1;
            }
            else { SP.IsApplyAll = 2; }
            string temp = string.Empty;
            string daynum = string.Empty;
            switch (this.CycleUnit.SelectedIndex)
            {
                case 0:
                    temp = "D";
                    break;
                case 1:
                    temp = "W";
                    daynum = (this.CycleDay.SelectedIndex + 1).ToString();
                    break;
                case 2:
                    temp = "M";
                    daynum = (this.CycleDay.SelectedIndex + 1).ToString();
                    if (int.Parse(daynum) > 28)
                    {
                        daynum = (int.Parse(daynum) - 32).ToString();
                    }
                    break;
                case 3:
                    temp = "Y";
                    daynum = (this.CycleDay.SelectedIndex + 1).ToString();
                    break;
                default:
                    break;
            }
            SP.CycleTimeParam.Add(temp);
            SP.CycleTimeParam.Add(this.CycleTime.Text);
            //SP.CycleTimeParam.Add(Convert.ToDateTime(this.Time.Text).ToUniversalTime().ToString());
            SP.CycleTimeParam.Add(daynum);

            return SP;
        }

        //这个就是把界面上的查询条件（后来存入了 mListUCConditionItems 里面）,放到mListQueryConditionDetails里面
        private void CreateConditionResultList()
        {
            try
            {
                ListTransParams.Clear();
                StatisticParam SP = GetValueFromPage();
                if (SP.StatisticalParamID == 0) { return; }
                OperationReturn optReturn = XMLHelper.SeriallizeObject<StatisticParam>(SP);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("CreateConditionResultList Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string StatisticC = optReturn.Data as string;
                ListTransParams.Add(StatisticC);

                for (int i = 0; i < mListABCDItems.Count; i++)
                {
                    StatisticalParamItemModel ItemModel = mListABCDItems[i];
                    ItemModel.abcdItem.CurrentApp = CurrentApp;
                    ItemModel.abcdItem.GetValue();
                    StatisticalParamItem SPI = ItemModel.abcdItem.ResultValues;
                    if (SPI.IsUsed == true && (SPI.Value == null || SPI.Value.Replace(" ", "") == string.Empty))
                    {
                        //勾选了，但是没有写值
                        CurrentApp.GetLanguageInfo("3108T003", "勾选了，但是没有写值");
                        return;
                    }
                    if (SPI.IsUsed == false) { continue; }
                    optReturn = XMLHelper.SeriallizeObject<StatisticalParamItem>(SPI);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("CreateConditionResultList Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticC = optReturn.Data as string;
                    ListTransParams.Add(StatisticC);
                    //ListABCDItems更新
                    int index = ListABCDItems.FindIndex(p => p.StatisticalParamItemID == SPI.StatisticalParamItemID);
                    ListABCDItems[index] = SPI;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveConfig()
        {
            try
            {
                if (ListTransParams.Count == 0) { return; }
                if (ListTransParams.Count == 1) { ShowInformation(CurrentApp.GetLanguageInfo("310802T002", "没有小项")); return; }
                string msg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.SaveConfig;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData = ListTransParams;
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowInformation(string.Format("{0}\n{1}", CurrentApp.GetLanguageInfo("3108T002", "保存fail"),
                        string.Format("SaveConfig Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));

                    #region 记录日志
                    msg = string.Format("{0}{1}{2}:{3}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108006")), OrgItem.Name, Statistic.StatisticalParamName);
                    CurrentApp.WriteOperationLog("3108006", ConstValue.OPT_RESULT_FAIL, msg);
                    #endregion
                    return;
                }
                // 0    大项编号
                // 1    cycletime
                // 2    rowname
                // 3    statisticKey
                int index = 0;
                string ID = webReturn.ListData[0];
                for (index = 0; index < ListABCD.Count(); index++)
                {
                    if (ListABCD[index].StatisticalParamID.ToString() == ID && ListABCD[index].StatisticKey == 0)
                    {
                        ListABCD[index].CycleTime = Convert.ToInt64(webReturn.ListData[2]);
                        ListABCD[index].RowNum = Convert.ToInt32(webReturn.ListData[1]);
                        ListABCD[index].StatisticKey = Convert.ToInt64(webReturn.ListData[3]);
                    }
                }
                ShowInformation(CurrentApp.GetLanguageInfo("3108T001", "保存成功"));
                #region 记录日志
                msg = string.Format("{0}{1}{2}:{3}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108006")), OrgItem.Name, Statistic.StatisticalParamName);
                CurrentApp.WriteOperationLog("3108006", ConstValue.OPT_RESULT_SUCCESS, msg);
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool CheckValues()
        {
            if (!(this.UpdateTime.Text == "" || this.UpdateTime.Text == "" || this.UpdateUnit.SelectedItem == null
                //|| this.ST.Text == null || ET.Text == null
                || (this.ParallelWay.IsChecked == false && this.DrillWay.IsChecked == false)
                //|| (this.StartCheck.IsChecked == false && this.StopCheck.IsChecked == false))
                || this.CycleUnit.SelectedItem == null))
            {
                //写统计时间的输入值判断
                if (this.CycleDay.IsVisible)
                {
                    if (this.CycleDay.SelectedItem == null)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3108T003", "填入参数不全"));
                        return false;
                    }
                }
                //判断是否启用全部机构使用，在选择了平行统计的情况下
                if (this.ParallelWay.IsChecked == true)
                {
                    if (this.StopCheck.IsChecked == false && this.StartCheck.IsChecked == false)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3108T003", "填入参数不全"));
                        return false;
                    }
                }
                //判断时间：打标时长（更新时长）>=运行周期（统计周期）
                int UpdateDays = 0;
                int CycleDays = 0;
                switch (this.CycleUnit.SelectedIndex)
                {
                    case 0:
                        CycleDays = 1;
                        break;
                    case 1:
                        CycleDays = 7;
                        break;
                    case 2:
                        CycleDays = 31;
                        break;
                    case 3:
                        CycleDays = 366;
                        break;
                    default:
                        break;
                }
                int day = int.TryParse(this.UpdateTime.Text, out day) ? day : 0;
                if (day == 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3108T003", "填入参数不全"));
                    return false;
                }
                switch (this.UpdateUnit.SelectedIndex)
                {
                    case 0:
                        UpdateDays = 366;
                        break;
                    case 1:
                        UpdateDays = 31;
                        break;
                    case 2:
                        UpdateDays = 7;
                        break;
                    case 3:
                        UpdateDays = 1;
                        break;
                    default:
                        break;
                }
                UpdateDays *= day;
                if (UpdateDays < CycleDays)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("310802T001", "更新时长要大于运行周期"));
                    return false;
                }
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3108T003", "填入参数不全"));
                return false;
            }
            return true;
        }

        private void ControlPage()
        {
            if (OrgItem.ItemID == null || OrgItem.ItemID.Length < 3) { return; }
            this.ParallelWay.IsChecked = true;
            this.StopCheck.IsChecked = true;
            if (OrgItem.ItemID.Substring(0, 3) != "101")
            {
                this.ParallelWay.IsEnabled = false;
                this.DrillWay.IsEnabled = false;
                this.StartCheck.IsEnabled = false;
                this.StopCheck.IsEnabled = false;
            }
        }

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                this.TxtGBHeader.Text = CurrentApp.GetLanguageInfo("310802P002", "统计配置a");
                this.TexCycleTime.Text = CurrentApp.GetLanguageInfo("310802P003", "统计运行周期a");
                this.TexUpdate.Text = CurrentApp.GetLanguageInfo("310802P004", "更新数据时长");
                //this.TexSTime.Text = CurrentApp.GetLanguageInfo("310802P005", "起效时间");
                //this.TexETime.Text = CurrentApp.GetLanguageInfo("310802P006", "失效时间");
                this.TexApplyAll.Text = CurrentApp.GetLanguageInfo("310802P008", "是否应用于所有机构");
                this.TexStatisticWay.Text = CurrentApp.GetLanguageInfo("310802P007", "统计方式");
                this.Parallel.Text = CurrentApp.GetLanguageInfo("310802P009", "平行");
                this.Drill.Text = CurrentApp.GetLanguageInfo("310802P010", "钻取");
                this.Start.Text = CurrentApp.GetLanguageInfo("310802P011", "启用");
                this.Stop.Text = CurrentApp.GetLanguageInfo("310802P012", "禁用");
                this.BtnConfirm.Content = CurrentApp.GetLanguageInfo("310802B002", "绑定");
                this.BtnClose.Content = CurrentApp.GetLanguageInfo("310802B003", "取消");

                InitPageCombox();
                //下拉combox更新语言
                for (int i = 0; i < ListABCD.Count(); i++)
                {
                    string temp = ListABCD[i].StatisticalParamID.ToString();
                    temp = temp.Substring(temp.Length - 1);
                    ListABCD[i].StatisticalParamName = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}", temp), "");
                    ListABCD[i].Description = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}", temp), "");
                }

                InitListABCDItem();
                InitmListABCDItems();
            }
            catch (Exception ex)
            {

            }
        }

    }
}