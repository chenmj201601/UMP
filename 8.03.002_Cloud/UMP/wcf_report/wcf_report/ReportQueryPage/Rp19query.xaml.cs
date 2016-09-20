using Common61011;
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
using UMPS1101.Models;
using UMPS6101.Models;
using UMPS6101.SharingClasses;
using UMPS6101.Wcf31021;
using UMPS6101.Wcf61012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS6101.ReportQueryPage
{
    /// <summary>
    /// Rp19query.xaml 的交互逻辑
    /// </summary>
    public partial class Rp19query
    {
        #region Memembers
        public ReportDisplayPage PageParent;

        public AboutDateTime SendDate;

        public ObjectItem mRootItem = new ObjectItem();
        public ObjectItem mRootItem1 = new ObjectItem();
        public ScoreSheet mRootItem2 = new ScoreSheet();
        public ObjectItem itemRootPFB = new ObjectItem();

        private List<ObjectItem> mListSelectedObjects;//勾选项的内容
        private List<ObjectItem> mListSelectedObjects1;
        private List<ScoreGroup> mListSelectedObjects2;
        private string ScoreSheetID;

        private BackgroundWorker mWorker;
        private ObservableCollection<QueryConditionInfo> mListQueryConditions;
        List<ScoreGroup> ListScoreGroup;
        private ObjectItem SelectObjItem;
        private List<ScoreGroup> TempPFBItems;
        #endregion
        //时间控件相关的
        private string IStrBeginTime = string.Empty;
        private string IStrEndTime = string.Empty;
        public S6101App S6101App;
        public Rp19query()
        {
            InitializeComponent();
            InitLoggedBeginEndTime();

            Loaded += Rp19query_Loaded;
            ScoreSheetID = string.Empty;
            mListQueryConditions = new ObservableCollection<QueryConditionInfo>();
           
            mListSelectedObjects = new List<ObjectItem>();
            mListSelectedObjects1 = new List<ObjectItem>();
            mListSelectedObjects2 = new List<ScoreGroup>();
            TempPFBItems = new List<ScoreGroup>();

            Loaded += Rp19PFBList_Loaded;
            Loaded += Rp16Agents_Loaded;
            this.CbSaveConditions.Checked += CbSaveConditions_Checked;
            this.CTPFB.SelectedItemChanged += CTPFB_SelectedItemChanged;
            ComboQueryConditions.ItemsSource = mListQueryConditions;

            this.ComboQueryConditions.SelectionChanged += ComboQueryConditions_SelectionChanged;
            
        }

        void Rp19query_Loaded(object sender, RoutedEventArgs e)
        {
            InitQueryCondition();
            ChangeLanguage();
            itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
        }

        void CTPFB_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ScoreSheetSelectChange();
        }
        private void ScoreSheetSelectChange()
        {
            try
            {
                ObjectItem ScoreSheetSelect = this.CTPFB.SelectedItem as ObjectItem;
                if (ScoreSheetSelect == null)
                {
                    if (SelectObjItem == null)
                        return;
                    else
                    {
                        ScoreSheetSelect = SelectObjItem;
                    }
                }
                else
                {
                    itemRootPFB = ScoreSheetSelect;
                }
                ScoreSheetID = ScoreSheetSelect.FullName;
                if (ScoreSheetSelect != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = 15;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(ScoreSheetSelect.FullName);
                    webRequest.ListData.Add("0");
                    Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("R19 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("R19XmlSS Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    mRootItem2 = optReturn.Data as ScoreSheet;
                    if (mRootItem2 == null)
                    {
                        ShowException(string.Format("Fail.\tScoreSheet is null"));
                        return;
                    }
                    mRootItem2.Init();
                    List<ScoreObject> ListScoreObject = new List<ScoreObject>();
                    ListScoreGroup = new List<ScoreGroup>();
                    mRootItem2.GetAllScoreObject(ref ListScoreObject);
                    foreach (ScoreObject Sobject in ListScoreObject)
                    {
                        ScoreGroup TempScoreGroup = Sobject as ScoreGroup;
                        if (TempScoreGroup != null)
                        {
                            ListScoreGroup.Add(TempScoreGroup);
                        }
                    }
                    this.CTStandards.ItemsSource = ListScoreGroup;// mRootItem2.Items;
                }
            }
            catch (Exception ex)
            {
            }
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
            //TypeOfReport.Content = CurrentApp.GetLanguageInfo("610100000010", "报表类型");
            //Default.Content = CurrentApp.GetLanguageInfo("610100000011", "默认");
            //DayReport.Content = CurrentApp.GetLanguageInfo("610100000012", "日报");
            //WeekReport.Content = CurrentApp.GetLanguageInfo("610100000013", "周报");
            //MonthReport.Content = CurrentApp.GetLanguageInfo("610100000014", "月报");
            //YearReport.Content = CurrentApp.GetLanguageInfo("610100000015", "年报");
            //自身部分
            Title.Content = CurrentApp.GetLanguageInfo("610100001901", "坐席评分表类别统计报表条件");
            TimeL.Content = CurrentApp.GetLanguageInfo("610100000402", "时间");
            TimeOfScore.Content = CurrentApp.GetLanguageInfo("610100000403", "评分时间");
            TimeOfRecord.Content = CurrentApp.GetLanguageInfo("610100000404", "录音时间");
            TabAgent.Header = CurrentApp.GetLanguageInfo("610100000405", "坐席");
            TabPFB.Header = CurrentApp.GetLanguageInfo("610100000406", "评分表");
            TabScoreStandards.Header = CurrentApp.GetLanguageInfo("610100001902", "评分表项");
            this.CbSaveConditions.Content = CurrentApp.GetLanguageInfo("61010103", "Save Conditions");
            this.LabTopNum.Content = CurrentApp.GetLanguageInfo("610100001603", "");
        }

        //加载评分表项的
        void Rp19PFBList_Loaded(object sender, RoutedEventArgs e)
        {
            CTPFB.ItemsSource = mRootItem.Children;
            mRootItem.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitPFBList();
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
        private void GetCheckedPFB()
        {
            try
            {
                mListSelectedObjects.Clear();
                GetCheckedPFB(mRootItem);
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
                        //if (item.ObjType == ConstValue.RESOURCE_ORG)//尽管这句话这里有些问题，因为这里的判断恒为false，因此不会执行这个里面的。但是，对整个进程也不会影响，我先做个标记
                        //{
                        //    if (item.IsChecked == false) { continue; }
                        //    GetCheckedPFB(item);
                        //}
                        //else
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

        //加载坐席列表的
        void Rp16Agents_Loaded(object sender, RoutedEventArgs e)
        {
            CTAgent.ItemsSource = mRootItem1.Children;
            mRootItem1.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitControlObjects();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem1.IsChecked = false;
                if (mRootItem1.Children.Count > 0)
                {
                    mRootItem1.Children[0].IsExpanded = true;
                }
                //ChangeLanguage();
            };
            mWorker.RunWorkerAsync();
        }
        private void GetCheckedAgents()
        {
            try
            {
                mListSelectedObjects1.Clear();
                GetCheckedAgents(mRootItem1);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void GetCheckedAgents(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_ORG)
                        {
                            if (item.IsChecked == false) { continue; }
                            GetCheckedAgents(item);
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

        //获取standards
        private void GetStandards()
        {
            try
            {
                for (int i = 0; i < CTStandards.Items.Count; i++)
                {
                    ScoreGroup item = CTStandards.Items[i] as ScoreGroup;
                    if (item != null)
                    {
                        //if (item.Type == ScoreObjectType.ScoreGroup)
                        {
                            if (item.IsAllowNA == true)
                            {
                                mListSelectedObjects2.Add(item as ScoreGroup);
                                TempPFBItems.Add(item as ScoreGroup);
                            }
                        }
                    }
                }
                if (mListSelectedObjects2.Count != 0)
                {
                    S6101App.ListScoreSheet = mListSelectedObjects2;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #region Init and load

        private void InitPFBList()
        {
            //AddChildObject(mRootItem, itemRootPFB);
            //InitPFBList(itemRootPFB, "-1");
            InitPFBList(mRootItem, "-1");
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
                    item.Name = strPFBName;
                    item.FullName = strPFBID;
                    item.Data = strInfo;
                    item.DisplayContent = string.Format("{0}", strPFBName);
                    item.Icon = "Images/pfb.ico";
                    //item.IsSingleSelected = true;

                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlObjects()
        {
            InitControlOrgs(mRootItem1, "-1");
        }

        private void InitControlOrgs(ObjectItem parentItem, string parentID)
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
                    InitControlOrgs(item, item.ObjID.ToString());
                    InitControlAgents(item, item.ObjID.ToString());
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.GetControlAgentInfoList;
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
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;//坐席工号
                    item.FullName = strFullName;//坐席全名
                    item.Description = strName;
                    item.Data = strInfo;
                    item.DisplayContent = string.Format("{0}({1})", strFullName, strName);
                    item.Icon = "Images/agent.ico";

                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
        void CbSaveConditions_Checked(object sender, RoutedEventArgs e)
        {
            this.ComboQueryConditions.SelectedIndex = -1;
        }
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
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }
        #endregion

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
                MessageBox.Show("The time setting error");
            }
            else
            {
                List<string> QueryData;
                if (CollectData(out QueryData))
                {
                    PageParent.ComfireClick(QueryData, SendDate);
                    SaveQueryCondition();
                }
            }
        }
        public bool CollectData(out List<string> QueryData)
        {
            GetCheckedAgents(); TotalReportFuc(); GetStandards(); GetCheckedPFB();
            QueryData = new List<string>(); string TimeType = BoolTime();
            QueryData.Add(TimeType + Convert.ToDateTime(UC_DateTime_Begin.Value).ToUniversalTime().ToString("yyyyMMddHHmmss"));
            QueryData.Add(TimeType + Convert.ToDateTime(UC_DateTime_End.Value).ToUniversalTime().ToString("yyyyMMddHHmmss"));
            string AG = "AG"; string GN = "GN"; string GS = "GS";
            try
            {
                if (ScoreSheetID != string.Empty)
                {
                    GN += ScoreSheetID + ";";
                    QueryData.Add(GN);
                }
                if (GN == "GN" && mListSelectedObjects.Count != 0)
                {
                    for (int i = 0; i < mListSelectedObjects.Count; i++)
                    {
                        GN += mListSelectedObjects[i].FullName + ";";
                    }
                    QueryData.Add(GN);
                }
                if (mListSelectedObjects1.Count != 0)
                {
                    for (int i = 0; i < mListSelectedObjects1.Count; i++)
                    {
                        AG += mListSelectedObjects1[i].ObjID + ";";
                    }
                    QueryData.Add(AG);
                }
                if (mListSelectedObjects2.Count != 0)
                {
                    for (int i = 0; i < mListSelectedObjects2.Count; i++)
                    {
                        GS += mListSelectedObjects2[i].ID + ";";
                    }
                    QueryData.Add(GS);
                }
                int numTop;
                if (int.TryParse(this.TxtTopNum.Text, out numTop))
                {
                    if (numTop > 0)
                        S6101App.TopNum = numTop;
                }
                else
                {
                    ShowException(CurrentApp.GetLanguageInfo("6101N0005", "Please Input Number in Number Point Count or Top Number"));
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show("Input Data Error"); return false;
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

        private string BoolTime()
        {
            string type_time = string.Empty;
            if (this.TimeOfScore.IsChecked == true)
            {
                type_time = "GT";
            }
            else
            {
                type_time = "RT";
            }
            return type_time;
        }

        private void InitQueryCondition()
        {
            mListQueryConditions.Clear();
            for (int num = 0; num < S6101App.mListQueryConditions.Count; num++)
            {
                if (S6101App.mListQueryConditions[num].ReportCode == 61010019)
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
            //List<QueryConditionItem> PFBquery = ListQueryItems.Where(p => p.QueryItemCode == (long)Const6101.Query_ScoreTable).ToList();
            LoadScoreTable(StrQueryCode);
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
                    //case (long)Const6101.Query_ScoreTable:
                    //    LoadScoreTable(StrQueryCode);
                    //    break;
                    case (long)Const6101.Query_Agent:
                        LoadAgent(StrQueryCode);
                        break;
                    case (long)Const6101.Query_ScoreTableItem:
                        LoadScoreTableItem(StrQueryCode);
                        break;
                    case (long)Const6101.Query_TopNumber:
                        this.TxtTopNum.Text = queryItem.Value1;
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
                    if (RootItem.FullName == CheckItemCode)
                    {
                        RootItem.IsChecked = true;
                        RootItem.IsSelected = true;
                        RootItem.IsExpanded = true;
                        SelectObjItem = RootItem;
                        return;
                    }
                    break;
                case 3:
                    for (int i = 0; i < ListScoreGroup.Count; i++)
                    {
                        if (ListScoreGroup[i].ID.ToString() == CheckItemCode)
                        {
                            //px
                            ListScoreGroup[i].IsAllowNA = true; return;
                        }
                    }
                    return;
                //break;
            }
            for (int i = 0; i < RootItem.Children.Count; i++)
            {
                ObjectItem ChildObjItem = RootItem.Children[i] as ObjectItem;
                CheckTreeItem(ChildObjItem, CheckItemCode, TypeCode);
            }
        }
        private void LoadAgent(string StrQueryCode)
        {
            List<QueryConditionItem> ListQueryConditionItem = S6101App.mListQueryConditionItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode
                && p.QueryItemCode == Const6101.Query_Agent).ToList();
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

        private void LoadScoreTableItem(string StrQueryCode)
        {
            List<QueryConditionItem> ListQueryConditionItem = S6101App.mListQueryConditionItems.Where(p => p.QueryConditionCode.ToString() == StrQueryCode
              && p.QueryItemCode == Const6101.Query_ScoreTableItem).ToList();
            foreach (ScoreGroup sg in ListScoreGroup)
            {
                sg.IsAllowNA = false;
            }
            for (int i = 0; i < ListQueryConditionItem.Count; i++)
            {
                string ItemCode = ListQueryConditionItem[i].Value1;
                CheckTreeItem(mRootItem, ItemCode, 3);

                ItemCode = ListQueryConditionItem[i].Value2;
                if (ItemCode != string.Empty && ItemCode.Length > 0)
                    CheckTreeItem(mRootItem, ItemCode, 3);

                ItemCode = ListQueryConditionItem[i].Value3;
                if (ItemCode != string.Empty && ItemCode.Length > 0)
                    CheckTreeItem(mRootItem, ItemCode, 3);

                ItemCode = ListQueryConditionItem[i].Value4;
                if (ItemCode != string.Empty && ItemCode.Length > 0)
                    CheckTreeItem(mRootItem, ItemCode, 3);

                ItemCode = ListQueryConditionItem[i].Value5;
                if (ItemCode != string.Empty && ItemCode.Length > 0)
                    CheckTreeItem(mRootItem, ItemCode, 3);
            }
        }//px

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
            ScoreSheetSelectChange();
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
                    SaveCondition.ReportCode = 61010019;
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
                for (int i = 1; i <= 5; i++)
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
                            TempDataST.Add(itemRootPFB.FullName);
                            //for (int j = 0; j < mListSelectedObjects.Count; j++)
                            //{
                            //    TempDataST.Add(mListSelectedObjects[j].FullName);
                            //}
                            TempDataST = SaveQueryConditionItems(TempDataST, queryCode, queryItem.QueryItemCode);
                            foreach (string Str in TempDataST)
                            {
                                ListSaveDatas.Add(Str);
                            }
                            break;
                        case 3://统计前几名
                            queryItem.QueryItemCode = (long)Const6101.Query_TopNumber;
                            queryItem.Value1 = this.TxtTopNum.Text;
                            break;
                        case 5://评分表项
                            queryItem.QueryItemCode = (long)Const6101.Query_ScoreTableItem;
                            List<string> TempData = new List<string>();
                            for (int j = 0; j < TempPFBItems.Count; j++)
                            {
                                TempData.Add(TempPFBItems[j].ID.ToString());
                            }
                            TempData = SaveQueryConditionItems(TempData, queryCode, queryItem.QueryItemCode);
                            foreach (string Str in TempData)
                            {
                                ListSaveDatas.Add(Str);
                            }
                            break;
                        case 4://坐席
                            queryItem.QueryItemCode = (long)Const6101.Query_Agent;
                            List<string> TempDataExt = new List<string>();
                            for (int j = 0; j < mListSelectedObjects1.Count; j++)
                            {
                                TempDataExt.Add(mListSelectedObjects1[j].ObjID.ToString());
                            }
                            TempDataExt = SaveQueryConditionItems(TempDataExt, queryCode, queryItem.QueryItemCode);
                            foreach (string StrE in TempDataExt)
                            {
                                ListSaveDatas.Add(StrE);
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
