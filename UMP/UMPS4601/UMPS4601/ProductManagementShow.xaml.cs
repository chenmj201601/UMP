using Common4602;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
using UMPS4601.Commands;
using UMPS4601.Converters;
using UMPS4601.Models;
using UMPS4601.Wcf11012;
using UMPS4601.Wcf46012;
using Visifire.Charts;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS4601
{
    /// <summary>
    /// ProductManagementShow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductManagementShow
    {
        public PMMainView ParentPage;

        #region 右侧机构、用户、座席、分机树

        string treeType = "O";
        /// <summary>
        /// 保存查询到的所有的座席、分机信息，以待匹配
        /// </summary>
        public List<UERAListInfo> ListAllAERInfo; 
                
        //递归的方式获取树节点
        /// <summary>
        /// 机构树
        /// </summary>
        public ObjectItem mRootTreeItem;
        /// <summary>
        /// 技能组树
        /// </summary>
        public ObjectItem mSkillTreeItem;
        /// <summary>
        /// 机构对象树
        /// </summary>
        public ObjectItem mOrgObjectItem;
        /// <summary>
        /// 技能组对象树
        /// </summary>
        public ObjectItem mSkillObjectItem;
        
        /// <summary>
        /// 被选中的机构
        /// </summary>
        string checkedOrg = string.Empty;
        /// <summary>
        /// 被选中的用户、座席等
        /// </summary>
        string checkOObject = string.Empty;
        /// <summary>
        /// 被选中的技能组
        /// </summary>
        string checkedSkiller = string.Empty;
        /// <summary>
        /// 被选中的技能组对象
        /// </summary>
        string checkedSObject = string.Empty;

        string checkedObject = string.Empty;

        List<UERAListInfo> checkedUERA = new List<UERAListInfo>();

        #endregion

        private BackgroundWorker mWorker;

        public List<PMItems> mListPMItems;

        //保存被删除、禁用的KPI-mapping
        public List<PMItems> mListDeletePmItems;

        //列表显示用
        public ObservableCollection<PMShowDataItems> mCurentPMDatas;
        /// <summary>
        /// 保存配置好的列信息
        /// </summary>
        public List<ViewColumnInfo> mListViewColumns;

        //GridTree显示用的存储对象
        private PMDataItems mPMDatas;
        //保存所有查出的数据
        public List<PMShowDataItems>mAllListPMDatas;

        //保存柱状图等图展示函数的参数
        public TempChartQueryItem mChartItem;

        #region 查询栏
        /// <summary>
        /// 选择列表模式时加载checkbox,其余的加载comboxitem
        /// </summary>
        public bool IsUseCheck = true;
        public ObservableCollection<CheckComboboxItems> mListCheckComItems ;

        //保存查询栏条件
        public TempPMQueryCondition mTempCondition;
        //是否需要查询数据库，优化界面响应时间--目前触发查询的事件：切换KPI、切换周期类型、时间范围大于上一次保存数据的时间范围、UERA增加了几个。
        bool IsNeedQuery = true;

        public string TableName=string.Empty;
        /// <summary>
        /// 选择多个KPI的模式下保存周期类型的并集
        /// </summary>
        string mKPICycle = string.Empty;
        /// <summary>
        ///保存选择的多个KPI的ID，查询备用
        /// </summary>
        string mKPIMapping = string.Empty;

        /// <summary>
        /// 保存小时之后的查询条件，比如第几行、第几列
        /// </summary>
        string HourMinTimeSre = string.Empty;

        long dtStart;
        long dtEnd;

        private int mPageIndex;
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private bool IsContinue;
        #endregion

        public ProductManagementShow()
        {
            InitializeComponent();
            ListAllAERInfo = new List<UERAListInfo>();
            mRootTreeItem = new ObjectItem();
            mSkillTreeItem = new ObjectItem();
            mOrgObjectItem = new ObjectItem();
            mSkillObjectItem = new ObjectItem();
            mListPMItems = new List<PMItems>();
            mListDeletePmItems = new List<PMItems>();
            mAllListPMDatas = new List<PMShowDataItems>();
            mCurentPMDatas = new ObservableCollection<PMShowDataItems>();
            mListViewColumns = new List<ViewColumnInfo>();
            mPMDatas = new PMDataItems();
            mListCheckComItems = new ObservableCollection<CheckComboboxItems>();
            mTempCondition = new TempPMQueryCondition();
            mWorker = new BackgroundWorker();
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 200;
            mRecordTotal = 0;
            rabORG.Checked+=rabObject_Checked;
            rabOUERA.Checked += rabObject_Checked;
            rabSkill.Checked += rabObject_Checked;
            rabSUERA.Checked += rabObject_Checked;
            cbKeyPort.SelectionChanged += cbKeyPort_SelectionChanged;
            cbCycleType.SelectionChanged += cbCycleType_SelectionChanged;
            cbYear.SelectionChanged += cbYear_SelectionChanged;
            cbCycleTime.SelectionChanged += cbCycleTime_SelectionChanged;
            cbChartType.SelectionChanged += cbChartType_SelectionChanged;
            this.Loaded += ProductManagementShow_Loaded;
            OrgSTree.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OrgSTree_MouseLeftButtonDown), true);
            objectTree.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OrgSTree_MouseLeftButtonDown), true);
            //页面跳到第几页
            TxtPage.KeyUp += TxtPage_KeyUp;
            cbKeyPort_Table.ItemsSource = mListCheckComItems; 
            rabORG.IsChecked = true;
            IsContinue = false;
        }

                
        void ProductManagementShow_Loaded(object sender, RoutedEventArgs e)
        {
            mRootTreeItem = new ObjectItem();
            mOrgObjectItem = new ObjectItem();
            mSkillTreeItem = new ObjectItem();
            mSkillObjectItem = new ObjectItem();
            int sum = 0;
            try
            {
                ParentPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                //DateStart.Value = DateTime.Now.Date;
                //DateEnd.Value = DateTime.Now.Date.AddDays(1);
                ChangeLanguage();
                //BindCommands();
                S4601App.WeekStartDay = GetGlobalSetting("12010101");
                S4601App.MonthStartDay = GetGlobalSetting("12010102");
                CreatPageButtons();
                SetDateControlVisiable(2);
                CreateToolBarButtons();
                mWorker.DoWork += (s, de) =>
                {
                    if (sum == 0)
                    {
                        InitControlOrg(mRootTreeItem, "-1", "O");
                        InitControlOrg(mOrgObjectItem, "-1", "");
                        InitControlSkillGroup(mSkillTreeItem, "-1", "S");
                        InitControlSkillGroup(mSkillObjectItem, "-1", "");
                    }
                    GetPMSetting();
                    GetDeletePMSetting();
                    sum += 1;
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    treeType = "O";
                    rabORG.IsChecked = true;
                    rabSkill.IsChecked = rabSUERA.IsChecked = false;
                    mWorker.Dispose();
                    ParentPage.SetBusy(false,string.Empty);
                };
                mWorker.RunWorkerAsync();

                OrgSTree.ItemsSource = mRootTreeItem.Children;
                tabOrg.IsSelected = true;
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 0为周日,1星期一,6为星期六
        /// </summary>
        private int GetGlobalSetting(string GlobalSetType)
        {
            int tempType = 1;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetGlobalSetting;
                webRequest.ListData.Add(GlobalSetType);//
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                WebReturn webReturn = client.UPMSOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return tempType;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data))
                {
                    ShowException(string.Format("Fail.\t Data is null"));
                    return tempType;
                }
                tempType=int.Parse(webReturn.Data);
                if (GlobalSetType == "12010101")
                {
                    if (tempType == 0)
                    {
                        tempType = 7;
                    }
                }
                return tempType;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return tempType;
            }
        }

        #region  获取机构/技能组及其下属的用户、座席、真实、虚拟分机

        private void tabOrg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            treeType = "O";
            rabORG.IsChecked = true;
            rabSkill.IsChecked = rabSUERA.IsChecked = false;
            rabObject_Checked(sender,e);
        }

        private void tabSkill_MouseDown(object sender, MouseButtonEventArgs e)
        {
            treeType = "S";
            rabSkill.IsChecked = true;
            rabORG.IsChecked = rabOUERA.IsChecked = false;
            rabObject_Checked(sender, e);
        }
        private void rabObject_Checked(object sender, RoutedEventArgs e)
        {
            if (treeType.Equals("O"))
            {
                if (rabORG.IsChecked.Equals(true))
                {
                    objectTree.Visibility = Visibility.Collapsed;
                    OrgSTree.Visibility = Visibility.Visible;
                    OrgSTree.ItemsSource = mRootTreeItem.Children;
                    checkedObject = checkedOrg;
                }
                if (rabOUERA.IsChecked.Equals(true))
                {
                    OrgSTree.Visibility = Visibility.Collapsed;
                    objectTree.Visibility = Visibility.Visible;
                    objectTree.ItemsSource = mOrgObjectItem.Children;
                    checkedObject = checkOObject;
                }
            }
            else if (treeType.Equals("S"))
            {
                if (rabSkill.IsChecked.Equals(true))
                {
                    objectTree.Visibility = Visibility.Collapsed;
                    OrgSTree.Visibility = Visibility.Visible;
                    OrgSTree.ItemsSource = mSkillTreeItem.Children;
                    checkedObject = checkedSkiller;
                }
                if (rabSUERA.IsChecked.Equals(true))
                {
                    OrgSTree.Visibility = Visibility.Collapsed;
                    objectTree.Visibility = Visibility.Visible;
                    objectTree.ItemsSource = mSkillObjectItem.Children;
                    checkedObject = checkedSObject;
                }
            }
            InitQueryColums();

        }

        /// <summary>
        /// 获取机构极其下属座席
        /// </summary>
        public void InitControlOrg(ObjectItem parentItem, string OrgID,string type)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetOrgList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    UERAListInfo ctrolOrg = new UERAListInfo();
                    ctrolOrg.ID = arrInfo[0];
                    ctrolOrg.Name= ctrolOrg.FullName= arrInfo[1];
                    ctrolOrg.OrgParentID = arrInfo[2];
                    if (ListAllAERInfo.Where(p => p.ID == ctrolOrg.ID).Count() == 0)
                    {
                        ListAllAERInfo.Add(ctrolOrg);
                    }

                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(arrInfo[0]);
                    item.Name = arrInfo[1];
                    item.Data = strInfo;
                    item.IsChecked = false;
                    item.IsExpanded = true;
                    item.ParantID = arrInfo[2];
                    if (arrInfo[0] == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    InitControlOrg(item, arrInfo[0], type);
                    if (!type.Equals("O"))
                    {
                        InitControlUser(item, arrInfo[0]);
                        InitControlAgents(item, arrInfo[0], "G");
                        if (S4601App.GroupingWay.Contains("R"))
                        {
                            InitControlRealityExtension(item, arrInfo[0], "G");
                        }
                        if (S4601App.GroupingWay.Contains("E"))
                        {
                            InitControlExtension(item, arrInfo[0], "G");
                        }
                    }
                    AddChildObject(parentItem, item,type);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message+ex.StackTrace+ex.Source);
            }
        }


        /// <summary>
        /// 获取技能组极其下属座席
        /// </summary>
        public void InitControlSkillGroup(ObjectItem parentItem, string OrgID,string type)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetSkillGroup;
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SkillGroupInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    SkillGroupInfo ctrolSkill = optReturn.Data as SkillGroupInfo;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_TECHGROUP;
                    item.ObjID = Convert.ToInt64(ctrolSkill.SkillGroupID);
                    item.Name = ctrolSkill.SkillGroupName;
                    item.Icon = "Images/SkillGroup.ico";
                    item.IsChecked = false;
                    if (!type.Equals("S"))
                    {
                        InitControlAgents(item, ctrolSkill.SkillGroupID, "S");
                        if (S4601App.GroupingWay.Contains("R"))
                        {
                            InitControlRealityExtension(item, ctrolSkill.SkillGroupID, "S");
                        }
                        if (S4601App.GroupingWay.Contains("E"))
                        {
                            InitControlExtension(item, ctrolSkill.SkillGroupID, "S");
                        }
                    }
                    AddChildObject(parentItem, item,type);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlUser(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetCUser;
                webRequest.ListData.Add(parentID);
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UERAListInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UERAListInfo ctrolQa = optReturn.Data as UERAListInfo;
                    if (ctrolQa != null && ListAllAERInfo.Where(u => u.ID == ctrolQa.ID).Count() < 1)
                    {
                        ListAllAERInfo.Add(ctrolQa);
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(ctrolQa.ID);
                    item.ParantID = parentID;
                    item.Name = ctrolQa.Name;
                    item.Description = ctrolQa.FullName;
                    item.Data = strInfo;
                    item.IsChecked = false;
                    item.Icon = "Images/user.ico";
                    AddChildObject(parentItem, item,string.Empty);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItem parentItem, string parentID,string OrgOrSkillType)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetCAgentREx;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("A");
                webRequest.ListData.Add(OrgOrSkillType);
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    UERAListInfo ctrolAgent = new UERAListInfo();
                    ctrolAgent.ID = arrInfo[0];
                    ctrolAgent.Name = arrInfo[1];
                    ctrolAgent.FullName = arrInfo[2];
                    ctrolAgent.OrgID = parentID;
                    if (ListAllAERInfo.Where(p => p.ID == ctrolAgent.ID).Count() == 0 && !ctrolAgent.FullName.Equals("N/A"))
                    {
                        ListAllAERInfo.Add(ctrolAgent);
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(ctrolAgent.ID);
                    item.ParantID = parentID;
                    item.Name = ctrolAgent.Name;
                    item.Description = ctrolAgent.FullName;
                    item.Data = strInfo;
                    item.IsChecked = false;
                    item.Icon = "Images/agent.ico";
                    if (item.Name != "N/A")
                    {
                        AddChildObject(parentItem, item, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealityExtension(ObjectItem parentItem, string parentID, string OrgOrSkillType)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetCAgentREx;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("R");
                webRequest.ListData.Add(OrgOrSkillType);
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    UERAListInfo ctrolRex = new UERAListInfo();
                    ctrolRex.ID = arrInfo[0];
                    ctrolRex.Name = arrInfo[1];
                    ctrolRex.FullName = arrInfo[1];
                    ctrolRex.OrgID = parentID;
                    if (ListAllAERInfo.Where(p => p.ID == ctrolRex.ID).Count() == 0)
                    {
                        ListAllAERInfo.Add(ctrolRex);
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(ctrolRex.ID);
                    item.ParantID = parentID;
                    item.Name = ctrolRex.Name;
                    item.Description = ctrolRex.FullName;
                    item.Data = strInfo;
                    item.IsChecked = false;
                    item.Icon = "Images/RealExtension.ico";
                    AddChildObject(parentItem, item, string.Empty);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtension(ObjectItem parentItem, string parentID, string OrgOrSkillType)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetCAgentREx;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("E");
                webRequest.ListData.Add(OrgOrSkillType);
                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UPMSOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    UERAListInfo ctrolEx = new UERAListInfo();
                    ctrolEx.ID = arrInfo[0];
                    ctrolEx.Name = arrInfo[1];
                    ctrolEx.FullName = arrInfo[1];
                    ctrolEx.OrgID = parentID;
                    if (ListAllAERInfo.Where(p => p.ID == ctrolEx.ID).Count() == 0)
                    {
                        ListAllAERInfo.Add(ctrolEx);
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(ctrolEx.ID);
                    item.ParantID = parentID;
                    item.Name = ctrolEx.Name;
                    item.Description = ctrolEx.FullName;
                    item.Data = strInfo;
                    item.IsChecked = false;
                    item.Icon = "Images/extension.ico";
                    AddChildObject(parentItem, item, string.Empty);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddChildObject(ObjectItem parentItem, ObjectItem item,string type)
        {
            if (type.Equals("O") || type.Equals("S"))
            {
                item.IsExpanded = true;
            }
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }
        
        /// <summary>
        /// type=1,表示不需要机构、技能组ID
        /// </summary>
        private void GetUERAIsCheck(ObjectItem parent, ref string strUERA ,int type)
        {
            //是否继续查询子树
            bool isQueryChild = true;
            try
            {
                foreach (ObjectItem o in parent.Children)
                {
                    if (o.Isselected.Equals(true)||o.IsChecked.Equals(true))
                    {
                        if (type == 0 )
                        {
                            isQueryChild = false;
                            strUERA += o.ObjID.ToString() + ",";
                        }
                        else if (type == 1 && o.ObjType != ConstValue.RESOURCE_ORG && o.ObjType != ConstValue.RESOURCE_TECHGROUP)
                        {
                            isQueryChild = true;
                            strUERA += o.ObjID.ToString() + ",";
                        }
                        UERAListInfo temp = new UERAListInfo();
                        temp.FullName = o.Description;
                        if (o.ObjType == ConstValue.RESOURCE_ORG) { temp.FullName = o.Name; }
                        temp.Name = o.Name;
                        temp.ID = o.ObjID.ToString();
                        temp.OrgParentID = o.ParantID;
                        checkedUERA.Add(temp);

                    }
                    //选择机构树时,根机构下属的子机构数据会被一起查出，所以被选中对象将排除根机构下属的子机构。
                    if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0 && isQueryChild.Equals(true))
                    {
                        GetUERAIsCheck(o, ref strUERA,type);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        
        private void OrgSTree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cbKeyPort.Items.Clear();
                mListCheckComItems.Clear();
                Thread t = new Thread(() =>
                {
                    Thread.Sleep(500);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        checkedObject = string.Empty;
                        checkedUERA.Clear();
                        IsNeedQuery = true;
                        if (treeType.Equals("O"))
                        {
                            if (rabORG.IsChecked.Equals(true))
                            {
                                GetUERAIsCheck(mRootTreeItem, ref checkedObject,0);
                            }
                            else if (rabOUERA.IsChecked.Equals(true))
                            {
                                GetUERAIsCheck(mOrgObjectItem, ref checkedObject,1);
                            }
                        }
                        else if (treeType.Equals("S"))
                        {
                            if (rabSkill.IsChecked.Equals(true))
                            {
                                GetUERAIsCheck(mSkillTreeItem, ref checkedObject, 0);
                            }
                            else if (rabSUERA.IsChecked.Equals(true))
                            {
                                GetUERAIsCheck(mSkillObjectItem, ref checkedObject,1);
                            }
                        }
                        checkedObject = checkedObject.TrimEnd(',');

                        if (rabORG.IsChecked.Equals(true))
                        {
                            checkedOrg = checkedObject;
                        }
                        if (rabOUERA.IsChecked.Equals(true))
                        {
                            checkOObject = checkedObject;
                        }
                        if (rabSkill.IsChecked.Equals(true))
                        {
                            checkedSkiller = checkedObject;
                        }
                        if (rabSUERA.IsChecked.Equals(true))
                        {
                            checkedSObject = checkedObject;
                        }
                        InitQueryColums();
                    }));
                });
                t.Start();
            }
            catch (Exception)
            {

            }
        }
        #endregion


        #region 初始化查询栏条件
        private void InitQueryColums()
        {
            try
            {
                cbKeyPort.Items.Clear();
                mListCheckComItems.Clear();
                cbKeyPort_Table.Text = string.Empty;
                ComboBoxItem cbItem;
                CheckComboboxItems checkcombItem;
                for (int i = 0; i < mListPMItems.Count; i++)
                {
                     cbItem= new ComboBoxItem();
                     checkcombItem = new CheckComboboxItems();
                     cbItem.Content = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", mListPMItems[i].KPIID), mListPMItems[i].KPIName);
                     checkcombItem.Description = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", mListPMItems[i].KPIID), mListPMItems[i].KPIName);
                    checkcombItem.KPIMappingID = mListPMItems[i].KPIID.ToString();
                    checkcombItem.KPICycle = mListPMItems[i].KPICycle;

                    OperationInfo opt = new OperationInfo();
                    opt.ID = mListPMItems[i].KPIID;
                    opt.Description = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", mListPMItems[i].KPIID), mListPMItems[i].KPIName);
                    cbItem.DataContext = opt;
                    if (MatchObject(mListPMItems[i]).Equals(true))
                    {
                        mListCheckComItems.Add(checkcombItem);
                        cbKeyPort.Items.Add(cbItem);
                    }
                }
                cbKeyPort.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //获取KPI设定
        private void GetPMSetting()
        {
            mListPMItems.Clear();
            try
            {
                 WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetPMSetting;
                webRequest.ListData.Add("1");//1 获取可用的KPI绑定设置
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //Service46012Client client = new Service46012Client();
                WebReturn webReturn = client.UPMSOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tPmData is null"));
                    return;
                }
                bool flag = true;// 存在重复KPI设为false
                int tempArrInt=0;
                int arrInt = -1;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<PMItems>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    PMItems items = optReturn.Data as PMItems;
                    flag = true;
                    if (mListPMItems.Where(p => p.KPIID == items.KPIID).Count() > 0)
                    {
                        tempArrInt=mListPMItems.Where(p => p.KPIID == items.KPIID).FirstOrDefault().arrInt;
                        mListPMItems[tempArrInt].KPICycle = ReturnIntersectionCycle(mListPMItems[tempArrInt].KPICycle,items.KPICycle);
                        if (mListPMItems[tempArrInt].KpiObjectID.Contains(items.ObjectID.ToString()).Equals(false))
                        {
                            mListPMItems[tempArrInt].KpiObjectID += "," + items.ObjectID.ToString();
                        }
                        flag = false;
                    }
                    else
                    {
                        arrInt += 1;
                    }
                    items.arrInt = arrInt;
                    if (items != null && flag==true)
                    {
                        mListPMItems.Add(items);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return;
            }
        }

        private void GetDeletePMSetting()
        {
            mListDeletePmItems.Clear();
            try
            {                
                 WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.GetPMSetting;
                webRequest.ListData.Add("0");//1 获取可用的KPI绑定设置
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                //Service46012Client client = new Service46012Client();
                WebReturn webReturn = client.UPMSOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tPmData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<PMItems>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    PMItems items = optReturn.Data as PMItems;
                    mListDeletePmItems.Add(items);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private Boolean MatchObject(PMItems Items)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(checkedObject)) { return false; }
                string[] tempArray = checkedObject.Split(',');
                foreach (string tempString in tempArray)
                {
                    if (!Items.KpiObjectID.Contains(tempString)) { return false; }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        void ckItem_Checked(object sender, RoutedEventArgs e)
        {
            string KpiName = string.Empty;
            mKPIMapping = string.Empty;
            mKPICycle = string.Empty;
            int i = 0;
            string cycle1 = string.Empty;
            string cycle2 = string.Empty;
            foreach(CheckComboboxItems item in mListCheckComItems)
            {
                if (item.IsSelected.Equals(true))
                {
                    mKPIMapping += item.KPIMappingID + ",";
                    KpiName += item.Description + ",";
                    if (i % 2 == 0)
                    {
                        cycle1 = item.KPICycle;
                    }
                    else
                    {
                        cycle2 = item.KPICycle;
                    }
                    if (i > 0)
                    { 
                        mKPICycle = ReturnUnionCycle(cycle1, cycle2);
                        if (i % 2 == 0)
                        {
                            cycle1 = mKPICycle;
                        }
                        else
                        {
                            cycle2 = mKPICycle;
                        }
                    }
                    else { mKPICycle = cycle1; }
                    i++;
                }//if.end
            }//foreach.end
            KpiName = KpiName.TrimEnd(',');
            mKPIMapping = mKPIMapping.TrimEnd(',');
            cbKeyPort_Table.Text = KpiName;
            InitCycleCombobox();
        }

        /// <summary>
        /// 取出多个KPI周期的交集
        /// </summary>
        /// <returns></returns>
        private string ReturnUnionCycle(string cycle1,string cycle2)
        {
            string unionCycle = string.Empty;
            if (string.IsNullOrWhiteSpace(cycle1) || string.IsNullOrWhiteSpace(cycle2)) { return string.Empty; }
            try
            {
                char[] char1=cycle1.ToCharArray();
                char[] char2=cycle2.ToCharArray();
                int i=0;
                foreach (char temp in char1)
                {
                    if (char1[i] == '1' && char2[i] == '1')
                    {
                        char1[i] = '1';
                    }
                    else
                    {
                        char1[i] = '0';
                    }
                    i++;
                }
                unionCycle = new string(char1);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return string.Empty;
            }
            return unionCycle;
        }
        
        /// <summary>
        /// 取出多个KPI周期的并集
        /// </summary>
        /// <returns></returns>
        private string ReturnIntersectionCycle(string cycle1, string cycle2)
        {
            string unionCycle = string.Empty;
            if (string.IsNullOrWhiteSpace(cycle1) || string.IsNullOrWhiteSpace(cycle2)) { return cycle1; }
            try
            {
                char[] char1 = cycle1.ToCharArray();
                char[] char2 = cycle2.ToCharArray();
                int i = 0;
                foreach (char temp in char1)
                {
                    if (char1[i] == '1' || char2[i] == '1')
                    {
                        char1[i] = '1';
                    }
                    i++;
                }
                unionCycle = new string(char1);
            }
            catch (Exception ex)
            {
                return cycle1;
            }
            return unionCycle;
        }

        //选择相应的KPI后加载设置好的实际应用周期
        void cbKeyPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Thread.Sleep(150);
            InitCycleCombobox();
        }

        private void InitCycleCombobox()
        {
            OperationInfo opt;
            if (mListPMItems.Count() > 0)
            {
                cbCycleType.Items.Clear();
                if (IsUseCheck.Equals(true))
                {
                    if (string.IsNullOrWhiteSpace(mKPICycle)) { return; }
                }
                else//图表
                {
                    if (cbKeyPort.SelectedIndex < 0) { return; }
                    var temp = cbKeyPort.SelectedItem as ComboBoxItem;
                    if (temp == null) { return; }
                    opt = temp.DataContext as OperationInfo;
                    //mKPICycle = mListPMItems[cbKeyPort.SelectedIndex].KPICycle;
                    mKPICycle = mListPMItems.Where(p => p.KPIID == opt.ID).FirstOrDefault().KPICycle;
                }
                char[] arrInfo = mKPICycle.ToCharArray();
                if (arrInfo == null) { return; }

                #region 加载comboxitem
                ComboBoxItem cbItem;
                if (arrInfo[0] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 1;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00014", "Year");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[1] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 2;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00015", "Month");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[2] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 3;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00016", "Week");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[3] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 4;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00017", "Day");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[4] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 5;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00018", "1Hour");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[5] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 6;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00019", "30Min");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[6] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 7;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00020", "15Min");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[7] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 8;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00021", "10Min");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                if (arrInfo[8] == '1')
                {
                    cbItem = new ComboBoxItem();
                    opt = new OperationInfo();
                    opt.ID = 9;
                    cbItem.Content = CurrentApp.GetLanguageInfo("4601P00022", "5 Min");
                    cbItem.DataContext = opt;
                    cbCycleType.Items.Add(cbItem);
                }
                cbCycleType.SelectedIndex = 0;
                #endregion
            }
        }

        private void cbCycleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = cbCycleType.SelectedItem as ComboBoxItem;
            if (temp != null)
            {
                DateEnd.Visibility = Visibility.Visible;
                cbYear.Items.Clear();
                cbCycleTime.Items.Clear();
                OperationInfo tCycleOpt = temp.DataContext as OperationInfo;
                ComboBoxItem item;
                OperationInfo opt;
                switch (tCycleOpt.ID)
                {
                    case 1://年
                        SetDateControlVisiable(2);
                        cbYear.Visibility = Visibility.Visible;
                        for (int i = 0; i < 10; i++)
                        {
                            item = new ComboBoxItem();
                            opt = new OperationInfo();
                            item.Content = DateTime.Now.Year - i;
                            opt.Display = (DateTime.Now.Year - i).ToString();
                            item.DataContext = opt;
                            cbYear.Items.Add(item);
                        }
                        break;
                    case 2://月
                    case 3://周
                        DateStart.Text = string.Empty;
                        DateStart.Mask = "0000/00/00";
                        DateStart.Width = 85;
                        DateEnd.Text = string.Empty;
                        DateEnd.Mask = "0000/00/00";
                        DateEnd.Width = 85;
                        SetDateControlVisiable(0);
                        if (tCycleOpt.ID == 2)
                        {
                            DateStart.Visibility = Visibility.Collapsed;
                            DateEnd.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            DateStart.Visibility = Visibility.Visible;
                            DateStart.IsEnabled = false;
                            DateEnd.Visibility = Visibility.Visible;
                            DateEnd.IsEnabled = false;
                        }
                        cbCycleTime.Visibility = Visibility.Visible;
                        for (int i = 0; i < 3; i++)
                        {
                            item = new ComboBoxItem();
                            opt = new OperationInfo();
                            item.Content = (DateTime.Now.Year - i).ToString();
                            opt.Display = (DateTime.Now.Year - i).ToString();
                            item.DataContext = opt;
                            cbYear.Items.Add(item);
                        }
                        break;
                    case 4://天
                        //cbCycleTime.Visibility = Visibility.Visible;
                        //DateStart.Text = string.Empty;
                        //DateStart.Mask = "00-00";
                        //DateStart.Width =50;
                        //DateStart.Text = DateTime.Now.Date.ToString("MM-dd");
                        //DateStart.Visibility = Visibility.Visible;
                        //DateStart.IsEnabled = true;
                        //DateEnd.Text = string.Empty;
                        //DateEnd.Visibility = Visibility.Collapsed;
                        SetDateControlVisiable(1);
                        if (DateTime.Now.Month < 6)//如果当前月小于6月，增加去年的月份
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                item.Content = (DateTime.Now.Year - i).ToString();
                                opt.Display = (DateTime.Now.Year - i).ToString();
                                item.DataContext = opt;
                                cbYear.Items.Add(item);
                            }
                            for (int i = 0; i < DateTime.Now.Month; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                item.Content = (DateTime.Now.Month - i).ToString();
                                opt.Display = (DateTime.Now.Month - i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        else
                        {
                            item = new ComboBoxItem();
                            item.Content = DateTime.Now.Year.ToString();
                            cbYear.Items.Add(item);
                            for (int i = 0; i < 6; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                item.Content = (DateTime.Now.Month - i).ToString();
                                opt.Display = (DateTime.Now.Month - i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        break;
                    case 5://小时、分钟之后都只显示 
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        SetDateControlVisiable(0);
                        borCycleTime.Visibility = Visibility.Collapsed;
                        DateStart.Text = string.Empty;
                        DateStart.Mask = "00:00";
                        DateStart.Width =50;
                        DateStart.Text = DateTime.Now.Date.ToString("HH:mm");
                        DateStart.IsEnabled = true;
                        DateEnd.Text = string.Empty;
                        DateEnd.Visibility = Visibility.Collapsed;
                        for (int i = 0; i < 7; i++)
                        {
                            item = new ComboBoxItem();
                            opt = new OperationInfo();
                            item.Content = (DateTime.Now.AddDays(-i)).ToString("yyyy MM-dd");
                            opt.Display = (DateTime.Now.AddDays(-i)).ToString("yyyy MM-dd");
                            item.DataContext = opt;
                            cbYear.Items.Add(item);
                        }
                        break;
                }
                cbYear.SelectedIndex = 0;
            }
        }

        private void SetDateControlVisiable(int i)
        {
            if (i == 0)//非日的周期
            {
                borDatePicker.Visibility = Visibility.Collapsed;
                cbYear.Visibility = Visibility.Visible;
                borCycleTime.Visibility = Visibility.Visible;
                DateStart.Visibility = Visibility.Visible;
                DateEnd.Visibility = Visibility.Visible;
            }
            else if (i == 1)//天
            {
                cbYear.Visibility = Visibility.Collapsed;
                borCycleTime.Visibility = Visibility.Collapsed;
                DateStart.Visibility = Visibility.Collapsed;
                DateEnd.Visibility = Visibility.Collapsed;
                borDatePicker.Visibility = Visibility.Visible;
                DatePicker.Value = DateTime.Now.Date;
            }
            else if (i == 2)//初始化，全不显示
            {
                cbYear.Visibility = Visibility.Collapsed;
                borCycleTime.Visibility = Visibility.Collapsed;
                DateStart.Visibility = Visibility.Collapsed;
                DateEnd.Visibility = Visibility.Collapsed;
                borDatePicker.Visibility = Visibility.Collapsed;
            }
        }
        
        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCycleTime.Items.Clear();
            var temp = cbCycleType.SelectedItem as ComboBoxItem;
            if (temp != null)
            {
                OperationInfo tCycleOpt = temp.DataContext as OperationInfo;
                ComboBoxItem item;
                OperationInfo opt;
                temp = cbYear.SelectedItem as ComboBoxItem;
                if (temp == null) { return; }
                OperationInfo tempContext = temp.DataContext as OperationInfo;
                if (tempContext == null) { return; }
                switch (tCycleOpt.ID)
                {
                    case 1://年
                        DateStart.Mask = "0000/00/00";
                        DateStart.Text = string.Format("{0}/01/01", tempContext.Display);
                        break;
                    case 2://月
                        if (tempContext.Display != DateTime.Now.Year.ToString())//非今年年份，说明当前月小于12月
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                item.Content = (1 + i).ToString() + CurrentApp.GetLanguageInfo("4601P00015", "month");
                                opt.Display = (1+ i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        else//今年的月份
                        {
                            for (int i = 0; i < DateTime.Now.Month; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                item.Content = (1 + i).ToString() + CurrentApp.GetLanguageInfo("4601P00015", "month");
                                opt.Display = (1 + i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        break;
                    case 3: //周
                        if (tempContext.Display != DateTime.Now.Year.ToString())//非今年年份
                        {
                            long tempSum=CycleTimeConverter.WeekTime(DateTime.Now.Year,3)[2];
                            for (int i = 0; i < tempSum; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                //item.Content = CurrentApp.GetLanguageInfo(" ", "第")+(1 + i).ToString() + CurrentApp.GetLanguageInfo("4601P00016", "week");
                                item.Content = string.Format(CurrentApp.GetLanguageInfo("4601P00035", "{0}week"), (1 + i).ToString());
                                opt.Display = (1 + i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < CycleTimeConverter.GetValidTime(DateTime.Now, 3)[0]; i++)
                            {
                                item = new ComboBoxItem();
                                opt = new OperationInfo();
                                //item.Content = (1 + i).ToString() + CurrentApp.GetLanguageInfo("4601P00016", "week");
                                item.Content = string.Format(CurrentApp.GetLanguageInfo("4601P00035", "{0}week"), (1 + i).ToString());
                                opt.Display = (1 + i).ToString();
                                item.DataContext = opt;
                                cbCycleTime.Items.Add(item);
                            }
                        }
                        //cbCycleTime.SelectedIndex = 0;
                        break;
                    case 4://天
                        cbCycleTime.Visibility = Visibility.Collapsed;
                        DateStart.Mask = "00-00";
                        DateStart.Text = DateTime.Now.Date.ToString("MM-dd");
                        break;
                    case 5://小时
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        DateTime temp111 = Convert.ToDateTime(tempContext.Display);
                        DateStart.Text = temp111.ToString("HH:mm");
                        DateEnd.Mask="0000/00/00";
                        DateEnd.Text = string.Format("{0}", temp111.ToString("yyyy/MM/dd"));
                        break;

                }
            }
        }

       private void cbCycleTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = cbCycleType.SelectedItem as ComboBoxItem;
            if (temp != null)
            {
                OperationInfo tempId = temp.DataContext as OperationInfo;
                if (tempId != null)
                {
                    temp = cbCycleTime.SelectedItem as ComboBoxItem;
                    if (temp != null)
                    {
                        OperationInfo tCycleOpt = temp.DataContext as OperationInfo;
                        if (tCycleOpt != null)
                        {
                            switch (tempId.ID)
                            {
                                //case 1:因为周期为年，不会触发这个事件
                                //    break;
                                case 2://月
                                    DateStart.Mask = "0000/00/00";
                                    DateStart.Text = string.Format("{0}/{1}/01", cbYear.Text, tCycleOpt.Display.PadLeft(2,'0'));
                                    break;
                                case 3://周
                                    long startTime = CycleTimeConverter.WeekTime(int.Parse(cbYear.Text), int.Parse(tCycleOpt.Display))[0];
                                    long endTime = CycleTimeConverter.WeekTime(int.Parse(cbYear.Text), int.Parse(tCycleOpt.Display))[1];
                                    DateStart.Mask = "0000/00/00";
                                    DateStart.Width = 85;
                                    DateStart.IsEnabled = false;
                                    DateStart.Text = CycleTimeConverter.NumberToDatetime(startTime.ToString()).ToString("yyyy/MM/dd");
                                    DateEnd.Mask = "0000/00/00";
                                    DateEnd.Width = 85;
                                    DateEnd.IsEnabled = false;
                                    DateEnd.Text = CycleTimeConverter.NumberToDatetime(endTime.ToString()).ToString("yyyy/MM/dd");
                                    break;
                                //case 4:因为周期为日的控件使用的是另外一个，也不会触发这个事件
                                //    break;
                            }
                        }
                    }
                }
            }
        }
                
       private void CreateToolBarButtons()
       {
           try
           {
               PanelToolButton.Children.Clear();
               ToolButtonItem toolItem;
               Button btn;
               //保存列的顺序
               toolItem = new ToolButtonItem();
               toolItem.Name = "BT" + "ColumnSetting";
               toolItem.Display = CurrentApp.GetLanguageInfo("3103T00168", "Save Columns");
               toolItem.Tip = CurrentApp.GetLanguageInfo("3103T00168", "Save Columns");
               toolItem.Icon = "Images/setting.png";
               btn = new Button();
               btn.Click += ToolButton_Click;
               btn.DataContext = toolItem;
               btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
               btn.HorizontalAlignment = HorizontalAlignment.Right;
               PanelToolButton.Children.Add(btn);

           }
           catch (Exception ex)
           {
               ShowException(ex.Message);
           }
       }

       void ToolButton_Click(object sender, RoutedEventArgs e)
       {
           var toolBtn = e.Source as Button;
           if (toolBtn != null)
           {
               ToolButtonItem item = toolBtn.DataContext as ToolButtonItem;
               if (item != null)
               {
                   switch (item.Name)
                   {
                       case "BTColumnSetting":
                           PopupPanel.Title = "PM Show Setting";
                           PMShowSetting ucPmSetting = new PMShowSetting();
                           ucPmSetting.PageParent = this;
                           ucPmSetting.CurrentApp = CurrentApp;
                           PopupPanel.Content = ucPmSetting;
                           PopupPanel.IsOpen = true;
                           break;
                   }
               }
           }
       }

        #endregion
        
       private bool GetPMDataOperation()
        {
            try
            {
                if (IsUseCheck.Equals(false))
                {
                    if (cbKeyPort.SelectedIndex < 0 || cbCycleType.SelectedIndex<0)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("4601P00009", "Please check query condition"));
                        return false;
                    }
                    else
                    {
                        //mKPIMapping = mListPMItems[cbKeyPort.SelectedIndex].KPIID.ToString();
                        var tempKPIMapping = cbKeyPort.SelectedItem as ComboBoxItem;
                        if (tempKPIMapping == null) { return false; }
                        OperationInfo opt = tempKPIMapping.DataContext as OperationInfo;
                        mKPIMapping = opt.ID.ToString();
                    }
                }

                if (string.IsNullOrWhiteSpace(mKPIMapping) || cbChartType.SelectedIndex < 0 || cbCycleType.SelectedIndex < 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("4601P00009", "Please check query condition"));
                    return false;
                }
                if (checkedUERA.Count == 0) return false;
                
                var temp = cbCycleType.SelectedItem as ComboBoxItem;
                OperationInfo tCycleOpt = temp.DataContext as OperationInfo;
                if (IsNeedQuery)
                {
                    switch (tCycleOpt.ID)//查询
                    {
                        case 1://年
                            TableName="T_46_015";
                            break;

                        case 2://月
                            if (cbCycleTime.SelectedIndex < 0)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("4601P00009", "Please check query condition"));
                                return false;
                            }
                            TableName = "T_46_014";
                            break;

                        case 3://周
                            if (cbCycleTime.SelectedIndex < 0)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("4601P00009", "Please check query condition"));
                                return false;
                            }
                            TableName = "T_46_013";
                            break;

                        case 4://日
                            TableName = "T_46_012";
                            break;

                        case 5://1小时

                        case 6://30分钟

                        case 7://15分钟

                        case 8://10分钟

                        case 9://5分钟
                            TableName = "T_46_011";
                            break;

                        default:
                            break;
                    }
                    string tempDateStr = string.Empty;
                    tempDateStr = DateStart.Text;
                    if (tCycleOpt.ID == 4)//天
                    {
                        DateStart.Mask = "0000/00/00";
                        if (DatePicker.Value.Value.Year < 2000) { DatePicker.Value = DateTime.Now.AddMonths(-6).Date; }//时间异常的处理方式
                        DateStart.Text = DateTime.Parse(DatePicker.Value.ToString()).ToString("yyyy/MM/dd");
                        tempDateStr = DateStart.Text;
                    }
                    else if (tCycleOpt.ID >=5)//小时之后的周期
                    {
                        DateTime tempdate0 = DateTime.Now;
                        if (!DateTime.TryParse(DateStart.Text, out tempdate0))//时间异常的处理方式
                        {
                            DateStart.Text = tempdate0.ToString("HH:mm");
                        }
                        tempDateStr = string.Format("{0} {1}", DateEnd.Text, DateStart.Text);
                        HourMinTimeSre = CycleTimeConverter.ReturnMinutesStr(DateTime.Parse(tempDateStr),tCycleOpt.ID);
                    }
                    List<long> tempListTime = CycleTimeConverter.GetValidTime(DateTime.Parse(tempDateStr), tCycleOpt.ID);
                    if (tCycleOpt.ID == 3)
                    {
                        dtStart = Convert.ToInt64(DateTime.Parse(DateStart.Text).ToString("yyyyMMddHHmmss"));
                        dtEnd = Convert.ToInt64(DateTime.Parse(DateEnd.Text).ToString("yyyyMMddHHmmss"));
                    }
                    else
                    {
                        if (tempListTime.Count() < 2) { return false; }
                        dtStart = tempListTime[0];
                        dtEnd = tempListTime[1];
                    }
                    mPMDatas = new PMDataItems();
                    mAllListPMDatas.Clear();
                    //这个是按KPI分组，或者是以机构为查询对象
                    int brushInt = 0;
                    if (rabORG.IsChecked.Equals(true) || rabSkill.IsChecked.Equals(true) || mListViewColumns.Where(p => p.SortID == 0).First().ColumnName == "KPIName")
                    {
                        string[] tempKPIMapping = mKPIMapping.Split(',');
                        for (int i = 0; i < tempKPIMapping.Count(); i++)
                        {
                            if (!IsContinue) { return false; }
                            IsContinue = QueryPMDatas(TableName, tCycleOpt.ID, dtStart, dtEnd, mPMDatas, ref brushInt, checkedObject, tempKPIMapping[i], 0);
                        }
                    }
                    //这个是以对象为查询方式，并且按对象分组
                   else if (mListViewColumns.Where(p => p.SortID == 0).First().ColumnName == "UERName")
                    {
                        string[] tempObject = checkedObject.Split(',');
                        for (int i = 0; i < tempObject.Count(); i++)
                        {
                            if (!IsContinue) { return false; }
                            IsContinue = QueryPMDatas(TableName, tCycleOpt.ID, dtStart, dtEnd, mPMDatas, ref brushInt, tempObject[i], mKPIMapping, 1);
                        }
                    }
                }


                #region 

                List<DateTime> tempTime = new List<DateTime>();
                List<PMShowDataItems> tempPmData = new List<PMShowDataItems>();
                List<long> tempUser = new List<long>();

                tempPmData = mAllListPMDatas.Where(p => p.StartLocalTime >= dtStart && p.StartLocalTime <= dtEnd).ToList();
                foreach (PMShowDataItems tempItem in tempPmData)
                {
                    DateTime dtday ;
                    if (tCycleOpt.ID == 1 || tCycleOpt.ID >= 5)
                    {
                        dtday = tempItem.dtLocalTime;
                    }
                    else
                    {
                        dtday = new DateTime(tempItem.pmYear, tempItem.pmMonth, tempItem.pmDay);
                    }
                    if (!tempTime.Contains(dtday))
                        tempTime.Add(dtday);
                }
                tempTime.Sort();
                #endregion

                PmShowGrid.Children.Clear();
                PmShowGrid.RowDefinitions.Clear();
                PmShowGrid.ColumnDefinitions.Clear();
                mChartItem = new TempChartQueryItem();
                mChartItem.DataList = tempPmData;
                mChartItem.ListDateTime = tempTime;
                mChartItem.Type = tCycleOpt.ID;
                mChartItem.ChartType = cbChartType.SelectedIndex;
                switch (cbChartType.SelectedIndex)
                {
                    case 0://列表
                        CreateCharTable();
                        break;
                    case 1:// 柱状图
                        CreateChartColumn1(tempTime, checkedUERA, tCycleOpt.ID, tempPmData);
                        break;
                    case 2:// 点状图
                        //CreateChartSpline1(tempTime, checkedUERA, tCycleOpt.ID, tempPmData);折线图
                        CreateChartBubble1(tempTime, checkedUERA, tCycleOpt.ID, tempPmData);
                        break;
                    case 3:// 饼状图
                        for (int i = 0; i < checkedUERA.Count(); i++)
                        {
                            CreateChartPie1( tempTime, checkedUERA[i], tCycleOpt.ID, tempPmData, i);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            IsNeedQuery = true;
            return true;
        }

       //queryType=0,以Kpi分组; queryType=1,以对象分组.
        public bool QueryPMDatas(string tableName, long cycleType, long dtStart, long dtEnd, PMDataItems pmdatas, ref int brushInt, string objectID, string tempKPIMapping,int queryType)
        {
            bool flag = true;
            mCurentPMDatas.Clear();
            mRecordTotal = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4602Codes.QueryPMDatas;
                webRequest.ListData.Add(cycleType.ToString());//0     周期类型
                webRequest.ListData.Add(tempKPIMapping);//1     KPIID,T_46_003.C002,T46_001.C001
                webRequest.ListData.Add(objectID);//2     对象ID
                webRequest.ListData.Add(dtStart.ToString());//3     开始时间
                webRequest.ListData.Add(dtEnd.ToString());//4     结束时间
                webRequest.ListData.Add(HourMinTimeSre);//5     针对周期为小时之后的更细致的查询条件

                //Service46012Client client = new Service46012Client();
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46012"));
                WebReturn webReturn = client.UPMSOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    CurrentApp.WriteLog("PM Query Fail", webReturn.Message);
                    return false;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tPmData is null"));
                    return false;
                }
                long tempUserID = 0;//减少匹配全名的次数，减低查询耗时
                string fullName = string.Empty;
                PMDataItems tableItem;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string pmInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<PMShowDataItems>(pmInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        CurrentApp.WriteLog("PM Query Fail", optReturn.Message);
                        return false;
                    }
                    PMShowDataItems items = optReturn.Data as PMShowDataItems;
                    int total = mRecordTotal + 1;
                    if (items.UERAId == tempUserID)//如果ID相同就不需要进行匹配了
                    {
                        items.UERName = fullName;
                    }
                    else
                    {
                        items.UERName = ReturnFullName(items.UERAId);
                        fullName = items.UERName;
                    }
                    mRecordTotal = total;
                    items.dtLocalTime = DateTime.ParseExact(items.StartLocalTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);//yyyyMMddHHmmss代表24小时制
                    tempUserID = items.UERAId;
                    items.KPIName = mListPMItems.Where(p => p.KPIID == items.KPIID).FirstOrDefault().KPIName;
                    items.KPIName = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", items.KPIID), items.KPIName);
                    if (queryType == 0)
                    {
                        if (mAllListPMDatas.Where(p => p.KPIID == items.KPIID).Count() > 0) { items.KPIName = string.Empty; }
                    }
                    else if (queryType == 1)
                    {
                        if (mAllListPMDatas.Where(p => p.UERName == items.UERName).Count() > 0) { items.UERName = string.Empty; }
                    }
                    if (mListDeletePmItems.Where(p => p.KPIMappingID == items.KPIMappingID.ToString()).Count() <= 0)
                    {
                        mAllListPMDatas.Add(items);
                    }

                    tableItem = new PMDataItems(items);
                    tableItem.PMTime=string.Empty;
                    if (mListDeletePmItems.Where(p => p.KPIMappingID == items.KPIMappingID.ToString()).Count() <= 0)//没有被删除的kpi数据
                    {
                        if (brushInt % 2 != 0)
                        {
                            tableItem.Background = Brushes.LightGray;
                        }
                        else
                        {
                            tableItem.Background = Brushes.AntiqueWhite;
                        }
                        AddNewRecord(pmdatas, tableItem);
                        brushInt += 1;
                        string ChildAERO = string.Empty;
                        if (Convert.ToInt32(items.UERAId.ToString().Substring(0, 3)) == ConstValue.RESOURCE_USER ||
                            Convert.ToInt32(items.UERAId.ToString().Substring(0, 3)) == ConstValue.RESOURCE_ORG ||
                            Convert.ToInt32(items.UERAId.ToString().Substring(0, 3)) == ConstValue.RESOURCE_TECHGROUP)//这个对象为机构、技能组、用户时
                        {
                            foreach (UERAListInfo ueraItem in ListAllAERInfo)
                            {
                                if (ueraItem.OrgParentID == items.UERAId.ToString() || ueraItem.OrgID == items.UERAId.ToString())
                                {
                                    ChildAERO += ueraItem.ID + ",";
                                }
                            }
                            ChildAERO = ChildAERO.TrimEnd(',');
                            if (!string.IsNullOrWhiteSpace(ChildAERO))
                            {
                                flag=QueryPMDatas(tableName, cycleType, dtStart, dtEnd, tableItem, ref brushInt, ChildAERO, tempKPIMapping, queryType);
                                if (!flag) { return false; }
                            }
                        }
                    }
                }
                //mAllListPMDatas=mAllListPMDatas.OrderBy(p => p.StartLocalTime).ToList();
                //mPageIndex = 0;
                //FillListView();
                //SetPageState();
                CurrentApp.WriteLog("PM Query", webReturn.Message);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void AddNewRecord(PMDataItems parentItem, PMDataItems item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        #region 列表

        private void InitTableColumns()
        {
            try
            {
                mListViewColumns.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("4602001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                mListViewColumns = listColumns.OrderBy(c => c.SortID).ToList();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        public void CreateCharTable()
        {
            if (cbChartType.SelectedIndex > 0) { return; }//不是列表就不用运行下去了
            try
            {
                InitTableColumns();
                //ListView LvPM = new ListView();lv
                GridTree gtPM = new GridTree();                
                //string[] lans = "4601P00005,4601P00024,4601P00025,4601P00028,4601P00026,4601P00030,4601P00027".Split(',');
                //string[] cols = "KPIName,ActualValue,Goal1,Goal2,Trend1,Compare,Score".Split(',');
                //int[] colwidths = { 120,80, 80, 80, 80, 80, 80};
                string[] lans = new string[mListViewColumns.Count()];
                string[] cols = new string[mListViewColumns.Count()];
                int[] colwidths = new int[mListViewColumns.Count()];
                foreach (ViewColumnInfo item in mListViewColumns)
                {
                    if (rabORG.IsChecked.Equals(true))//以机构分组，固定的以name为第一列
                    {
                        lans[0] = string.Format("4601P00{0}",mListViewColumns.Where(p=>p.ColumnName=="KPIName").First().DataType.ToString().Substring(1, 3));
                        cols[0] = "KPIName";
                        colwidths[0] = 120;
                        if (item.SortID >= 2)
                        {
                            lans[item.SortID - 1] = string.Format("4601P00{0}", item.DataType.ToString().Substring(1, 3));
                            cols[item.SortID - 1] = item.ColumnName;
                            colwidths[item.SortID - 1] = item.Width;
                        }
                    }
                    else//以对象分组，第一列跟第二列根据配置来排列
                    {
                        lans[item.SortID ] = string.Format("4601P00{0}", item.DataType.ToString().Substring(1, 3));
                        cols[item.SortID] = item.ColumnName;
                        colwidths[item.SortID] = item.Width;
                    }
                }
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;
                List<GridViewColumn> listColumns=new List<GridViewColumn>();

                for (int i = 0; i < cols.Count(); i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "dtLocalTime")
                    {
                        var binding = new Binding(cols[i]);
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        gvc.DisplayMemberBinding = binding;
                    }
                    else if (cols[i]=="Trend1")
                    {
                        DataTemplate dt = Resources["CellTrendType1Template"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else if (cols[i] == "UERName")
                    {
                        DataTemplate dt = Resources["CellButtonTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else if (cols[i] == "Score")
                    {
                        DataTemplate dt = Resources["CellStandardTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else 
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    //ColumnGridView.Columns.Add(gvc);
                    if (rabORG.IsChecked.Equals(true))//以机构分组，固定的以name为第一列,所以要少一行
                    {
                        if (i == cols.Count() - 1) { break; }
                        if (mListViewColumns.Where(p => p.SortID-1 == i).First().Visibility == "1")//可见
                        {
                            listColumns.Add(gvc);
                        }
                    }
                    else
                    {
                        if (mListViewColumns.Where(p => p.SortID == i).First().Visibility == "1")//可见
                        {
                            listColumns.Add(gvc);
                        }
                    }
                }
                GridViewColumnHeader header = new GridViewColumnHeader();
                header.Content = CurrentApp.GetLanguageInfo("4601P00029", "Objects");
                //gtPM.SetColumns(header, 160, listColumns);
                int tempWidth = 160;
                if (rabOUERA.IsChecked.Equals(true))//以对象分组，隐藏gridtree固定的第一列
                {
                    header.Visibility = Visibility.Collapsed;
                    tempWidth = 0;
                }
                gtPM.SetColumns(header, tempWidth, listColumns);

                /*LvPM.View = ColumnGridView;
                var style = Resources["ListViewStyle"] as Style;
                if (style != null)
                {
                    LvPM.Style = style;
                }
                var itemStyle = Resources["ListViewItemPM"] as Style;
                if (itemStyle != null)
                {
                    LvPM.ItemContainerStyle = itemStyle;
                }**/
                var style = Resources["GridTreeStyle"] as Style;
                if (style != null)
                {
                    gtPM.Style = style;
                }
                var itemStyle = Resources["GridTreeItemStyle"] as Style;
                if (itemStyle != null)
                {
                    gtPM.ItemContainerStyle = itemStyle;
                }
                HierarchicalDataTemplate template = Resources["GridTreeItemTemplate"] as HierarchicalDataTemplate;
                if (itemStyle != null)
                {
                    gtPM.ItemTemplate = template;
                }
                PmShowGrid.Children.Add(gtPM);
                //LvPM.ItemsSource = mCurentPMDatas;
                gtPM.ItemsSource = mPMDatas.Children;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BindCommands()//ListView向下钻取的Binding事件
        {
            try
            {
                CommandBindings.Add(
                new CommandBinding(TableListViewCommand.ButtonCommand,
                    ButtonCommand_Executed,
                    (s, e) => e.CanExecute = true));
            }
            catch (Exception ex)
            {
            }
        }

        private void ButtonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PMShowDataItems Item = e.Parameter as PMShowDataItems;
            int tempID = Convert.ToInt32(Item.UERAId.ToString().Substring(0,3));
            if (tempID == ConstValue.RESOURCE_AGENT) { return; }
            string ChildAERO = string.Empty;
            foreach (UERAListInfo ueraItem in ListAllAERInfo)
            {
                if (ueraItem.OrgParentID == Item.UERAId.ToString() || ueraItem.OrgID == Item.UERAId.ToString())
                {
                    ChildAERO += ueraItem.ID + ",";
                }
            }
            ChildAERO = ChildAERO.TrimEnd(',');
            if (!string.IsNullOrWhiteSpace(ChildAERO))
            {
                checkedObject = ChildAERO; 
                var temp = cbCycleType.SelectedItem as ComboBoxItem;
                OperationInfo tCycleOpt = temp.DataContext as OperationInfo;
                //QueryPMDatas(TableName, tCycleOpt.ID, dtStart, dtEnd);
            }
        }
        
        #endregion

        #region 翻页功能及按钮事件

        private void CreatPageButtons()
        {
            List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
            ToolButtonItem item = new ToolButtonItem();
            //item.Name = "TB" + "StopQueryMark";
            //item.Display = CurrentApp.GetLanguageInfo("3103T00080", "Stop Query");
            //item.Tip = CurrentApp.GetLanguageInfo("3103T00080", "Stop Query");
            //item.Icon = "Images/stop.png";
            //listBtns.Add(item);

            item = new ToolButtonItem();
            item.Name = "TB" + "FirstPage";
            item.Display = CurrentApp.GetLanguageInfo("3103T00081", "First Page");
            item.Tip = CurrentApp.GetLanguageInfo("3103T00081", "First Page");
            item.Icon = "Images/go_first.png";
            listBtns.Add(item);
            item = new ToolButtonItem();
            item.Name = "TB" + "PrePage";
            item.Display = CurrentApp.GetLanguageInfo("3103T00082", "Pre Page");
            item.Tip = CurrentApp.GetLanguageInfo("3103T00082", "Pre Page");
            item.Icon = "Images/go_previous.png";
            listBtns.Add(item);
            item = new ToolButtonItem();
            item.Name = "TB" + "NextPage";
            item.Display = CurrentApp.GetLanguageInfo("3103T00083", "Next Page");
            item.Tip = CurrentApp.GetLanguageInfo("3103T00083", "Next Page");
            item.Icon = "Images/go_next.png";
            listBtns.Add(item);
            item = new ToolButtonItem();
            item.Name = "TB" + "LastPage";
            item.Display = CurrentApp.GetLanguageInfo("3103T00084", "Last Page");
            item.Tip = CurrentApp.GetLanguageInfo("3103T00084", "Last Page");
            item.Icon = "Images/go_last.png";
            listBtns.Add(item);
            

            PanelPageButtons.Children.Clear();
            for (int i = 0; i < listBtns.Count; i++)
            {
                ToolButtonItem toolBtn = listBtns[i];
                Button btn = new Button();
                btn.DataContext = toolBtn;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                btn.Click += PageButton_Click;
                PanelPageButtons.Children.Add(btn);
            }
        }

        void PageButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn != null)
            {
                var item = btn.DataContext as ToolButtonItem;
                if (item == null) { return; }
                switch (item.Name)
                {
                    case "TB" + "FirstPage":
                        if (mPageIndex > 0)
                        {
                            mPageIndex = 0;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "PrePage":
                        if (mPageIndex > 0)
                        {
                            mPageIndex--;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "NextPage":
                        if (mPageIndex < mPageCount - 1)
                        {
                            mPageIndex++;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "LastPage":
                        if (mPageIndex < mPageCount - 1)
                        {
                            mPageIndex = mPageCount - 1;
                            FillListView();
                            SetPageState();
                        }
                        break;
                }
            }
        }

        private void FillListView()
        {
            try
            {
                mCurentPMDatas.Clear();
                int intStart = mPageIndex * mPageSize;
                int intEnd = (mPageIndex + 1) * mPageSize;
                for (int i = intStart; i < intEnd && i < mRecordTotal; i++)
                {
                    mCurentPMDatas.Add(mAllListPMDatas[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetPageState()
        {
            try
            {
                int pageCount = mRecordTotal / mPageSize;
                int mod = mRecordTotal % mPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                mPageCount = pageCount;
                string temp = CurrentApp.GetLanguageInfo("3103T00097", "Records");
                string strPageInfo = string.Format("{0}/{1} {2} {3}", mPageIndex + 1, mPageCount, mRecordTotal, temp);
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtPageInfo.Text = strPageInfo;
                    TxtPage.Text = (mPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (mPageCount > 0)
                    {
                        int pageIndex;
                        if (!int.TryParse(TxtPage.Text, out pageIndex))
                        {
                            TxtPage.Text = (mPageIndex + 1).ToString();
                            return;
                        }
                        pageIndex--;
                        if (pageIndex < 0)
                        {
                            pageIndex = 0;
                        }
                        if (pageIndex > mPageCount - 1)
                        {
                            pageIndex = mPageCount - 1;
                        }
                        mPageIndex = pageIndex;
                        FillListView();
                        SetPageState();
                    }
                    else
                        TxtPage.Text = "0";
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region 柱状图
        public void CreateChartColumn1(List<DateTime> lsTime, List<UERAListInfo> lstUser, long CycleType,List<PMShowDataItems>lstPMData)
        {
            //创建一个图标
            Chart chart = new Chart();
            chart.Width = 700;
            chart.Height = 450;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示
            chart.ShadowEnabled = true;
            chart.AnimatedUpdate = true;
            //创建一个标题的对象
            Title title = new Title();

            var temp = cbKeyPort.SelectedItem as ComboBoxItem;
            if (temp == null) { return; }
            OperationInfo opt = temp.DataContext as OperationInfo;
            //设置标题的名称
            title.Text = string.Format("{0} {1}", opt.Description, CurrentApp.GetLanguageInfo("4601P00011", "Column Chart"));
            title.Padding = new Thickness(0, 10, 5, 0);
            //向图标添加标题
            chart.Titles.Add(title);

            foreach (UERAListInfo user in lstUser)
            {
                if (lstPMData.Where(p => p.UERAId.ToString() == user.ID).ToList().Count == 0)
                    continue;
                // 创建一个新的数据线。               
                DataSeries dataUserSeries = new DataSeries();
                dataUserSeries.RenderAs = RenderAs.Column;//柱状图
                dataUserSeries.LegendText = user.FullName;
                // 设置数据点              
                DataPoint dataPoint;
                for (int i = 0; i < lsTime.Count; i++)
                {
                    double actual = 0;
                    if (CycleType == 1 || CycleType >= 5)
                    {
                        actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID && p.dtLocalTime == lsTime[i]).ToList().First().ActualValue;
                    }
                    else
                    {
                        actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID && p.pmYear == lsTime[i].Year && p.pmMonth == lsTime[i].Month && p.pmDay == lsTime[i].Day).ToList().First().ActualValue;
                    }
                    // 创建一个数据点的实例。
                    dataPoint = new DataPoint();
                    dataPoint.LabelText = user.FullName;
                    // 设置X轴点
                    //dataPoint.AxisXLabel = cbYear.Text + cbCycleTime.Text;//X轴描述
                    dataPoint.ToolTipText = actual + "/" + user.FullName;
                    switch (CycleType)
                    {
                        case 1:
                            dataPoint.AxisXLabel = string.Format("{0}{1}", cbYear.Text, CurrentApp.GetLanguageInfo("4601P00014", "Year"));
                            //dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                            break;
                        case 2:
                        case 3:
                            dataPoint.AxisXLabel = string.Format("{0}{1}{2}", cbYear.Text, CurrentApp.GetLanguageInfo("4601P00014", "Year"), cbCycleTime.Text);
                            //dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                            break;
                        case 4:
                            dataPoint.AxisXLabel = string.Format("{0}{1}{2}", cbYear.Text, CurrentApp.GetLanguageInfo("4601P00014", "Year"), DateStart.Value);
                            //dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd");
                            break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM-dd HH:mm:ss");
                            //dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd HH:mm:ss");
                            break;
                    }
                    dataPoint.YValue = actual;
                    dataPoint.Name = user.ID;
                    dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                    dataUserSeries.DataPoints.Add(dataPoint);
                }

                chart.Series.Add(dataUserSeries);
            }

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            ScrollViewer sv = new ScrollViewer();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Content = gr;
            PmShowGrid.Children.Add(sv);
        }
        #endregion

        #region 折线图
        public void CreateChartSpline1(List<DateTime> lsTime, List<UERAListInfo> lstUser, long CycleType, List<PMShowDataItems> lstPMData)
        {
            //创建一个图标
            Chart chart = new Chart();
            //是否动态绘制图表
            chart.AnimationEnabled = true;
            chart.Width = 700;
            chart.Height = 450;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示
            //创建一个标题的对象
            Title title = new Title();

            var temp = cbKeyPort.SelectedItem as ComboBoxItem;
            if (temp == null) { return; }
            OperationInfo opt = temp.DataContext as OperationInfo;
            //设置标题的名称
            title.Text = string.Format("{0} {1}", opt.Description, CurrentApp.GetLanguageInfo("4601P00012", "Line Chart"));
            title.Padding = new Thickness(0, 10, 5, 0);
            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
            switch (CycleType)
            {
                case 1:
                    xaxis.IntervalType = IntervalTypes.Years;
                    break;
                case 2:
                    xaxis.IntervalType = IntervalTypes.Months;
                    break;
                case 3:
                    xaxis.IntervalType = IntervalTypes.Weeks;
                    break;
                case 4:
                    xaxis.IntervalType = IntervalTypes.Days;
                    break;
                case 5:
                    xaxis.IntervalType = IntervalTypes.Hours;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    xaxis.IntervalType = IntervalTypes.Minutes;
                    break;
            }
            xaxis.Interval = 1;
            //设置X轴的时间显示格式为7-10 11：20     
            switch (CycleType)
            {
                case 1:
                    xaxis.ValueFormatString = "yyyy";
                    break;
                case 2:
                    xaxis.ValueFormatString = "yyyy-MM";
                    break;
                case 3:
                case 4:
                    xaxis.ValueFormatString = "MM-dd";
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    xaxis.ValueFormatString = "yyyy-MM-dd HH:mm:ss";
                    break;
            }
            //给图标添加Axis            
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "s";
            chart.AxesY.Add(yAxis);

            foreach (UERAListInfo user in lstUser)
            {
                if (lstPMData.Where(p => p.UERAId.ToString() == user.ID).ToList().Count == 0)
                    continue;
                // 创建一个新的数据线。               
                DataSeries dataUserSeries = new DataSeries();
                dataUserSeries.LegendText = user.FullName;
                dataUserSeries.RenderAs = RenderAs.Spline;//折线图
                dataUserSeries.XValueType = ChartValueTypes.DateTime;
                // 设置数据点              
                DataPoint dataPoint;
                for (int i = 0; i < lsTime.Count; i++)
                {
                    double actual = 0;
                    if (CycleType == 1 || CycleType >= 5)
                        actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID
                        && p.dtLocalTime == lsTime[i]).ToList().First().ActualValue;
                    else
                        actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID
                        && p.pmYear == lsTime[i].Year && p.pmMonth == lsTime[i].Month && p.pmDay == lsTime[i].Day).ToList().First().ActualValue;
                    // 创建一个数据点的实例。                   
                    dataPoint = new DataPoint();
                    // 设置X轴点                    
                    dataPoint.XValue = lsTime[i];
                    //设置Y轴点                   
                    dataPoint.YValue = actual;
                    dataPoint.MarkerSize = 8;

                    dataPoint.LabelText = user.FullName;
                    switch (CycleType)
                    {
                        case 1:
                            dataPoint.AxisXLabel = lsTime[i].ToString("yyyy");
                            dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                            break;
                        case 2:
                            dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM");
                            dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                            break;
                        case 3:
                        case 4:
                            dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM-dd");
                            dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd");
                            break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM-dd HH:mm:ss");
                            dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd HH:mm:ss");
                            break;
                    }
                    //dataPoint.Tag = tableName.Split('(')[0];
                    //设置数据点颜色                  
                    // dataPoint.Color = new SolidColorBrush(Colors.LightGray);                   
                    dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                    //添加数据点                   
                    dataUserSeries.DataPoints.Add(dataPoint);
                }

                chart.Series.Add(dataUserSeries);
            }

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            ScrollViewer sv = new ScrollViewer();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Content = gr;
            PmShowGrid.Children.Add(sv);
        }
        #endregion

        #region 点状图
        public void CreateChartBubble1(List<DateTime> lsTime, List<UERAListInfo> lstUser, long CycleType, List<PMShowDataItems> lstPMData)
        {
            //创建一个图标
            Chart chart = new Chart();
            //是否动态绘制图表
            chart.AnimationEnabled = true;
            chart.Width = 700;
            chart.Height = 450;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示
            //创建一个标题的对象
            Title title = new Title();

            var temp = cbKeyPort.SelectedItem as ComboBoxItem;
            if (temp == null) { return; }
            OperationInfo opt = temp.DataContext as OperationInfo;
            //设置标题的名称
            title.Text = string.Format("{0} {1}", opt.Description, CurrentApp.GetLanguageInfo("4601P00049", "Point Chart"));
            title.Padding = new Thickness(0, 10, 5, 0);
            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
            xaxis.Interval = 1;
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            //yAxis.Suffix = "s";
            chart.AxesY.Add(yAxis);
            DataSeries userSeris = new DataSeries();
            userSeris.RenderAs = RenderAs.Bubble;
            foreach (UERAListInfo user in lstUser)
            {
                if (lstPMData.Where(p => p.UERAId.ToString() == user.ID).ToList().Count == 0)
                    continue;
                DataPoint userPoint = new DataPoint();
                userPoint.AxisXLabel = user.FullName;
                userPoint.Name = user.ID;
                userPoint.MarkerSize =3.5;
                userPoint.YValue = lstPMData.Where(p => p.UERAId.ToString() == user.ID && p.dtLocalTime == lsTime[0]).ToList().First().ActualValue;
                userPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                userSeris.DataPoints.Add(userPoint);
            }
            chart.Series.Add(userSeris);
            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            ScrollViewer sv = new ScrollViewer();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Content = gr;
            PmShowGrid.Children.Add(sv);
        }

        #endregion

        #region 饼状图
        public void CreateChartPie1(List<DateTime> lsTime, UERAListInfo user, long CycleType, List<PMShowDataItems> lstPMData, int gridFloor)
        {
            Chart chart = new Chart();
            chart.Width = 600;
            chart.Height = 450;
            chart.Margin = new Thickness(100, 5, 10, 5);
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = true;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            Title title = new Title();

            var tempopt = cbKeyPort.SelectedItem as ComboBoxItem;
            if (tempopt == null) { return; }
            OperationInfo opt = tempopt.DataContext as OperationInfo;
            title.Text = string.Format("{0} {1}", opt.Description, CurrentApp.GetLanguageInfo("4601P00013", "Pie Chart"));
            title.Padding = new Thickness(0, 10, 5, 0);
            chart.Titles.Add(title);
            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            dataSeries.LegendText = user.FullName;
            dataSeries.RenderAs = RenderAs.Pie;//柱状Stacked
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < lsTime.Count; i++)
            {
                double actual = 0;
                if (CycleType == 1 || CycleType >= 5)
                    actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID
                    && p.dtLocalTime == lsTime[i]).ToList().First().ActualValue;
                else
                    actual = lstPMData.Where(p => p.UERAId.ToString() == user.ID
                    && p.pmYear == lsTime[i].Year && p.pmMonth == lsTime[i].Month && p.pmDay == lsTime[i].Day).ToList().First().ActualValue;
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                dataPoint.LabelText = user.FullName;
                // 设置X轴点
                switch (CycleType)
                {
                    case 1:
                        dataPoint.AxisXLabel = lsTime[i].ToString("yyyy");
                        dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                        break;
                    case 2:
                        dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM");
                        dataPoint.ToolTipText = actual + "/" + dataPoint.AxisXLabel;
                        break;
                    case 3:
                    case 4:
                        dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM-dd");
                        dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd");
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        dataPoint.AxisXLabel = lsTime[i].ToString("yyyy-MM-dd HH:mm:ss");
                        dataPoint.ToolTipText = actual + "/" + lsTime[i].ToString("MM-dd HH:mm:ss");
                        break;
                }
                //设置Y轴点                   
                dataPoint.YValue = actual;
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            chart.Series.Add(dataSeries);

            Grid scrollGrid = new Grid();
            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid. 
            Grid gr = new Grid();
            int temp = gridFloor % 2;
            gr.Children.Add(chart);

            scrollGrid.RowDefinitions.Add(new RowDefinition());
            scrollGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) });
            gr.SetValue(Grid.RowProperty, gridFloor / 2);
            gr.SetValue(Grid.ColumnProperty, temp);

            ScrollViewer sv = new ScrollViewer();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Content = scrollGrid;
            PmShowGrid.Children.Add(sv);
        }
        #endregion
        
        private void pmQuerybtn_Click(object sender, RoutedEventArgs e)
        {
            InitTableColumns();
            bool flag = false;
            IsContinue = true;
            ParentPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                flag = GetPMDataOperation();
                mWorker.Dispose();
                ParentPage.SetBusy(false, string.Empty);
            };
            mWorker.RunWorkerAsync();            
        }
        
        void cbChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PmShowGrid.Children.Clear();
            PmShowGrid.RowDefinitions.Clear();
            PmShowGrid.ColumnDefinitions.Clear();
            if (cbChartType.SelectedIndex == 0)//列表 模式
            {
                //turnBorder.Visibility = Visibility.Visible;
                mKPICycle = string.Empty;
                IsUseCheck = true;
                CreateCharTable();
                cbCycleType.SelectedItem=null;
                cbKeyPort.Visibility = Visibility.Collapsed;
                cbKeyPort_Table.Visibility = Visibility.Visible;
                ckItem_Checked(sender,  e);
            }
            else// 图表  模式
            {
                //turnBorder.Visibility = Visibility.Collapsed;
                if (IsUseCheck.Equals(true)) 
                {
                    IsUseCheck = false;
                }
                InitCycleCombobox();
                cbCycleType.SelectedIndex = 0;
                cbKeyPort_Table.Visibility = Visibility.Collapsed;
                cbKeyPort.Visibility = Visibility.Visible;
            }
        }

        void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataPoint dp = sender as DataPoint;
            try
            {
                if (rabORG.IsChecked.Equals(true))
                {
                    List<UERAListInfo> mtempObjects = ListAllAERInfo.Where(p => p.OrgParentID == dp.Name||p.OrgID==dp.Name).ToList();
                    if (mtempObjects.Count() <= 0) { return; }
                    switch (mChartItem.ChartType)
                    {
                        case 1:// 柱状图
                            CreateChartColumn1(mChartItem.ListDateTime, mtempObjects, mChartItem.Type, mChartItem.DataList);
                            break;
                        case 2:// 点状图
                            //CreateChartSpline1(tempTime, checkedUERA, tCycleOpt.ID, tempPmData);折线图
                            CreateChartBubble1(mChartItem.ListDateTime, mtempObjects, mChartItem.Type, mChartItem.DataList);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        

        #region Others

        /// <summary>
        /// 存储查询栏中设置的条件
        /// </summary>
        public class TempPMQueryCondition
        {
            /// <summary>
            /// 查询条件中选中的UERA
            /// </summary>
            public string UERAID { get; set; }
            /// <summary>
            /// KPI ID
            /// </summary>
            public long KPIID { get; set; }
            /// <summary>
            /// 周期类型
            /// </summary>
            public long CycleType { get; set; }

            public long startTime { get; set; }

            public long endTime { get; set; }
        }

        public class TempChartQueryItem
        {
            public List<DateTime> ListDateTime { get; set; }
            public long Type { get; set; }
            public List<PMShowDataItems> DataList { get; set; }
            public int ChartType { get; set; }
        }

        public string ReturnFullName(long UerID)
        {
            string fullName=string.Empty;
            try
            {
                UERAListInfo tempInfo=ListAllAERInfo.Where(p => p.ID == UerID.ToString()).FirstOrDefault();
                if(tempInfo==null){fullName=UerID.ToString();}
                fullName=tempInfo.FullName;
            }
            catch (Exception ex)
            {
                return UerID.ToString();
            }
            return fullName;
        }


        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                PMExpanderBasic.Header = CurrentApp.GetLanguageInfo("4601P00003", "Query");
                tabOrg.Header = rabORG.Content = CurrentApp.GetLanguageInfo("4601P00001", "Org");
                tabSkill.Header = rabSkill.Content = CurrentApp.GetLanguageInfo("4601P00002", "Skill");
                rabSUERA.Content = rabOUERA.Content = CurrentApp.GetLanguageInfo("4601P00029", "Objects");

                lbKeyPort.Content = CurrentApp.GetLanguageInfo("4601P00005", "Key Performance Indicator");
                lbChartType.Content = CurrentApp.GetLanguageInfo("4601P00004", "Chart");
                lbCycleType.Content = CurrentApp.GetLanguageInfo("4601P00008", "Cycle");
                //lbDateStart.Content = CurrentApp.GetLanguageInfo("4601P00006", "StartTime");
                //lbDateEnd.Content = CurrentApp.GetLanguageInfo("4601P00007", "EndTime");
                pmQuerybtn.Content = CurrentApp.GetLanguageInfo("4601P00003", "Query");

                chartTable.Content = CurrentApp.GetLanguageInfo("4601P00010", "Table");
                chartBar.Content = CurrentApp.GetLanguageInfo("4601P00011", "Bar Chart");
                chartLine.Content = CurrentApp.GetLanguageInfo("4601P00049", "point Chart");
                //chartLine.Content = CurrentApp.GetLanguageInfo("4601P00012", "Line Chart");
                //chartPie.Content = CurrentApp.GetLanguageInfo("4601P00013", "Pie Chart");
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("ChangeLang", string.Format("ChangeLang fail.\t{0}", ex.Message));
            }
        }


        private int IntConvert(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            else
                return Convert.ToInt32(obj);
        }
        private long LongConvert(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            else
                return Convert.ToInt64(obj);
        }
        private double DoubleConvert(object obj)
        {
            if (obj == null || obj.ToString() == "")
                return 0;
            else
                return Convert.ToDouble(obj);
        }

        #endregion
        
    }
}
