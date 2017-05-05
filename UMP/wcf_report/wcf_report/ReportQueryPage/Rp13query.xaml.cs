using Common61011;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS6101;
using UMPS1101.Models;
using UMPS6101.Models;
using UMPS6101.SharingClasses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using UMPS6101.Wcf61012;
using System.Collections.ObjectModel;
using VoiceCyber.Common;

namespace UMPS6101.ReportQueryPage
{
    /// <summary>
    /// Rp13query.xaml 的交互逻辑
    /// </summary>
    public partial class Rp13query
    {
        #region Memembers
        public ReportDisplayPage PageParent;
        public BandingDemo BandingDemo_EX;
      
        public AboutDateTime SendDate;
      
        public ObjectItem mRootItem;
        public ObjectItem mRootItem1;
        public ObjectItem itemRootPFB;

        private List<ObjectItem> mListSelectedObjects;//勾选项的内容
        private List<ObjectItem> mListSelectedObjects1;

        private BackgroundWorker mWorker;
        private ObservableCollection<QueryConditionInfo> mListQueryConditions;
        //时间控件相关的
        private string IStrBeginTime = string.Empty;
        private string IStrEndTime = string.Empty;
        #endregion
        public S6101App S6101App;
        public Rp13query()
        {
            InitializeComponent();
            InitLoggedBeginEndTime();

            mRootItem = new ObjectItem();
            mRootItem1 = new ObjectItem();
            itemRootPFB = new ObjectItem();
           
            mListQueryConditions = new ObservableCollection<QueryConditionInfo>();

            mListSelectedObjects = new List<ObjectItem>();
            mListSelectedObjects1 = new List<ObjectItem>();

            Loaded += Rp13InspectorList_Loaded;
            Loaded += Rp13PFBList_Loaded;
            Loaded += Rp13query_Loaded;
            ComboQueryConditions.ItemsSource = mListQueryConditions;
            this.CbSaveConditions.Checked += CbSaveConditions_Checked;
            this.ComboQueryConditions.SelectionChanged += ComboQueryConditions_SelectionChanged;
           
        }

        void Rp13query_Loaded(object sender, RoutedEventArgs e)
        {
            InitQueryCondition();
            ChangeLanguage();
            itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
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
            Title.Content = CurrentApp.GetLanguageInfo("610100001301", "质检员工作量报表条件(无任务)");
            TimeL.Content = CurrentApp.GetLanguageInfo("610100001302", "时间");
            TimeOfScore.Content = CurrentApp.GetLanguageInfo("610100001303", "评分时间");
            TimeOfRecord.Content = CurrentApp.GetLanguageInfo("610100001304", "录音时间");
            TabPFB.Header = CurrentApp.GetLanguageInfo("610100001305", "评分表");
            TabInspector.Header = CurrentApp.GetLanguageInfo("610100001306", "评分人");
            this.CbSaveConditions.Content = CurrentApp.GetLanguageInfo("61010103", "Save Conditions");
        }

        //加载质检员
        void Rp13InspectorList_Loaded(object sender, RoutedEventArgs e)
        {
            CTInspector.ItemsSource = mRootItem.Children;
            mRootItem.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitInspectorParents();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem.IsChecked = false;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();
        }
        private void GetCheckedInspector()
        {
            try
            {
                mListSelectedObjects.Clear();
                GetCheckedInspector(mRootItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void GetCheckedInspector(ObjectItem parentItem)
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
                            GetCheckedInspector(item);
                        }
                        else
                        {
                            if (item.IsChecked == true)
                            {
                                mListSelectedObjects.Add(item);
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
      
        //加载评分表项的
        void Rp13PFBList_Loaded(object sender, RoutedEventArgs e)
        {
            CTPFB.ItemsSource = mRootItem1.Children;
            mRootItem1.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitPFBList();
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
        private void GetCheckedPFB()
        {
            try
            {
                mListSelectedObjects1.Clear();
                GetCheckedPFB(itemRootPFB);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void GetCheckedPFB(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_ORG)//尽管这句话这里有些问题，因为这里的判断恒为false，因此不会执行这个里面的。但是，对整个进程也不会影响，我先做个标记
                        {
                            if (item.IsChecked == false) { continue; }
                            GetCheckedPFB(item);
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

        #region Init and load
        private void InitInspectorParents()
        {
            InitControlOrgsOfInspector(mRootItem, "-1");
        }

        private void InitControlOrgsOfInspector(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.GetControlOrgInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service61012Client client = new Service61012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service61012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPReportOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<ObjectItem> TempObjItems = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    item.DisplayContent = string.Format("{0}", strName);
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    TempObjItems.Add(item);
                }
                TempObjItems.OrderBy(p=>p.Name);
                foreach(ObjectItem item in TempObjItems)
                {
                    InitControlOrgsOfInspector(item, item.ObjID.ToString());
                    InitInspector(item, item.ObjID.ToString());
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInspector(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.GetInspectorList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service61012Client client = new Service61012Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service61012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPReportOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.FullName = strFullName;
                    item.Data = strInfo;
                    item.DisplayContent = string.Format("{0}({1})", strFullName, strName);
                    item.Icon = "Images/inspector.ico";
                    
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPFBList()
        {
            AddChildObject(mRootItem1, itemRootPFB);
            InitPFBList(itemRootPFB, "-1");
        }

        private void InitPFBList(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.GetPFBList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("-1");
                Service61012Client client = new Service61012Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service61012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPReportOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 1) { continue; }
                    string strPFBID = arrInfo[0];
                    string strPFBName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_OPERATIONLOG_UMP;
                    item.Name = strPFBID;
                    item.FullName = strPFBName;
                    item.Data = strInfo;
                    item.DisplayContent = string.Format("{0}", strPFBName);
                    item.Icon = "Images/pfb.ico";
                  
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        private void CBXLoggedTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitLoggedBeginEndTime();
        }

        private void InitLoggedBeginEndTime()
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
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));//这句话得理解下 
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
        void CbSaveConditions_Checked(object sender, RoutedEventArgs e)
        {
            this.ComboQueryConditions.SelectedIndex = -1;
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
            int sec = (DTEndStr - DTBeginStr).Seconds;
            if (sec < 0)
            {
                // CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0010", "The time setting error"));
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
            InitCBTypeOfReport(); GetCheckedInspector(); GetCheckedPFB();
            List<string> QueryData = new List<string>(); string time = BoolTime();
            QueryData.Add(time + Convert.ToDateTime(UC_DateTime_Begin.Value).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            QueryData.Add(time + Convert.ToDateTime(UC_DateTime_End.Value).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            string GP = "GP"; string GN = "GN";
            if (mListSelectedObjects1.Count != 0)
            {
                for (int i = 0; i < mListSelectedObjects1.Count; i++)
                {
                    GN += mListSelectedObjects1[i].Name + ";";
                }
                QueryData.Add(GN);
            }
            if (mListSelectedObjects.Count != 0)
            {
                for (int i = 0; i < mListSelectedObjects.Count; i++)
                {
                    GP += mListSelectedObjects[i].ObjID + ";";
                }
                QueryData.Add(GP);
            }
            //if (mListSelectedObjects.Count == 0)
            //{
            //    GetCheckedInspector_All();
            //    for (int i = 0; i < mListSelectedObjects.Count; i++)
            //    {
            //        GP += mListSelectedObjects[i].ObjID + ";";
            //    }
            //    QueryData.Add(GP);
            //}
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

        private void CheckScore(object sender, RoutedEventArgs e)
        {
            if (this.TimeOfScore.IsChecked == true)
            {
                this.TimeOfRecord.IsChecked = false;
            }
            else
            {
                this.TimeOfRecord.IsChecked = true;
            }
        }

        private void CheckRecord(object sender, RoutedEventArgs e)
        {
            if (this.TimeOfRecord.IsChecked == true)
            {
                this.TimeOfScore.IsChecked = false;
            }
            else
            {
                this.TimeOfScore.IsChecked = true;
            }
        }

        //GT代表评分时间  RT代表录音时间
        private string BoolTime()
        {
            string TimeTypeCheck = null;
            if (this.TimeOfScore.IsChecked == true)
            {
                TimeTypeCheck = "GT";
            }
            else
            {
                TimeTypeCheck = "RT";
            }
            return TimeTypeCheck;
        }

        private void InitQueryCondition()
        {
            mListQueryConditions.Clear();
            for (int num = 0; num < S6101App.mListQueryConditions.Count; num++)
            {
                if (S6101App.mListQueryConditions[num].ReportCode == 61010013)
                {
                    mListQueryConditions.Add(S6101App.mListQueryConditions[num]);
                }
            }
            this.ComboQueryConditions.SelectedIndex = -1;
            this.ComboQueryConditions.Text = "";
        }
        private void ComboQueryConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    case (long)Const6101.Query_Time:
                        
                        this.CBXLoggedTime.SelectedIndex = Convert.ToInt32(queryItem.Value3);
                        this.UC_DateTime_Begin.Text = queryItem.Value1;
                        this.UC_DateTime_End.Text = queryItem.Value2;
                        if (queryItem.Value4 == "1")
                        {
                            this.TimeOfRecord.IsChecked = true;
                        }
                        else
                        {
                            this.TimeOfScore.IsChecked = true;
                        }
                        break;
                    case (long)Const6101.Query_ScoreTable:
                        LoadScoreTable(StrQueryCode);
                        break;
                    case (long)Const6101.Query_ReportType:
                        this.CBTypeOfReport.SelectedIndex = Convert.ToInt32(queryItem.Value1);
                        break;
                    case (long)Const6101.Query_Grader:
                        LoadGrader(StrQueryCode);
                        break;

                }
            }
        }
        private void CheckTreeItem(ObjectItem RootItem, string CheckItemCode, int TypeCode)
        {
            switch (TypeCode)
            {
                case 1:
                    if (RootItem.ObjID.ToString() == CheckItemCode)
                    {
                        RootItem.IsChecked = true; return;
                    }
                    break;
                case 2:
                    if (RootItem.Name == CheckItemCode)
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
        private void LoadGrader(string StrQueryCode)
        {
            List<QueryConditionItem> ListQueryConditionItem = S6101App.mListQueryConditionItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode
               && p.QueryItemCode == Const6101.Query_Grader).ToList();
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

        private void LoadScoreTable(string StrQueryCode)
        {
            List<QueryConditionItem> ListQueryConditionItem = S6101App.mListQueryConditionItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode
              && p.QueryItemCode == Const6101.Query_ScoreTable).ToList();
            S6101App.TreeItemNoSelect(mRootItem);
            for (int i = 0; i < ListQueryConditionItem.Count; i++)
            {
                string ItemCode = ListQueryConditionItem[i].Value1;
                CheckTreeItem(mRootItem, ItemCode, 2);

                ItemCode = ListQueryConditionItem[i].Value2;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem, ItemCode, 2);

                ItemCode = ListQueryConditionItem[i].Value3;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem, ItemCode, 2);

                ItemCode = ListQueryConditionItem[i].Value4;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem, ItemCode, 2);

                ItemCode = ListQueryConditionItem[i].Value5;
                if (ItemCode != string.Empty && ItemCode.Length > 4)
                    CheckTreeItem(mRootItem, ItemCode, 2);
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
                    SaveCondition.ReportCode = 61010013;
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
                for (int i = 1; i <= 4; i++)
                {
                    QueryConditionItem queryItem = new QueryConditionItem();
                    queryItem.QueryConditionCode = queryCode;
                    queryItem.Sort = i;
                    queryItem.Type = 0;
                    switch (i)
                    {
                        case 1://时间
                            queryItem.QueryItemCode = (long)Const6101.Query_Time;
                            queryItem.Value1 = this.UC_DateTime_Begin.Text;
                            queryItem.Value2 = this.UC_DateTime_End.Text;
                            queryItem.Value3 = this.CBXLoggedTime.SelectedIndex.ToString();
                            if (this.TimeOfRecord.IsChecked == true)
                            {
                                queryItem.Value4 = "1";
                            }
                            else
                            {
                                queryItem.Value4 = "2";
                            }
                            break;
                        case 2://评分表
                            queryItem.QueryItemCode = (long)Const6101.Query_ScoreTable;
                            List<string> TempDataST = new List<string>();
                            for (int j = 0; j < mListSelectedObjects1.Count; j++)
                            {
                                TempDataST.Add(mListSelectedObjects1[j].Name);
                            }
                            TempDataST = SaveQueryConditionItems(TempDataST, queryCode, queryItem.QueryItemCode);
                            foreach (string Str in TempDataST)
                            {
                                ListSaveDatas.Add(Str);
                            }
                            break;
                        case 3://报表类型
                            queryItem.QueryItemCode = (long)Const6101.Query_ReportType;
                            queryItem.Value1 = this.CBTypeOfReport.SelectedIndex.ToString();
                            break;
                        case 4://评分人
                            queryItem.QueryItemCode = (long)Const6101.Query_Grader;
                            List<string> TempData = new List<string>();
                            for (int j = 0; j < mListSelectedObjects.Count; j++)
                            {
                                TempData.Add(mListSelectedObjects[j].ObjID.ToString());
                            }
                            TempData = SaveQueryConditionItems(TempData, queryCode, queryItem.QueryItemCode);
                            foreach (string Str in TempData)
                            {
                                ListSaveDatas.Add(Str);
                            }
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
