using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS4601.Models;
using UMPS4601.Wcf11012;
using UMPS4601.Wcf46011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common4601;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4601
{
    /// <summary>
    /// BandingPage.xaml 的交互逻辑
    /// </summary>
    public partial class BandingPage
    {
        public UC_PMParameterBindingPage PageParent;
        public ObjectItem CurrentSelectObjectItem;

        //这个变量只有在前面点击查看内容的时候才能看到
        public KpiMapObjectInfoItem CurrentKpiMapObjectInfoItem;
        private BackgroundWorker mWorker;
        private KpiInfo mCurrentSelectKpiInfo;
        private ComboxObjectType mCurrentSelectApplyOjectType;
        private ComboxObjectType mCurrentSelectApplyCycle;
        private ComboxObjectType mCurrentSelectOperationSymbol;
        private ObservableCollection<KpiInfo> mListKpiInfo;
        private ObservableCollection<ComboxObjectType> mListOperationSymbols;
        private ObservableCollection<ComboxObjectType> mListApplyCycle;
        //对象是否绑定了当前选中的KPI的标志
        private string mFlagIsBanding;//0表示此对象还未绑定选中的kpi，如果是一个流水号的话表示已绑定了选中的Kpi

        private ObservableCollection<ComboxObjectType> mListUseObjectType;

        //这个里面存的是界面上的输入的值
        private KpiMapObjectInfo mKpiMapObjectInfo;

        //这个里面存的是根据所选的对象以及所选的KPI，获得数据库里已存的值
        private KpiMapObjectInfo mKpiMapObjectInfoFromDB;

        private List<KpiMapObjectInfo> mListKpiMapObjectInfo;

        /// <summary>
        /// 进入这个页面的方式  变量为"1",是点击查看内容(修改内容)的按钮进来的
        /// </summary>
        public string IntoThisPageWay;

        public BandingPage()
        {
            InitializeComponent();

            mCurrentSelectApplyOjectType = new ComboxObjectType();
            mKpiMapObjectInfo = new KpiMapObjectInfo();
            mKpiMapObjectInfoFromDB = new KpiMapObjectInfo();
            mListKpiInfo = new ObservableCollection<KpiInfo>();
            mListUseObjectType = new ObservableCollection<ComboxObjectType>();
            mListOperationSymbols = new ObservableCollection<ComboxObjectType>();
            mListApplyCycle = new ObservableCollection<ComboxObjectType>();
            mListKpiMapObjectInfo = new List<KpiMapObjectInfo>();

            Loaded += BandingPage_Loaded;
            KpiCombox.SelectionChanged += KpiCombox_SelectionChanged;
            ApplyCycleCombox.SelectionChanged += ApplyCycleCombox_SelectionChanged;
            BtnSave.Click += BtnSave_Click;
            BtnCancel.Click += BtnCancel_Click;      
        }

        private void BandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            KpiCombox.ItemsSource = mListKpiInfo;
            ApplyCycleCombox.ItemsSource = mListApplyCycle;
            GoalOperation1.ItemsSource = mListOperationSymbols;
            LoadUIContent();
            LoadKPIList();
            ChangeLanguage();

            //如果是点击查看内容进来的 就要执行这里
            if (IntoThisPageWay == "1")
            {
                SetValue_Modify();
            }
        }

        //这个方法是在外面点了查看内容之后执行的
        private void SetValue_Modify()
        {
            if (CurrentKpiMapObjectInfoItem != null)
            {
                KpiMapObjectInfo temp = new KpiMapObjectInfo();
                temp = CurrentKpiMapObjectInfoItem.KpiMapObjectInfo;
                ObjectName.Content = CurrentKpiMapObjectInfoItem.ObjectItem.Name;

                KpiInfo kpiInfoTemp = new KpiInfo();
                //kpi名称
                for (int i = 0; i < mListKpiInfo.Count; i++)
                {
                    if (temp.KpiID == mListKpiInfo[i].KpiID)
                    {
                        KpiCombox.SelectedItem = mListKpiInfo[i];
                        kpiInfoTemp = mListKpiInfo[i];
                    }
                }
                BorderBandingContent.IsEnabled = true;
                KpiCombox.IsEnabled = false;
                ApplyCycleCombox.IsEnabled = false;
                ApplyObjectStackPanel.IsEnabled = false;
                StaticWayStackPanel.IsEnabled = false;

                //应用周期
                for (int i = 0; i < mListApplyCycle.Count; i++)
                {
                    if (temp.ApplyCycle == mListApplyCycle[i].Code)
                    {
                        ApplyCycleCombox.SelectedItem = mListApplyCycle[i];
                    }
                }
                //应用对象
                int count = temp.ApplyType.Replace("*", "").Replace("1", "*").Split('*').Length - 1;//Banding的对象的个数[有几个1就是代表绑定了几个对象]
                if (count >= 2)
                {
                    if (CurrentKpiMapObjectInfoItem.ObjectItem.ParantID.IndexOf("101") == 0)//为机构
                    {                        
                        if (PageParent.GroupingWay.IndexOf("A") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(0, 1) == "1")
                        {
                            CB_Agent.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("E") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(1, 1) == "1")
                        {
                            CB_Extension.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("R") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(3, 1) == "1")
                        {
                            CB_RealExtension.IsEnabled = true;
                        }
                        if (mCurrentSelectKpiInfo.UseType.Substring(2, 1) == "1")
                        {
                            CB_User.IsEnabled =true;
                        }

                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(0, 1) == "1")
                        {
                            CB_Agent.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(1, 1) == "1")
                        {
                            CB_Extension.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(3, 1) == "1")
                        {
                            CB_RealExtension.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(2, 1) == "1")
                        {
                            CB_User.IsChecked = true;
                        }
                        IsUsedAllOrg.IsChecked = temp.ApplyAll == "1" ? true : false;
                        IsUsedAllOrg.IsEnabled = false;
                    }
                    else //技能组
                    {
                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked =  false;
                        IsUsedAllOrg.IsEnabled = false;
                        if (PageParent.GroupingWay.IndexOf("A") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(0, 1) == "1")
                        {
                            CB_Agent.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("E") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(1, 1) == "1")
                        {
                            CB_Extension.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("R") >= 0 && mCurrentSelectKpiInfo.UseType.Substring(3, 1) == "1")
                        {
                            CB_RealExtension.IsEnabled = true;
                        }
                        if (mCurrentSelectKpiInfo.UseType.Substring(2, 1) == "1")
                        {
                            CB_User.IsEnabled = true;
                        }

                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(0, 1) == "1")
                        {
                            CB_Agent.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(1, 1) == "1")
                        {
                            CB_Extension.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(3, 1) == "1")
                        {
                            CB_RealExtension.IsChecked = true;
                        }
                        if (CurrentKpiMapObjectInfoItem.ObjectType.Substring(2, 1) == "1")
                        {
                            CB_User.IsChecked = true;
                        }
                    }
                }
                else
                {                   
                    //分机座席或者
                    switch (CurrentSelectObjectItem.ObjType)
                    {
                        case ConstValue.RESOURCE_ORG:
                            {
                            }
                            break;
                        case ConstValue.RESOURCE_REALEXT:
                            {
                                CB_RealExtension.IsChecked = true;
                                IsUsedAllOrg.Visibility = Visibility.Collapsed;
                                IsUsedAllOrg.IsChecked = false;
                            }
                            break;
                        case ConstValue.RESOURCE_EXTENSION:
                            {
                                CB_Extension.IsChecked = true;

                                IsUsedAllOrg.Visibility = Visibility.Collapsed;
                                IsUsedAllOrg.IsChecked = false;
                            }
                            break;
                        case ConstValue.RESOURCE_AGENT:
                            {
                                CB_Agent.IsChecked = true;

                                IsUsedAllOrg.Visibility = Visibility.Collapsed;
                                IsUsedAllOrg.IsChecked = false;
                            }
                            break;
                        case ConstValue.RESOURCE_USER:
                            {
                                CB_User.IsChecked = true;

                                IsUsedAllOrg.Visibility = Visibility.Collapsed;
                                IsUsedAllOrg.IsChecked = false;
                            }
                            break;
                        case ConstValue.RESOURCE_TECHGROUP:
                            {
                            }
                            break;
                    }
                }
                //是否启用
                IsUsedCheckBox.IsChecked = temp.IsActive == "1" ? true : false;
                

                //目标以及比较符赋值
                string StrApplyGoalOperator1 = kpiInfoTemp.GoalOperator1;
                mListOperationSymbols.Clear();
                string[] StrValues = StrApplyGoalOperator1.Split('|');
                for (int i = 0; i < StrValues.Length; i++)
                {
                    if (!string.IsNullOrEmpty(StrValues[i]))
                    {
                        ComboxObjectType item = new ComboxObjectType();
                        item.Name = StrValues[i].ToString();
                        mListOperationSymbols.Add(item);
                    }
                }
                var tempItem = mListOperationSymbols.FirstOrDefault(o => o.Name == temp.GoalOperation1) as ComboxObjectType;
                if (tempItem != null)
                {
                    GoalOperation1.SelectedItem = tempItem;
                }

                
                //目标的值
                Goal1Value.Text = temp.GoldValue1;
                Goal2Value.Text = temp.GoldValue2;
                if (CurrentKpiMapObjectInfoItem.ObjectItem.ObjType == ConstValue.RESOURCE_ORG)
                {
                    StaticWayStackPanel.IsEnabled = false;
                    //统计方式
                    if (temp.DropDown == "1")
                    {
                        RBParallel.IsChecked = true;
                    }
                    if (temp.DropDown == "2")
                    {
                        RBDropDown.IsChecked = true;
                    }

                }
                else
                {
                    StaticWayStackPanel.IsEnabled = false;
                }

            }
        }

        /// <summary>
        /// 加载界面上的内容
        /// </summary>
        private void LoadUIContent()
        {
            ObjectName.Content = CurrentSelectObjectItem.Name;
            StylePath = "UMPS4601/BandingPage.xaml";    
            Image aaa = new Image();
            switch (CurrentSelectObjectItem.ObjType)
            {
                case ConstValue.RESOURCE_ORG:
                    {
                        aaa.SetResourceReference(StylePathProperty, "org");
                        IsUsedAllOrg.IsChecked = true;
                    }
                    break;
                case ConstValue.RESOURCE_REALEXT:
                    {
                        aaa.SetResourceReference(StylePathProperty, "RealExtension");
                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked = false;
                    }
                    break;
                case ConstValue.RESOURCE_EXTENSION:
                    {
                        aaa.SetResourceReference(StylePathProperty, "extension");

                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked = false;
                    }
                    break;
                case ConstValue.RESOURCE_AGENT:
                    {
                        aaa.SetResourceReference(StylePathProperty, "agent");

                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked = false;
                    }
                    break;
                case ConstValue.RESOURCE_USER:
                    {
                        aaa.SetResourceReference(StylePathProperty, "user");

                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked = false;
                    }
                    break;
                case ConstValue.RESOURCE_TECHGROUP:
                    {
                        aaa.SetResourceReference(StylePathProperty, "SkillGroup");

                        IsUsedAllOrg.Visibility = Visibility.Collapsed;
                        IsUsedAllOrg.IsChecked = false;
                    }
                    break;
            }
            ObjectType = aaa;
        }

        //根据传入的对象的类型加载相对应的KPI 放入最上面的Combox里面
        private void LoadKPIList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.GetKPIInfoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentSelectObjectItem.ObjType.ToString());
                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<KpiInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    KpiInfo kpiInfo = optReturn.Data as KpiInfo;
                    kpiInfo.Description = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", kpiInfo.KpiID), kpiInfo.Description);
                    if (kpiInfo != null)
                    {
                        mListKpiInfo.Add(kpiInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        // 选择kpi时 周期加载 放入combox里
        private void InitApplyCycle()
        {
            try
            {
                #region 加载可以应用的周期
                string itemApplyCycle = mCurrentSelectKpiInfo.ApplyCycle;
                ComboxObjectType itemCycle;
                mListApplyCycle.Clear();
                if (itemApplyCycle.Substring(0, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP1000000001", "Year");
                    itemCycle.Code = "1000000001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(1, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0100000001", "Month");
                    itemCycle.Code = "0100000001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(2, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0010000001", "Week");
                    itemCycle.Code = "0010000001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(3, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0001000001", "Day");
                    itemCycle.Code = "0001000001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(4, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0000100001", "1Hour");
                    itemCycle.Code = "0000100001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(5, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0000010001", "30Min");
                    itemCycle.Code = "0000010001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(6, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0000001001", "15Min");
                    itemCycle.Code = "0000001001";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(7, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0000000101", "10Min");
                    itemCycle.Code = "0000000101";
                    mListApplyCycle.Add(itemCycle);
                }
                if (itemApplyCycle.Substring(8, 1) == "1")
                {
                    itemCycle = new ComboxObjectType();
                    itemCycle.Name = CurrentApp.GetLanguageInfo("4601BP0000000011", "5Min");
                    itemCycle.Code = "0000000011";
                    mListApplyCycle.Add(itemCycle);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        //选择kpi时加载
        private void InitBindingObject(KpiInfo AKpinInfo)
        {
            string StrApplyObject = AKpinInfo.UseType;
            string StrApplyCycle = AKpinInfo.ApplyCycle;
            CB_Agent.IsEnabled = false;
            CB_Extension.IsEnabled = false;
            CB_RealExtension.IsEnabled = false;
            CB_User.IsEnabled = false;
            CB_Agent.IsChecked = false;
            CB_Extension.IsChecked = false;
            CB_RealExtension.IsChecked = false;
            CB_User.IsChecked = false;

            //1、传入的对象判定是机构还是座席还是用户
            //2、判断该kpi能添加到那些对象上
            switch (CurrentSelectObjectItem.ObjType)
            {
                case ConstValue.RESOURCE_ORG:
                    {
                        if (PageParent.GroupingWay.IndexOf("A") >= 0 && StrApplyObject.Substring(0, 1).Equals("1"))
                        {
                            CB_Agent.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("E") >= 0 && StrApplyObject.Substring(1, 1).Equals("1"))
                        {
                            CB_Extension.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("R") >= 0 && StrApplyObject.Substring(3, 1).Equals("1"))
                        {
                            CB_RealExtension.IsEnabled = true;
                        }
                        if (StrApplyObject.Substring(2, 1).Equals("1"))
                        {
                            CB_User.IsEnabled = true;
                        }
                        StaticWayStackPanel.IsEnabled = true;
                    }
                    break;
                case ConstValue.RESOURCE_REALEXT:
                    {
                        if (PageParent.GroupingWay.IndexOf("R") >= 0 && StrApplyObject.Substring(3, 1).Equals("1"))
                        {
                            CB_RealExtension.IsEnabled = true;
                        }
                    }
                    break;
                case ConstValue.RESOURCE_EXTENSION:
                    {
                        if (PageParent.GroupingWay.IndexOf("E") >= 0 && StrApplyObject.Substring(1, 1).Equals("1"))
                        {
                            CB_Extension.IsEnabled = true;
                        }
                    }
                    break;
                case ConstValue.RESOURCE_AGENT:
                    {
                        if (PageParent.GroupingWay.IndexOf("A") >= 0 && StrApplyObject.Substring(0, 1).Equals("1"))
                        {
                            CB_Agent.IsEnabled = true;
                        }
                    }
                    break;
                case ConstValue.RESOURCE_USER:
                    {
                        if (StrApplyObject.Substring(2, 1).Equals("1"))
                        {
                            CB_User.IsEnabled = true;
                        }
                    }
                    break;
                case ConstValue.RESOURCE_TECHGROUP:
                    {
                        if (PageParent.GroupingWay.IndexOf("A") >= 0 && StrApplyObject.Substring(0, 1).Equals("1"))
                        {
                            CB_Agent.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("E") >= 0 && StrApplyObject.Substring(1, 1).Equals("1"))
                        {
                            CB_Extension.IsEnabled = true;
                        }
                        if (PageParent.GroupingWay.IndexOf("R") >= 0 && StrApplyObject.Substring(3, 1).Equals("1"))
                        {
                            CB_RealExtension.IsEnabled = true;
                        }
                        if (StrApplyObject.Substring(2, 1).Equals("1"))
                        {
                            CB_User.IsEnabled = true;
                        }
                    }
                    break;
            }

            //目标以及比较符赋值
            string StrApplyGoalOperator1 = AKpinInfo.GoalOperator1;
            mListOperationSymbols.Clear();
            string[] StrValues = StrApplyGoalOperator1.Split('|');
            for (int i = 0; i < StrValues.Length; i++)
            {
                if (!string.IsNullOrEmpty(StrValues[i]))
                {
                    ComboxObjectType item = new ComboxObjectType();
                    item.Name = StrValues[i].ToString();
                    mListOperationSymbols.Add(item);
                }
            }

            var tempItem = mListOperationSymbols.FirstOrDefault(o => o.Name == AKpinInfo.DefaultSymbol) as ComboxObjectType;
            if (tempItem != null)
            {
                GoalOperation1.SelectedItem = tempItem;
            }
            GoalOperation1.IsEnabled = true;
            Goal1Value.Text = mCurrentSelectKpiInfo.GoalValue1;
            Goal2Value.Text = mCurrentSelectKpiInfo.GoalValue2;
            
        }

        #region 一些事件
        //主要的kpi选择时
        private void KpiCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var kpiInfo = KpiCombox.SelectedItem as KpiInfo;
            if (kpiInfo != null)
            {
                ApplyCycleCombox.IsEnabled = true;
                mCurrentSelectKpiInfo = kpiInfo;
                //启用梆周期
                InitApplyCycle();
                InitBindingObject(kpiInfo);

                //启用梆定对象
                BorderBandingContent.IsEnabled = true;
            }
            else
            {
                BorderBandingContent.IsEnabled = false;
            }
        }

        private void ApplyCycleCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cycleInfo = ApplyCycleCombox.SelectedItem as ComboxObjectType;
            if (cycleInfo != null)
            {
                mCurrentSelectApplyCycle = cycleInfo;
                BorderBandingContent.IsEnabled = true;             
            }
        }

       //操作符
        private void GoalOperation1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var aaa = GoalOperation1.SelectedItem as ComboxObjectType;
            if (aaa != null)
            {
                mCurrentSelectOperationSymbol = aaa;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentSelectApplyCycle==null)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00009", "Please Select A  KPI  OR Apply Cycle"));
                return;
            }

            IsBandingKPi();
             if(mCurrentSelectApplyOjectType.Code == null)
             {
                ShowException(CurrentApp.GetLanguageInfo("4601TIP00002", "Please Select Object"));
                 return;
             }
             if (mCurrentSelectApplyOjectType.Code.Equals("0000000000") || mCurrentSelectApplyOjectType.Code == "0" )
             {
                 ShowException(CurrentApp.GetLanguageInfo("4601TIP00002", "Please Select Object"));
                 return;
             }
             if (!mFlagIsBanding.Equals("0") && IntoThisPageWay == "0")
             {
                 ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00010", " Only Banding Once With One Cycle And Same Statistics Object "));
                 
                 return;
             }

            if (BorderBandingContent.IsEnabled == false)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00009", "Please Select A  KPI  OR Apply Cycle"));
                return;
            }
            try
            {
               

                if (CreateKPIMapObjectInfo() == false)
                {
                    return;
                }
                if (mKpiMapObjectInfo == null)
                {
                    return;
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => OptSave();
                mWorker.RunWorkerCompleted += (s, de) =>
                {
                    ClosePopupPannel();
                    PageParent.LoadKpiMapObjectInfo();
                };
                mWorker.RunWorkerAsync();


            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }


        private void RBParallel_Click_1(object sender, RoutedEventArgs e)
        {
            if (RBParallel.IsChecked == true)
            {
                IsUsedAllOrg.IsEnabled = true;
            }
            else 
            {
                IsUsedAllOrg.IsEnabled = false;
                IsUsedAllOrg.IsChecked = true;
            }
        }

        private void RBDropDown_Click_1(object sender, RoutedEventArgs e)
        {
            if (RBParallel.IsChecked == true)
            {
                IsUsedAllOrg.IsEnabled = true;
            }
            else
            {
                IsUsedAllOrg.IsEnabled = false;
                IsUsedAllOrg.IsChecked = true;
            }
        }


        #endregion

        //根据所传入的对象以及对象的父级来判断这个对象是否绑定了当前选中的KPI
        private void IsBandingKPi()
        {
            int tempObj = 0;
            if ( CB_Agent.IsChecked == true)
            {
                tempObj += 1000000000;
            }
            if ( CB_Extension.IsChecked == true)
            {
                tempObj += 0100000000;
            }
            if (CB_User.IsChecked == true)
            {
                tempObj += 0010000000;
            }
            if ( CB_RealExtension.IsChecked == true)
            {
                tempObj += 0001000000;
            }
            if (CurrentSelectObjectItem.ObjType == ConstValue.RESOURCE_ORG) 
            {
                tempObj += 0000100000;
            }
            else if (CurrentSelectObjectItem.ObjType == ConstValue.RESOURCE_TECHGROUP)
            {
                tempObj += 0000010000;
            }
            
            mCurrentSelectApplyOjectType.Code = tempObj.ToString("0000000000");

            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.IsBandingKpi;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentSelectKpiInfo.KpiID);
                webRequest.ListData.Add(CurrentSelectObjectItem.ObjID.ToString());
                webRequest.ListData.Add(CurrentSelectObjectItem.ParantID);
                webRequest.ListData.Add(mCurrentSelectApplyCycle.Code);


                webRequest.ListData.Add(mCurrentSelectApplyOjectType.Code);

                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string item = webReturn.Data as string;
                if (item == "0")
                {
                    mFlagIsBanding = item;
                }
                else
                {
                    mFlagIsBanding = item;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

 

        //自己界面上设置的值放入到mKpiMapObjectInfo里面
        private bool CreateKPIMapObjectInfo()
        {
            try
            {
                mKpiMapObjectInfo.KpiID = mCurrentSelectKpiInfo.KpiID;
                mKpiMapObjectInfo.ObjectID = CurrentSelectObjectItem.ObjID.ToString();
                mKpiMapObjectInfo.BelongOrgSkg = CurrentSelectObjectItem.ParantID;
                
                //应用对象的类型

                mKpiMapObjectInfo.ApplyType = mCurrentSelectApplyOjectType.Code;

                #region 应用周期
                mKpiMapObjectInfo.ApplyCycle = mCurrentSelectApplyCycle.Code;
                if (mKpiMapObjectInfo.ApplyType == null)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00003", "Please Select Cycle"));
                    return false;
                }
                #endregion

                #region 是否启用 + 启用时间
                //mFlagIsUsed 是否启用
                mKpiMapObjectInfo.IsActive = IsUsedCheckBox.IsChecked == true ? "1" : "0";


                //开始时间暂时服务没有用到
                mKpiMapObjectInfo.StartTime = DateTime.Now.ToString();
                mKpiMapObjectInfo.StopTime = DateTime.Now.ToString();
                #endregion

                #region 统计方式 平行还是向下钻取[]
                 mKpiMapObjectInfo.DropDown = RBParallel.IsChecked == true ? "1" : "2";

                #endregion

                #region 是否应用到所有
                 if (CurrentSelectObjectItem.ObjType == ConstValue.RESOURCE_ORG) //1是应用到子机构 2不应用到子机构
                 {
                     mKpiMapObjectInfo.ApplyAll = IsUsedAllOrg.IsChecked == true ? "1" : "2";
                 }
                 else 
                 {
                     mKpiMapObjectInfo.ApplyAll = "0";
                 }   
                #endregion

                #region 添加人 添加时间
                mKpiMapObjectInfo.AdderID = CurrentApp.Session.UserID.ToString();
                mKpiMapObjectInfo.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                #endregion

                #region 目标1，目标2
                mKpiMapObjectInfo.IsStartGoal1 = "1";
                mKpiMapObjectInfo.GoldValue1 = Goal1Value.Text.ToString();
                mKpiMapObjectInfo.GoalOperation1 = mCurrentSelectOperationSymbol.Name;
                mKpiMapObjectInfo.IsStartMultiRegion1 = "n";

                mKpiMapObjectInfo.IsStartGoal2 = "1";
                mKpiMapObjectInfo.GoldValue2 = Goal2Value.Text.ToString();
                mKpiMapObjectInfo.GoalOperation2 = mCurrentSelectOperationSymbol.Name;
                mKpiMapObjectInfo.IsStartMultiRegion2 = "n";
                double dtINT;
                if (string.IsNullOrWhiteSpace(mKpiMapObjectInfo.GoldValue1) ||
                    string.IsNullOrWhiteSpace(mKpiMapObjectInfo.GoldValue2) ||
                    !double.TryParse(mKpiMapObjectInfo.GoldValue1, out dtINT) ||
                    !double.TryParse(mKpiMapObjectInfo.GoldValue2, out dtINT))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00006", "Please Write Right Value"));
                    return false;
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
                return false;
            }
        }

        //将mKpiMapObjectInfo的内容存入到数据库
        private void SaveToDB()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.SaveKpiMapObjectInfo;
                OperationReturn optReturn = new OperationReturn();
                optReturn = XMLHelper.SeriallizeObject(mKpiMapObjectInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //将mListKpiMapObjectInfo的内容存入数据库
        private void SaveListToDB()
        {
            try
            {
                for (int i = 0; i < mListKpiMapObjectInfo.Count; i++)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S4601Codes.SaveKpiMapObjectInfo;
                    OperationReturn optReturn = new OperationReturn();
                    optReturn = XMLHelper.SeriallizeObject(mListKpiMapObjectInfo[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    Service46011Client client = new Service46011Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OptSave()
        {
            try
            {
                SaveToDB();//不管怎么样都需要先将当前绑定的对象写入到数据库，然后再考虑要不要给其子对象绑定

                //1.应用对象为机构+不应用到所有  2.应用对象为技能组
                if ((mKpiMapObjectInfo.ApplyType == "0000100000" && mKpiMapObjectInfo.ApplyAll == "2") ||
                    mKpiMapObjectInfo.ApplyType == "0000010000")
                {
                    //这是单独储存的

                    return;
                }
                //绑定对象不为机构且不为技能组的时候
                if (mKpiMapObjectInfo.ObjectID.IndexOf("101") != 0 && mKpiMapObjectInfo.ObjectID.IndexOf("906") != 0)
                {
                    //这是单独储存的
                    return;
                }
                //接下来不是单独储存的 也分三种情况（1.技能组+（坐席/"分机/真实分机"） 2.机构+（机构/分机/坐席/真实分机/用户）+应用到所有 3.机构+（分机/坐席/真实分机/用户）+不应用到所有 这里没有机构+应用对象为机构+不应用到所有的情况，这个在上面筛选过了 ）
                mListKpiMapObjectInfo.Clear();

                if (mKpiMapObjectInfo.ObjectID.IndexOf("906") == 0 )
                {
                    if (Regex.Matches(mKpiMapObjectInfo.ApplyType, @"1").Count > 1)
                    {
                        if (mKpiMapObjectInfo.ApplyType.Substring(0, 1) == "1")//坐席
                        {
                            InitControlObjectItemInSkillGroup("103", mKpiMapObjectInfo.ObjectID);
                        }
                        if (mKpiMapObjectInfo.ApplyType.Substring(1, 1) == "1")//分机
                        {
                            InitControlObjectItemInSkillGroup("104", mKpiMapObjectInfo.ObjectID);
                        }
                        if(mKpiMapObjectInfo.ApplyType.Substring(2,1)=="1")//用户
                        {
                            InitControlUserInSkillGroup(mKpiMapObjectInfo.ObjectID);
                        }
                        if (mKpiMapObjectInfo.ApplyType.Substring(3, 1) == "1")//真实分机
                        {
                            InitControlObjectItemInSkillGroup("105", mKpiMapObjectInfo.ObjectID);
                        }
                    }
                    if (mListKpiMapObjectInfo != null)
                    {
                        SaveListToDB();
                    }
                }
                else  if (mKpiMapObjectInfo.ObjectID.IndexOf("101") == 0)
                {
                    if (mKpiMapObjectInfo.ApplyAll == "1")//应用到所有
                    {
                        if (mKpiMapObjectInfo.ApplyType.Substring(0, 1) == "1" )//坐席
                        {
                            InitControlAgents(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                        }
                        if (mKpiMapObjectInfo.ApplyType.Substring(1, 1) == "1")//分机
                        {
                            InitControlExtensions(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                        }
                        if (mKpiMapObjectInfo.ApplyType.Substring(2, 1) == "1")//用户
                        {
                            InitControlUsers(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                        }
                        if (mKpiMapObjectInfo.ApplyType.Substring(3, 1) == "1" )//真实分机
                        {
                            InitControlRealExtensions(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                        }

                        if (Regex.Matches(mKpiMapObjectInfo.ApplyType, @"1").Count > 1)
                        {
                            InitControlOrgs(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                        }
                    }
                    if (mKpiMapObjectInfo.ApplyAll == "2")//不应用到所有
                    {
                        if (Regex.Matches(mKpiMapObjectInfo.ApplyType, @"1").Count > 1)
                        {
                            if (mKpiMapObjectInfo.ApplyType.Substring(0, 1) == "1")//坐席
                            {
                                InitControlAgents(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                            }
                            if (mKpiMapObjectInfo.ApplyType.Substring(1, 1) == "1")//分机
                            {
                                InitControlExtensions(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                            }
                            if (mKpiMapObjectInfo.ApplyType.Substring(2, 1) == "1")//用户
                            {
                                InitControlUsers(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                            }
                            if (mKpiMapObjectInfo.ApplyType.Substring(3, 1) == "1")//真实分机
                            {
                                InitControlRealExtensions(CurrentSelectObjectItem, mKpiMapObjectInfo.ObjectID);
                            }
                        }
                    }
                    if (mListKpiMapObjectInfo != null)
                    {
                        SaveListToDB();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }


        //加载技能组下的管理的用户
        private void InitControlUserInSkillGroup(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlUserInfoListInSkill;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    //用户全名 在界面上显示的
                    item.Name = strFullName;
                    //用户账号  不在界面上显示的
                    item.FullName = strName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}({1})", strFullName, strName);
                    item.ParantID = parentID;


                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ParantID;
                    iItem.ApplyType = "0010000000";
                    iItem.ApplyAll = "0";
                    iItem.DropDown = "0";

                    iItem.StartTime = DateTime.Now.ToString();
                    iItem.StopTime = DateTime.Now.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }



        //加载技能组下的管理对象
        private void InitControlObjectItemInSkillGroup(string BandingObjectType, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlObjectInfoListInSkillGroup;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add(PageParent.GroupingWay);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    KpiMapObjectInfo iItem = new KpiMapObjectInfo();
                    iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    string strID = string.Empty;
                    string strIP = string.Empty;
                    string strName = string.Empty;
                    string strFullName = string.Empty;
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length < 3) { continue; }
                    if (arrInfo.Length == 4)
                    {
                        strID = arrInfo[0];
                        strIP = arrInfo[1];
                        strName = arrInfo[2];
                        strFullName = arrInfo[3];
                        item.ObjID = Convert.ToInt64(strID);
                        item.FullName = strIP;
                        item.Description = strFullName;
                        item.Name = strName;
                        item.ObjType = ConstValue.RESOURCE_EXTENSION;
                        iItem.ApplyType = "0100000000";
                    }
                    else
                    {
                        strID = arrInfo[0];
                        strName = arrInfo[1];
                        strFullName = arrInfo[2];
                        if (strID.IndexOf("103") == 0)
                        {
                            item.ObjType = ConstValue.RESOURCE_AGENT;
                            item.ObjID = Convert.ToInt64(strID);
                            item.Name = strName;
                            item.Description = strFullName;
                            item.Data = strInfo;
                            iItem.ApplyType = "1000000000";
                        }
                        else
                        {
                            item.ObjType = ConstValue.RESOURCE_REALEXT;
                            item.ObjID = Convert.ToInt64(strID);
                            item.Name = strName;
                            item.Description = strFullName;
                            item.Data = strInfo;
                            iItem.ApplyType = "0001000000";
                        }
                    }
                    item.ParantID = parentID;

                    
                    if (item.ObjID.ToString().IndexOf(BandingObjectType) == 0)
                    {
                        iItem.ObjectID = item.ObjID.ToString();
                        iItem.StartTime = DateTime.Now.ToString();
                        iItem.StopTime = DateTime.Now.ToString();
                        mListKpiMapObjectInfo.Add(iItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlOrgs(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    item.ParantID = strID;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        //item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        //item.Icon = "Images/org.ico";
                    }     

                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ObjID.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                    if (mKpiMapObjectInfo.ApplyAll == "1")
                    {
                        InitControlOrgs(item, strID);
                    }

                    if (mKpiMapObjectInfo.ApplyType.Substring(2, 1) == "1")
                    {
                        InitControlUsers(item, strID);
                    }
                    if (mKpiMapObjectInfo.ApplyType.Substring(3, 1) == "1")
                    {
                        InitControlRealExtensions(item, strID);
                    }
                    if (mKpiMapObjectInfo.ApplyType.Substring(1, 1) == "1")
                    {
                        InitControlExtensions(item, strID);
                    }
                    if (mKpiMapObjectInfo.ApplyType.Substring(0, 1) == "1")
                    {
                        InitControlAgents(item, strID);
                    }
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
                webRequest.Code = (int)S4601Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                //mListControlAgent = new List<string>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    item.ParantID = parentID;

                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ParantID;
                    iItem.ApplyAll = "0";
                    iItem.DropDown = "0";
                    iItem.ApplyType = "1000000000";
                    iItem.StartTime = DateTime.Now.ToString();
                    iItem.StopTime = DateTime.Now.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtensions(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strIP = arrInfo[1];
                    string strName = arrInfo[2];
                    string strFullName = string.Empty;
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length == 4)
                    {
                        strFullName = arrInfo[3];
                    }
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(strID);
                    if (strFullName == string.Empty)
                    {
                        strFullName = strName;
                    }
                    item.Name = strName;
                    item.FullName = strIP;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.ParantID = parentID;

                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ParantID;
                    iItem.ApplyAll = "0";
                    iItem.DropDown = "0";
                    iItem.ApplyType = "0100000000";

                    iItem.StartTime = DateTime.Now.ToString();
                    iItem.StopTime = DateTime.Now.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealExtensions(ObjectItem parentItem, string parentID)
        {
            try
            {              
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlRealExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length == 4)
                    {
                        strFullName = arrInfo[3];
                    }
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(strID);
                    if (strFullName == string.Empty)
                    {
                        strFullName = strName;
                    }
                    item.Name = strName;
                    item.FullName = strFullName;
                    item.Description = strName;
                    item.Data = strInfo;
                    item.ParantID = parentID;

                    
                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ParantID;
                    iItem.ApplyType = "0001000000";
                    iItem.ApplyAll = "0";
                    iItem.DropDown = "0";

                    iItem.StartTime = DateTime.Now.ToString();
                    iItem.StopTime = DateTime.Now.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlUsers(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlUserInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    KpiMapObjectInfo iItem = mKpiMapObjectInfo.CloneMumber();
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    //用户全名 在界面上显示的
                    item.Name = strFullName;
                    //用户账号  不在界面上显示的
                    item.FullName = strName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}({1})", strFullName, strName);
                    //item.Icon = "Images/user.ico";
                    item.ParantID = parentID;
                    //mListAllObjects.Add(item);
                    //AddChildObject(parentItem, item);

                   
                    iItem.ObjectID = item.ObjID.ToString();
                    iItem.BelongOrgSkg = item.ParantID;
                    iItem.ApplyType = "0010000000";
                    iItem.ApplyAll = "0";
                    iItem.DropDown = "0";

                    iItem.StartTime = DateTime.Now.ToString();
                    iItem.StopTime = DateTime.Now.ToString();
                    mListKpiMapObjectInfo.Add(iItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ClosePopupPannel()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

 

        #region  ChangeLanguage
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                if (this.IntoThisPageWay.Equals("0"))//添加
                {
                    parent.Title = CurrentApp.GetLanguageInfo("4601BP0002", "Add Banding Page");
                    
                    IsUsedAllOrg.Content = CurrentApp.GetLanguageInfo("4601P00036", "Add Banding To Son  Org");
                }
                else //修改
                {
                    parent.Title = CurrentApp.GetLanguageInfo("4601BP0003", "Modify Banding Page");
                    
                    IsUsedAllOrg.Content = CurrentApp.GetLanguageInfo("4601P00037", "Add Banding To Son  Org ");
                }
            }
            CB_Agent.Content = CurrentApp.GetLanguageInfo("4601BPOBJ1000000000", "Agent");
            CB_Extension.Content = CurrentApp.GetLanguageInfo("4601BPOBJ0100000000", "Extension");
            CB_RealExtension.Content = CurrentApp.GetLanguageInfo("4601BPOBJ0001000000", "RealExtension");
            CB_User.Content = CurrentApp.GetLanguageInfo("4601BPOBJ0010000000", "User");
            BtnSave.Content = CurrentApp.GetLanguageInfo("46010", "Save");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("46011", "Cancel");
            KpiName.Content = CurrentApp.GetLanguageInfo("COL4601001KPIName", "KPIName");
            LableApplyCycle.Content = CurrentApp.GetLanguageInfo("COL4601001ApplyCycle", "ApplyCycle");
            ApplyObjectLable.Content = CurrentApp.GetLanguageInfo("COL4601001ObjectType", "ObjectType");
            IsUsedCheckBox.Content = CurrentApp.GetLanguageInfo("4601BP0001", "IsActive");
            //StartTimeLable.Content = CurrentApp.GetLanguageInfo("COL4601001StartTime", "StartTime");
            //StopTimeLable.Content = CurrentApp.GetLanguageInfo("COL4601001StopTime", "StopTime");
            Goal1Name.Content = CurrentApp.GetLanguageInfo("COL4601001GoldValue1", "Goal1");
            Goal2Name.Content = CurrentApp.GetLanguageInfo("COL4601001GoldValue2", "Goal2");
            DropDown.Content = CurrentApp.GetLanguageInfo("COL4601001DropDown", "DropDown");
            RBParallel.Content = CurrentApp.GetLanguageInfo("4601BPDropDown1", "Parallel");
            RBDropDown.Content = CurrentApp.GetLanguageInfo("4601BPDropDown2", "DropDown");
            //ApplyAll.Content = CurrentApp.GetLanguageInfo("COL4601001ApplyAll", "ApplyAll");
            //RBYes.Content = CurrentApp.GetLanguageInfo("4601BPApplyAll1", "YES");
            //RBNo.Content = CurrentApp.GetLanguageInfo("4601BPApplyAll2", "NO");
        }
        #endregion

    }
}
