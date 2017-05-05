using Common61011;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using UMPS1101.Models;
using UMPS6101.Models;
using UMPS6101.SharingClasses;
using UMPS6101.Wcf61012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS6101.ReportQueryPage
{
    /// <summary>
    /// Rp22query.xaml 的交互逻辑
    /// </summary>
    public partial class Rp22query
    {
        #region Memembers

        //定义一个变量,和checkbox勾选有关的变量
        public int callin;
        public int callout;

        public ReportDisplayPage PageParent;

        public AboutDateTime SendDate;

        public ObjectItem mRootItem;
        public ObjectItem mRootItem1;
        public ObjectItem itemRootKW;

        private List<ObjectItem> mListSelectedObjects;//树里面勾中选项的内容
        private List<ObjectItem> mListSelectedObjects1;

        private ObservableCollection<QueryConditionInfo> mListQueryConditions;
        private BackgroundWorker mWorker;

        //时间控件
        private string IStrBeginTime = string.Empty;
        private string IStrEndTime = string.Empty;

        private bool IsAllExt = false;
        #endregion
        public S6101App S6101App;
        public Rp22query()
        {
            InitializeComponent();
            Loaded += Rp22KeyWords_Loaded;
            Loaded += Rp22query_Loaded;
            mRootItem = new ObjectItem();
            mRootItem1 = new ObjectItem();
            itemRootKW = new ObjectItem();
            mListQueryConditions = new ObservableCollection<QueryConditionInfo>();

            mListSelectedObjects = new List<ObjectItem>();
            mListSelectedObjects1 = new List<ObjectItem>();

            //初始化checkbox控件的值

            ComboQueryConditions.ItemsSource = mListQueryConditions;

            this.ComboQueryConditions.SelectionChanged += ComboQueryConditions_SelectionChanged;
            this.CbSaveConditions.Checked += CbSaveConditions_Checked;

        }

        void Rp22query_Loaded(object sender, RoutedEventArgs e)
        {
            itemRootKW.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
            this.TypeOfReport.Visibility = Visibility.Collapsed;
            this.CBTypeOfReport.Visibility = Visibility.Collapsed;
            this.CallDirection.Visibility = Visibility.Collapsed;
            this.RButn1.Visibility = Visibility.Collapsed;
            this.RButn2.Visibility = Visibility.Collapsed;
            InitQueryCondition();
            ChangeLanguage();
        }

        void CbSaveConditions_Checked(object sender, RoutedEventArgs e)
        {
            this.ComboQueryConditions.SelectedIndex = -1;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            //公共部分
            TabRoutine.Header = CurrentApp.GetLanguageInfo("610100000001", "常规");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("610100000002", "确定");
            BtnClose.Content = CurrentApp.GetLanguageInfo("610100000003", "关闭");
            TD.Content = CurrentApp.GetLanguageInfo("610100000004", "今天");
            TK.Content = CurrentApp.GetLanguageInfo("610100000005", "本周");
            TM.Content = CurrentApp.GetLanguageInfo("610100000006", "本月");
            LM.Content = CurrentApp.GetLanguageInfo("610100000007", "上月");
            L3M.Content = CurrentApp.GetLanguageInfo("610100000008", "上三月");
            CR.Content = CurrentApp.GetLanguageInfo("610100000009", "自定义");
            TypeOfReport.Content = CurrentApp.GetLanguageInfo("610100000010", "报表类型");
            Default.Content = CurrentApp.GetLanguageInfo("610100000011", "默认");
            DayReport.Content = CurrentApp.GetLanguageInfo("610100000012", "日报");
            WeekReport.Content = CurrentApp.GetLanguageInfo("610100000013", "周报");
            MonthReport.Content = CurrentApp.GetLanguageInfo("610100000014", "月报");
            YearReport.Content = CurrentApp.GetLanguageInfo("610100000015", "年报");
            //自身部分
            Title.Content = CurrentApp.GetLanguageInfo("610100002201", "关键词次数分析查询条件");
            TimeL.Content = CurrentApp.GetLanguageInfo("610100000102", "录音时间");
            CallDirection.Content = CurrentApp.GetLanguageInfo("610100000103", "呼叫方向");
            KeyWords.Header = CurrentApp.GetLanguageInfo("610100002202", "关键词");
            //TabExtension.Header = CurrentApp.GetLanguageInfo("610100000107", "分机号码");
            //this.CBItemAgent.Content = CurrentApp.GetLanguageInfo("610100002003", "Agent");
            //this.CBItemExt.Content = CurrentApp.GetLanguageInfo("610100002004", "Ext");
            this.CbSaveConditions.Content = CurrentApp.GetLanguageInfo("61010103", "Save Conditions");
        }

        //加载关键词      
        void Rp22KeyWords_Loaded(object sender, RoutedEventArgs e)
        {
            CTKeyWord.ItemsSource = mRootItem1.Children;
            //this.CBDepartBasic.SelectedIndex = 0;
            mRootItem1.Children.Clear();
            itemRootKW.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitKeyWords();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem1.IsChecked = false;
                if (mRootItem1.Children.Count > 0)
                {
                    mRootItem1.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();
        }

        private void InitKeyWords()
        {
            AddChildObject(mRootItem1, itemRootKW);
            List<ObjectItem> ListKeyW = new List<ObjectItem>();
            if (S6101App.ds_keywords != null && S6101App.ds_keywords.Tables.Count != 0)
                foreach (DataRow dr in S6101App.ds_keywords.Tables[0].Rows)
                {
                    string KeyW = string.Empty;
                    KeyW = dr["A001"].ToString();
                    ObjectItem IsFind = ListKeyW.FirstOrDefault(p => p.Name == KeyW);
                    if (IsFind == null)
                    {
                        ObjectItem ObjItem = new ObjectItem();
                        ObjItem.Name = KeyW;
                        ObjItem.FullName = dr["A002"].ToString();
                        ObjItem.Data = dr;
                        ObjItem.DisplayContent = dr["A002"].ToString();
                        //ObjItem.Icon = "Images/computer.ico";
                        //ListKeyW.Add()
                        InitKeyWords(KeyW, ObjItem);
                        ListKeyW.Add(ObjItem);
                        AddChildObject(itemRootKW, ObjItem);
                    }
                }
        }

        private void InitKeyWords(string KeyW, ObjectItem ObjItem)
        {
            List<DataRow> ListRowKW = S6101App.ds_keywords.Tables[0].Select(string.Format("A001={0}", KeyW)).ToList();
            if (ListRowKW.Count != 0)
            {
                foreach (DataRow DRow in ListRowKW)
                {
                    ObjectItem ObjItemKWC = new ObjectItem();
                    //ObjItemKWC.ObjType = 4;
                    ObjItemKWC.Name = KeyW;
                    ObjItemKWC.FullName = DRow["C002"].ToString();
                    ObjItemKWC.Data = DRow;
                    ObjItemKWC.DisplayContent = DRow["C002"].ToString();
                    AddChildObject(ObjItem, ObjItemKWC);
                }
            }
        }

        private void GetCheckedKW()
        {
            try
            {
                mListSelectedObjects1.Clear();
                GetCheckedKW(itemRootKW);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }//这个函数是放到确定这个按钮的地方的
        private void GetCheckedKW(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_ORG)//
                        {
                            if (item.IsChecked == false) { continue; }
                            GetCheckedKW(item);
                        }
                        else
                        {
                            if (item.IsChecked == true)
                            {
                                mListSelectedObjects1.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //时间控件
        private void CBXLoggedTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrRangType = string.Empty;
            string LStrBegin = string.Empty, LStrEnd = string.Empty;

            try
            {
                UC_DateTime_Begin.IsEnabled = false;
                UC_DateTime_End.IsEnabled = false;

                ComboBoxItem RangeTypeItem = CBXLoggedTime.SelectedItem as ComboBoxItem;
                LStrRangType = RangeTypeItem.Tag.ToString();
                if (LStrRangType == "CR")
                {
                    UC_DateTime_Begin.IsEnabled = true;
                    UC_DateTime_End.IsEnabled = true;
                    IStrBeginTime = LStrBegin + " 00:00:00";
                    IStrEndTime = LStrBegin + " 23:59:59";
                }

                if (LStrRangType == "TD")
                {
                    IStrBeginTime = DateTime.Today.ToString("yyyy-MM-dd") + " 00:00:00";
                    IStrEndTime = DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59";
                }

                if (LStrRangType == "WK")
                {
                    CalendarFunctions.GetWeek(ref LStrBegin, ref LStrEnd);
                    IStrBeginTime = LStrBegin + " 00:00:00";
                    IStrEndTime = LStrEnd + " 23:59:59";
                }

                if (LStrRangType == "TM")
                {
                    CalendarFunctions.GetThisMonth(ref LStrBegin, ref LStrEnd);
                    IStrBeginTime = LStrBegin + " 00:00:00";
                    IStrEndTime = LStrEnd + " 23:59:59";
                }

                if (LStrRangType == "LM")
                {
                    CalendarFunctions.GetPriorMonth(ref LStrBegin, ref LStrEnd);
                    IStrBeginTime = LStrBegin + " 00:00:00";
                    IStrEndTime = LStrEnd + " 23:59:59";
                }

                if (LStrRangType == "L3M")
                {
                    CalendarFunctions.GetLastestThreeMonth(ref LStrBegin, ref LStrEnd);
                    IStrBeginTime = LStrBegin + " 00:00:00";
                    IStrEndTime = LStrEnd + " 23:59:59";
                }

                UC_DateTime_Begin.Value = Convert.ToDateTime(IStrBeginTime);
                UC_DateTime_End.Value = Convert.ToDateTime(IStrEndTime);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 其他
        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }
        #endregion

        #region 关于密的
        public static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }
        public static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }
        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }
        #endregion

        //处理包含不包含
        private void DealCheckbox()
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.PopupClose();
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DateTime DTBeginStr = (DateTime)UC_DateTime_Begin.Value;
            DateTime DTEndStr = (DateTime)UC_DateTime_End.Value;
            double sec = (DTEndStr - DTBeginStr).TotalSeconds;
            if (sec < 0)
            {
                 CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0010", "The time setting error"));
            }
            else
            {
                CollectData();
                SaveQueryCondition();
            }
        }

        public void CollectData()
        {
            DealCheckbox();
            InitCBTypeOfReport(); GetCheckedKW();
            List<string> QueryData = new List<string>();
            QueryData.Add(Convert.ToDateTime(UC_DateTime_Begin.Value).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            QueryData.Add(Convert.ToDateTime(UC_DateTime_End.Value).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            string KW = "KW";
            if (mListSelectedObjects1.Count != 0)
            {
                for (int i = 0; i < mListSelectedObjects1.Count; i++)
                {
                    KW += mListSelectedObjects1[i].DisplayContent + ";";
                }
                QueryData.Add(KW);
            }

            PageParent.ComfireClick(QueryData, SendDate);
        }

        //日报处理 将选择的时间截取为一天一天的形式  从即时起到当日 23:59分，开始时间也就这么算.
        void DayReportFunc()
        {
            SendDate = new AboutDateTime();
            DateTime DTBeginStr = ((DateTime)UC_DateTime_Begin.Value).ToUniversalTime();
            DateTime DTEndStr = ((DateTime)UC_DateTime_End.Value).ToUniversalTime();
            //标志来了
            SendDate.Sign = "D";
            //首先判断起始时间的日期是不是在同一天,如果在同一天,那么就生成一天的日报
            if (DateDiff(DTBeginStr, DTEndStr) == 0)
            {
                SendDate.BeginDateTime.Add(DTBeginStr);
                SendDate.EndDateTime.Add(DTEndStr);
            }
            else
            {
                for (int i = 0; i <= DateDiff(DTBeginStr, DTEndStr); i++)
                {
                    if (i == 0)
                    {
                        SendDate.BeginDateTime.Add(DTBeginStr);
                    }
                    else
                    {
                        SendDate.BeginDateTime.Add(Convert.ToDateTime(DTBeginStr.AddDays(i).ToString("yyyy-MM-dd 00:00:00")));
                    }

                    if (i != DateDiff(DTBeginStr, DTEndStr))
                    {
                        SendDate.EndDateTime.Add(Convert.ToDateTime(DTBeginStr.AddDays(i).ToString("yyyy-MM-dd 23:59:59")));

                    }
                    else
                    {
                        SendDate.EndDateTime.Add(DTEndStr);
                    }
                }
            }

        }
        //周报处理 将选择的时间截取为一周一周的形式,将自己选择的时间按自然周切分,如果不足一周,那么就按一周计算,注意,是自然周,当取得的时间没有被
        void WeekReportFunc()
        {
            SendDate = new AboutDateTime();
            DateTime DTBeginStr = ((DateTime)UC_DateTime_Begin.Value).ToUniversalTime();
            DateTime DTEndStr = ((DateTime)UC_DateTime_End.Value).ToUniversalTime();
            //标志来了
            SendDate.Sign = "W";
            int Week = S6101App.WeekStart; var weekdays = new int[7];
            int dt1 = (int)DTBeginStr.DayOfWeek;
            int dt2 = (int)DTEndStr.DayOfWeek;
            TimeSpan TS12 = DTEndStr - DTBeginStr;
            SendDate.BeginDateTime.Add(DTBeginStr);
            if (TS12.Days > 7)
            {
                int year = 0; int month = 0; int day = 0; int Cishu = 0;
                DateTime WeekBefore = DTBeginStr.AddDays(7); DateTime WeekAfter = DTEndStr.AddDays(-7);
                if (dt1 >= Week)
                    Cishu++;
                if (dt1 != Week)
                {
                    if (dt1 > Week)
                    {
                        WeekBefore = DTBeginStr.AddDays(7 - dt1 + Week);
                        year = WeekBefore.Year; month = WeekBefore.Month; day = WeekBefore.Day;
                        WeekBefore = Convert.ToDateTime(year + "/" + month + "/" + day + " 00:00:00");
                    }
                    else
                    {
                        WeekBefore = DTBeginStr.AddDays(7 - Week + dt1);
                        year = WeekBefore.Year; month = WeekBefore.Month; day = WeekBefore.Day;
                        WeekBefore = Convert.ToDateTime(year + "/" + month + "/" + day + " 00:00:00");
                    }
                }
                if (dt2 != Week - 1)
                {
                    if (dt2 > Week - 1)
                    {
                        WeekAfter = DTEndStr.AddDays(dt2 - Week + 1);
                        year = WeekAfter.Year; month = WeekAfter.Month; day = WeekAfter.Day;
                        WeekAfter = Convert.ToDateTime(year + "/" + month + "/" + day + " 23:59:59");
                    }
                    else
                    {
                        WeekAfter = DTEndStr.AddDays(8 - Week + dt2);
                        year = WeekAfter.Year; month = WeekAfter.Month; day = WeekAfter.Day;
                        WeekAfter = Convert.ToDateTime(year + "/" + month + "/" + day + " 23:59:59");
                    }
                }
                Cishu += (WeekAfter - WeekBefore).Days / 7;
                for (int i = 0; i <= Cishu; i++)
                {
                    SendDate.BeginDateTime.Add(Convert.ToDateTime((WeekBefore.AddDays(7 * i)).ToString("yyyy/MM/dd 00:00:00")));
                    switch (i)
                    {
                        case 0:
                            SendDate.EndDateTime.Add(Convert.ToDateTime((WeekBefore.AddDays(-1)).ToString("yyyy/MM/dd 23:59:59")));
                            break;
                        default:
                            SendDate.EndDateTime.Add(Convert.ToDateTime(WeekBefore.AddDays(7 * i - 1).ToString("yyyy/MM/dd 23:59:59")));
                            break;
                    }
                }
            }
            SendDate.EndDateTime.Add(DTEndStr);
        }
        //月报处理 将选择的时间截取为一月一月的形式
        void MonthReportFunc()
        {
            SendDate = new AboutDateTime();
            DateTime DTBeginStr = ((DateTime)UC_DateTime_Begin.Value).ToUniversalTime();
            DateTime DTEndStr = ((DateTime)UC_DateTime_End.Value).ToUniversalTime();
            //标志来了
            SendDate.Sign = "M";
            SendDate.BeginDateTime.Add(DTBeginStr);
            int Month = S6101App.MonthStart;
            DateTime YearBefore = DTBeginStr; DateTime YearAfter = DTEndStr;
            string YearBtemp = DTBeginStr.ToString("yyyy/MM/dd 00:00:00");
            string YearAtemp = DTEndStr.ToString("yyyy/MM/dd 23:59:59");
            int month = 0; int year = 0; int day = 0; int i = 1;
            if (DTBeginStr.Day < Month)
                i = 0;
            //不同年
            if (DTBeginStr.Year != DTEndStr.Year)
            {
                int Cishu = 12 - DTBeginStr.Month + DTEndStr.Month;
                for (; i <= Cishu; i++)
                {
                    month = DTBeginStr.Month + i; year = DTBeginStr.Year;
                    if (month > 12)
                    {
                        month = month - 12; year++;
                    }
                    day = Month;
                    SendDate.BeginDateTime.Add(Convert.ToDateTime(year + "/" + month + "/" + day + " 00:00:00"));
                    if (DTBeginStr.Day < day)
                        SendDate.EndDateTime.Add(Convert.ToDateTime(SendDate.BeginDateTime[i + 1].AddDays(-1).ToString("yyyy/MM/dd 23:59:59")));
                    else
                        SendDate.EndDateTime.Add(Convert.ToDateTime(SendDate.BeginDateTime[i].AddDays(-1).ToString("yyyy/MM/dd 23:59:59")));
                }
            }
            //同年
            else
            {//不同月
                if (DTBeginStr.Month != DTEndStr.Month)
                {
                    for (; i <= DTEndStr.Month - DTBeginStr.Month; i++)
                    {
                        month = DTBeginStr.Month + i; year = DTBeginStr.Year;
                        if (month > 12)
                        {
                            month = month - 12; year++;
                        }
                        day = Month;
                        SendDate.BeginDateTime.Add(Convert.ToDateTime(year + "/" + month + "/" + day + " 00:00:00"));
                        if (DTBeginStr.Day < day)
                            SendDate.EndDateTime.Add(Convert.ToDateTime(SendDate.BeginDateTime[i + 1].AddDays(-1).ToString("yyyy/MM/dd 23:59:59")));
                        else
                            SendDate.EndDateTime.Add(Convert.ToDateTime(SendDate.BeginDateTime[i].AddDays(-1).ToString("yyyy/MM/dd 23:59:59")));
                    }
                }
                else
                {
                    if (DTBeginStr.Day < Month && Month < DTEndStr.Day)
                    {
                        SendDate.BeginDateTime.Add(Convert.ToDateTime(YearBtemp.Substring(0, 8) + Month.ToString() + " 00:00:00"));
                        SendDate.EndDateTime.Add(Convert.ToDateTime(YearBtemp.Substring(0, 8) + Month.ToString() + " 23:59:59"));
                    }
                }
            }
            SendDate.EndDateTime.Add(DTEndStr);
        }
        //年报处理  将选择的时间截取为一年一年的形式
        void YearReportFunc()
        {
            SendDate = new AboutDateTime();
            DateTime DTBeginStr = ((DateTime)UC_DateTime_Begin.Value).ToUniversalTime();
            DateTime DTEndStr = ((DateTime)UC_DateTime_End.Value).ToUniversalTime();
            //标志来了
            SendDate.Sign = "Y";
            if (YearDiff(DTBeginStr, DTEndStr) == 0)
            {
                SendDate.BeginDateTime.Add(DTBeginStr);
                SendDate.EndDateTime.Add(DTEndStr);
            }
            else
            {
                for (int i = 0; i <= YearDiff(DTBeginStr, DTEndStr); i++)
                {
                    if (i == 0)
                    {
                        SendDate.BeginDateTime.Add(DTBeginStr);
                    }
                    else
                    {
                        SendDate.BeginDateTime.Add(Convert.ToDateTime(DTBeginStr.AddYears(i).ToString("yyyy-01-01 00:00:00")));
                    }

                    if (i != YearDiff(DTBeginStr, DTEndStr))
                    {
                        SendDate.EndDateTime.Add(Convert.ToDateTime(DTBeginStr.AddYears(i).ToString("yyyy-12-31 23:59:59")));
                    }
                    else
                    {
                        SendDate.EndDateTime.Add(DTEndStr);
                    }
                }
            }
        }
        //什么都不处理，生成一张整表的
        void TotalReportFuc()
        {
            SendDate = new AboutDateTime();
            SendDate.Sign = "T";
            SendDate.BeginDateTime.Add(((DateTime)UC_DateTime_Begin.Value).ToUniversalTime());
            SendDate.EndDateTime.Add(((DateTime)UC_DateTime_End.Value).ToUniversalTime());
        }

        //计算日期的差值   差几天的 
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            int dateDiff = 0;

            //为了防止出现这种  2012-12-12 15:00:00  到  2012-12-13 12:00:00 这种情况出现   我就将每天的时间换成一样的
            TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(DateTime1.ToString("yyyy-MM-dd 23:59:59")).Ticks);
            TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(DateTime2.ToString("yyyy-MM-dd 23:59:59")).Ticks);

            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days;
            //dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            return dateDiff;

            #region note
            //C#中使用TimeSpan计算两个时间的差值
            //可以反加两个日期之间任何一个时间单位。
            //TimeSpan ts = Date1 - Date2;
            //double dDays = ts.TotalDays;//带小数的天数，比如1天12小时结果就是1.5 
            //int nDays = ts.Days;//整数天数，1天12小时或者1天20小时结果都是1  
            #endregion
        }

        //计算两个日期差几年
        public static int YearDiff(DateTime DateTime1, DateTime DateTime2)
        {
            int yearDiff = DateTime2.Year - DateTime1.Year;
            return yearDiff;
        }

        private void InitCBTypeOfReport()
        {
            string LStrRangType = string.Empty;
            try
            {
                ComboBoxItem RangeTypeItem = CBTypeOfReport.SelectedItem as ComboBoxItem;
                LStrRangType = RangeTypeItem.Tag.ToString();
                if (LStrRangType == "Default")
                {
                    TotalReportFuc();
                }

                if (LStrRangType == "Day")
                {
                    DayReportFunc();
                }

                if (LStrRangType == "Week")
                {
                    WeekReportFunc();
                }

                if (LStrRangType == "Month")
                {
                    MonthReportFunc();
                }

                if (LStrRangType == "Year")
                {
                    YearReportFunc();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitQueryCondition()
        {
            mListQueryConditions.Clear();
            for (int num = 0; num < S6101App.mListQueryConditions.Count; num++)
            {
                if (S6101App.mListQueryConditions[num].ReportCode == 61010020)
                {
                    mListQueryConditions.Add(S6101App.mListQueryConditions[num]);
                }
            }
            this.ComboQueryConditions.SelectedIndex = -1;
            this.ComboQueryConditions.Text = "";
        }

        void ComboQueryConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //加载相应的内容
            var queryCondition = this.ComboQueryConditions.SelectedItem;
            if (queryCondition == null) { return; }
            string StrQueryCode = (queryCondition as QueryConditionInfo).QueryCode.ToString();
            List<QueryConditionItem> ListQueryItems = new List<QueryConditionItem>();
            ListQueryItems = S6101App.mListQueryItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode).ToList();
            for (int i = 0; i < ListQueryItems.Count; i++)
            {
                QueryConditionItem queryItem = ListQueryItems[i];
                switch (queryItem.QueryItemCode)
                {
                    case (long)Const6101.Query_RecordTime:
                        this.CBXLoggedTime.SelectedIndex = Convert.ToInt32(queryItem.Value3);
                        this.UC_DateTime_Begin.Text = queryItem.Value1;
                        this.UC_DateTime_End.Text = queryItem.Value2;
                        break;
                    case (long)Const6101.Query_KeyWords:

                        break;
                    case (long)Const6101.Query_ReportType:
                        this.CBTypeOfReport.SelectedIndex = Convert.ToInt32(queryItem.Value1);
                        break;
                }
            }
        }

        private void LoadRecordMachine(string StrQueryCode)
        {
            List<QueryConditionItem> ListQueryConditionItem = S6101App.mListQueryConditionItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode
                && p.QueryItemCode == Const6101.Query_RecordMachine).ToList();
            S6101App.TreeItemNoSelect(mRootItem1);
            for (int i = 0; i < ListQueryConditionItem.Count; i++)
            {
                string ItemCode = ListQueryConditionItem[i].Value1;
                CheckTreeItem(mRootItem1, ItemCode, 1);

                ItemCode = ListQueryConditionItem[i].Value2;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem1, ItemCode, 1);

                ItemCode = ListQueryConditionItem[i].Value3;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem1, ItemCode, 1);

                ItemCode = ListQueryConditionItem[i].Value4;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem1, ItemCode, 1);

                ItemCode = ListQueryConditionItem[i].Value5;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem1, ItemCode, 1);
            }
        }

        private void CheckTreeItem(ObjectItem RootItem, string CheckItemCode, int TypeCode)
        {
            switch (TypeCode)
            {
                case 1:
                    if (RootItem.FullName == CheckItemCode)
                    {
                        RootItem.IsChecked = true; return;
                    }
                    break;
                case 2:
                    string CompareStr = RootItem.FullName + "," + RootItem.Description;
                    if (CompareStr == CheckItemCode)
                    {
                        RootItem.IsChecked = true; return;
                    }
                    break;
            }
            for (int i = 0; i < RootItem.Children.Count; i++)
            {
                ObjectItem ChildObjItem = RootItem.Children[i] as ObjectItem;
                CheckTreeItem(ChildObjItem, CheckItemCode, TypeCode);
            }
        }

        private void SaveQueryCondition()
        {
            if (this.CbSaveConditions.IsChecked == true)
            {
                //循环找出是否有重名的，有的话覆盖资料
                string conditionName = this.ComboQueryConditions.Text;
                if (conditionName.Trim() == string.Empty)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0006", "Name cannot be Empty.")); return;
                }
                long queryCode = 0;
                List<string> ListSaveDatas = new List<string>();
                QueryCondition SaveCondition = new QueryCondition();
                bool Flag = false;
                for (int i = 0; i < mListQueryConditions.Count; i++)
                {
                    if (mListQueryConditions[i].Name == conditionName)
                    {
                        SaveCondition.IsUse = mListQueryConditions[i].IsUse;
                        SaveCondition.mName = mListQueryConditions[i].Name;
                        SaveCondition.mDescription = mListQueryConditions[i].Description;
                        SaveCondition.Priority = mListQueryConditions[i].Priority;
                        SaveCondition.QueryCode = mListQueryConditions[i].QueryCode;
                        SaveCondition.UserID = mListQueryConditions[i].UserID;
                        SaveCondition.ReportCode = mListQueryConditions[i].ReportCode;
                        SaveCondition.SetTime = mListQueryConditions[i].SetTime;
                        SaveCondition.LastUseTime = mListQueryConditions[i].LastUseTime;
                        SaveCondition.Source = mListQueryConditions[i].Source;
                        Flag = true;
                        break;
                    }
                }
                if (!Flag)//没找到，要新建
                {
                    SaveCondition.QueryCode = 0;
                    SaveCondition.UserID = CurrentApp.Session.UserID;
                    SaveCondition.ReportCode = 61010022;
                    SaveCondition.SetTime = DateTime.Now;
                    SaveCondition.Source = 'S';
                    SaveCondition.LastUseTime = DateTime.Now;
                    SaveCondition.mName = this.ComboQueryConditions.Text;
                    SaveCondition.mDescription = this.ComboQueryConditions.Text;
                    SaveCondition.Priority = mListQueryConditions.Count;
                    SaveCondition.IsUse = true;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject<QueryCondition>(SaveCondition);
                if (!optReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("", "Save Fail!")); return;
                }
                ListSaveDatas.Add(optReturn.Data as string);
                queryCode = SaveCondition.QueryCode;
                //添加detail。
                for (int i = 1; i <= 3; i++)
                {
                    QueryConditionItem queryItem = new QueryConditionItem();
                    queryItem.QueryConditionCode = queryCode;
                    queryItem.Sort = i;
                    queryItem.Type = 0;
                    switch (i)
                    {
                        case 1://录音时间
                            queryItem.QueryItemCode = (long)Const6101.Query_RecordTime;

                            queryItem.Value1 = this.UC_DateTime_Begin.Text;
                            queryItem.Value2 = this.UC_DateTime_End.Text;
                            queryItem.Value3 = this.CBXLoggedTime.SelectedIndex.ToString();
                            break;
                        case 2://关键词
                            queryItem.QueryItemCode = (long)Const6101.Query_KeyWords;

                            break;
                        case 3://报表类型
                            queryItem.QueryItemCode = (long)Const6101.Query_ReportType;
                            queryItem.Value1 = this.CBTypeOfReport.SelectedIndex.ToString();
                            break;
                    }
                    OperationReturn optReturnItem = XMLHelper.SeriallizeObject<QueryConditionItem>(queryItem);
                    if (!optReturnItem.Result)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("", "Save Fail!")); return;
                    }
                    ListSaveDatas.Add(optReturnItem.Data as string);
                }
                //调用函数方法保存
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)WebCodes.SaveQueryCondition;
                webRequest.ListData = ListSaveDatas;
                webRequest.Session = CurrentApp.Session;
                Service61012Client client = new Service61012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service61012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPReportOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                S6101App.InitObjects();
                InitQueryCondition();
            }
        }

        private List<string> SaveQueryConditionItems(List<string> mListData, long QueryCode, long QueryItemCode)
        {
            List<string> mListQueryCondition = new List<string>();
            int num = mListData.Count;
            for (int i = 0; i < num; i += 5)
            {
                QueryConditionItem queryItem = new QueryConditionItem();
                queryItem.QueryConditionCode = QueryCode;
                queryItem.QueryItemCode = QueryItemCode;
                queryItem.Type = 1;
                queryItem.Sort = i;
                queryItem.Value1 = mListData[i];
                if (i + 1 < num)
                    queryItem.Value2 = mListData[i + 1];
                if (i + 2 < num)
                    queryItem.Value3 = mListData[i + 2];
                if (i + 3 < num)
                    queryItem.Value4 = mListData[i + 3];
                if (i + 4 < num)
                    queryItem.Value5 = mListData[i + 4];

                OperationReturn optReturn = XMLHelper.SeriallizeObject<QueryConditionItem>(queryItem);
                if (!optReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("", "Save Fail!")); return new List<string>();
                }
                mListQueryCondition.Add(optReturn.Data as string);
            }
            return mListQueryCondition;
        }
    }
}
