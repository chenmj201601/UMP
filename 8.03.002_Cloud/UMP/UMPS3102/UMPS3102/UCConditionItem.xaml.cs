//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    075c8b58-3950-42e2-814c-caed618b8b88
//        CLR Version:              4.0.30319.18444
//        Name:                     UCConditionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCConditionItem
//
//        created by Charley at 2014/11/7 15:12:39
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;

namespace UMPS3102
{
    /// <summary>
    /// UCConditionItem.xaml 的交互逻辑
    /// </summary>
    public partial class UCConditionItem //这个是每个tab条件里面的，条件项
    {

        public UCQueryCondition PageParent;
        public CustomConditionItem ConditionItem;
        public QueryConditionDetail ConditionResult;
        public List<ObjectItem> ListSelectedObjects;

        //ListAllObjects这个是在UCQueryCondition.cs里赋值,这个是管理对象~
        public List<ObjectItem> ListAllObjects;

        private List<QueryConditionSubItem> mListSubItems;//查询条件的分项,将原来的3条录音记录（全部是写在一起的）,然后分开写在里面的value1里面,比如有3个录音流水号的记录在value1里面,那么到这里只会变成3个mListSubItems[i],并且每个里面的value只有1个录音流水号记录
        private ObservableCollection<ConditionItemSubItem> mListConditionItemSubItems;

        //基本时间类型（今天，最近两天，这周，这个月等）
        private ObservableCollection<ConditionItemSubItem> mListTimeTypeBasicItems;
        //其他时间类型（数字+天/周/月/年）
        private ObservableCollection<ConditionItemSubItem> mListTimeTypeFromToItems;

        //评分表
        private ObservableCollection<ConditionItemSubItem> mListScoreSheetItems;

        private ObservableCollection<ConditionItemSubItem> mListIsScoreItems;

        public List<ObjectItem> mListInspectors;

        //ABCD
        private ObservableCollection<ConditionItemSubItem> mListOrgOrSkillGroup;

        private bool mIsInitialed;


        private ObjectItem mRootOrg;
        private List<string> mListOrg;
        private List<string> mListAgent;
        private List<string> mListExtension;


        #region DependencyProperty

        public static readonly DependencyProperty ItemTypeProperty =
           DependencyProperty.Register("ItemType", typeof(CustomConditionItemType), typeof(UCConditionItem), new PropertyMetadata(default(CustomConditionItemType)));

        public CustomConditionItemType ItemType
        {
            get { return (CustomConditionItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(CustomConditionItemFormat), typeof(UCConditionItem), new PropertyMetadata(default(CustomConditionItemFormat)));

        public CustomConditionItemFormat Format
        {
            get { return (CustomConditionItemFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty TxtLikeProperty =
            DependencyProperty.Register("TxtLike", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string TxtLike
        {
            get { return (string)GetValue(TxtLikeProperty); }
            set { SetValue(TxtLikeProperty, value); }
        }

        public static readonly DependencyProperty IsLikeProperty =
            DependencyProperty.Register("IsLike", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));

        public bool IsLike
        {
            get { return (bool)GetValue(IsLikeProperty); }
            set { SetValue(IsLikeProperty, value); }
        }

        public static readonly DependencyProperty Value01Property =
            DependencyProperty.Register("Value01", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string Value01
        {
            get { return (string)GetValue(Value01Property); }
            set { SetValue(Value01Property, value); }
        }

        public static readonly DependencyProperty Value02Property =
            DependencyProperty.Register("Value02", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string Value02
        {
            get { return (string)GetValue(Value02Property); }
            set { SetValue(Value02Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType0Property =
            DependencyProperty.Register("TxtTimeType0", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string TxtTimeType0
        {
            get { return (string)GetValue(TxtTimeType0Property); }
            set { SetValue(TxtTimeType0Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType1Property =
            DependencyProperty.Register("TxtTimeType1", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));


        public string TxtTimeType1
        {
            get { return (string)GetValue(TxtTimeType1Property); }
            set { SetValue(TxtTimeType1Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType2Property =
            DependencyProperty.Register("TxtTimeType2", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string TxtTimeType2
        {
            get { return (string)GetValue(TxtTimeType2Property); }
            set { SetValue(TxtTimeType2Property, value); }
        }

        public static readonly DependencyProperty TxtTimeFromToCountProperty =
            DependencyProperty.Register("TxtTimeFromToCount", typeof(int), typeof(UCConditionItem), new PropertyMetadata(default(int)));

        public int TxtTimeFromToCount
        {
            get { return (int)GetValue(TxtTimeFromToCountProperty); }
            set { SetValue(TxtTimeFromToCountProperty, value); }
        }

        public static readonly DependencyProperty TxtButtonProperty =
            DependencyProperty.Register("TxtButton", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));

        public string TxtButton
        {
            get { return (string)GetValue(TxtButtonProperty); }
            set { SetValue(TxtButtonProperty, value); }
        }


        public static readonly DependencyProperty TxtYesScoredProperty =
             DependencyProperty.Register("TxtYesScored", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtYesScored
        {
            get { return (string)GetValue(TxtYesScoredProperty); }
            set { SetValue(TxtYesScoredProperty, value); }
        }

        public static readonly DependencyProperty TxtNoScoredProperty =
             DependencyProperty.Register("TxtNoScored", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtNoScored
        {
            get { return (string)GetValue(TxtNoScoredProperty); }
            set { SetValue(TxtNoScoredProperty, value); }
        }

        public static readonly DependencyProperty IsYesCheckedProperty =
             DependencyProperty.Register("IsYesChecked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsYesChecked
        {
            get { return (bool)GetValue(IsYesCheckedProperty); }
            set { SetValue(IsYesCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsNoCheckedProperty =
             DependencyProperty.Register("IsNoChecked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsNoChecked
        {
            get { return (bool)GetValue(IsNoCheckedProperty); }
            set { SetValue(IsNoCheckedProperty, value); }
        }


        #region ABCD 条件
        public static readonly DependencyProperty TxtR1ScoredProperty =
            DependencyProperty.Register("TxtR1", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtR1
        {
            get { return (string)GetValue(TxtR1ScoredProperty); }
            set { SetValue(TxtR1ScoredProperty, value); }
        }

        public static readonly DependencyProperty TxtR2ScoredProperty =
            DependencyProperty.Register("TxtR2", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtR2
        {
            get { return (string)GetValue(TxtR2ScoredProperty); }
            set { SetValue(TxtR2ScoredProperty, value); }
        }

        public static readonly DependencyProperty TxtR3ScoredProperty =
            DependencyProperty.Register("TxtR3", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtR3
        {
            get { return (string)GetValue(TxtR3ScoredProperty); }
            set { SetValue(TxtR3ScoredProperty, value); }
        }

        public static readonly DependencyProperty TxtR4ScoredProperty =
            DependencyProperty.Register("TxtR4", typeof(string), typeof(UCConditionItem), new PropertyMetadata(default(string)));
        public string TxtR4
        {
            get { return (string)GetValue(TxtR4ScoredProperty); }
            set { SetValue(TxtR4ScoredProperty, value); }
        }

        public static readonly DependencyProperty IsR1CheckedProperty =
            DependencyProperty.Register("IsR1Checked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsR1Checked
        {
            get { return (bool)GetValue(IsR1CheckedProperty); }
            set { SetValue(IsR1CheckedProperty, value); }
        }

        public static readonly DependencyProperty IsR2CheckedProperty =
             DependencyProperty.Register("IsR2Checked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsR2Checked
        {
            get { return (bool)GetValue(IsR2CheckedProperty); }
            set { SetValue(IsR2CheckedProperty, value); }
        }

        public static readonly DependencyProperty IsR3CheckedProperty =
            DependencyProperty.Register("IsR3Checked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsR3Checked
        {
            get { return (bool)GetValue(IsR3CheckedProperty); }
            set { SetValue(IsR3CheckedProperty, value); }
        }

        public static readonly DependencyProperty IsR4CheckedProperty =
            DependencyProperty.Register("IsR4Checked", typeof(bool), typeof(UCConditionItem), new PropertyMetadata(default(bool)));
        public bool IsR4Checked
        {
            get { return (bool)GetValue(IsR4CheckedProperty); }
            set { SetValue(IsR4CheckedProperty, value); }
        }
        #endregion


        #endregion

        #region Template

        private const string PART_ListSubItems = "PART_ListSubItems";
        private const string PART_TabControlType = "PART_TabControlType";
        private const string PART_ComboTimeType0 = "PART_ComboTimeType0";
        private const string PART_ComboTimeType1 = "PART_ComboTimeType1";

        private const string PART_ComboScoreSheet = "PART_ComboScoreSheet";

        private const string PART_Score_ListSubItems = "PART_Score_ListSubItems";
        private const string PART_ComboSkillGroupOrOrg = "PART_ComboSkillGroupOrOrg";

        private TabControl mTabControlType;
        private ListBox mListBoxSubItems;
        private ComboBox mComboTimeType0;
        private ComboBox mComboTimeType1;

        private ListBox mListBox_ScoreSubItems;

        private ComboBox mComboScoreSheet;
        private ComboBox mComboOrgSkillGroup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mListBoxSubItems = GetTemplateChild(PART_ListSubItems) as ListBox;
            if (mListBoxSubItems != null)
            {
                mListBoxSubItems.ItemsSource = mListConditionItemSubItems;
            }
            mTabControlType = GetTemplateChild(PART_TabControlType) as TabControl;
            if (mTabControlType != null)
            {

            }
            mComboTimeType0 = GetTemplateChild(PART_ComboTimeType0) as ComboBox;
            if (mComboTimeType0 != null)
            {
                mComboTimeType0.ItemsSource = mListTimeTypeBasicItems;
            }
            mComboTimeType1 = GetTemplateChild(PART_ComboTimeType1) as ComboBox;
            if (mComboTimeType1 != null)
            {
                mComboTimeType1.ItemsSource = mListTimeTypeFromToItems;
            }

            mComboScoreSheet = GetTemplateChild(PART_ComboScoreSheet) as ComboBox;
            if (mComboScoreSheet != null)
            {
                mComboScoreSheet.ItemsSource = mListScoreSheetItems;
            }

            mListBox_ScoreSubItems = GetTemplateChild(PART_Score_ListSubItems) as ListBox;
            if (mListBox_ScoreSubItems != null)
            {
                mListBox_ScoreSubItems.ItemsSource = mListIsScoreItems;
            }

            mComboOrgSkillGroup = GetTemplateChild(PART_ComboSkillGroupOrOrg) as ComboBox;
            if (mComboOrgSkillGroup != null)
            {
                mComboOrgSkillGroup.ItemsSource = mListOrgOrSkillGroup;
            }

            SetValue();
        }

        #endregion

        public UCConditionItem()
        {
            InitializeComponent();
            mListSubItems = new List<QueryConditionSubItem>();

            mListConditionItemSubItems = new ObservableCollection<ConditionItemSubItem>();
            mListTimeTypeBasicItems = new ObservableCollection<ConditionItemSubItem>();
            mListTimeTypeFromToItems = new ObservableCollection<ConditionItemSubItem>();

            mListScoreSheetItems = new ObservableCollection<ConditionItemSubItem>();

            mListIsScoreItems = new ObservableCollection<ConditionItemSubItem>();
            mListOrgOrSkillGroup = new ObservableCollection<ConditionItemSubItem>();

            mListExtension = new List<string>();
            mListOrg = new List<string>();
            mListAgent = new List<string>();

            Loaded += UCConditionItem_Loaded;
            mIsInitialed = false;

        }


        void UCConditionItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInitialed)
            {
                Init();
                SetValue();
                mIsInitialed = true;

                ChangeLanguage();
            }
        }

        private void Init()
        {
            if (ConditionItem == null)
            {
                return;
            }
            //必显示项
            if (ConditionItem.ViewMode == 2)
            {
                IsChecked = true;
            }
            if (ConditionResult == null)
            {
                ConditionResult = new QueryConditionDetail();
                ConditionResult.ConditionItemID = ConditionItem.ID;
                ConditionResult.IsEnable = IsChecked;
            }
            DataContext = ConditionItem;
            CustomConditionItemFormat format = ConditionItem.Format;
            Format = format;
            int widthSize = 150;
            int heightSize = 35;
            switch (format)
            {
                case CustomConditionItemFormat.OneFour:
                    Width = widthSize * 4;
                    Height = heightSize;
                    break;
                case CustomConditionItemFormat.OneTwo:
                    Width = widthSize * 2;
                    Height = heightSize;
                    break;
                case CustomConditionItemFormat.OneOne:
                    Width = widthSize;
                    Height = heightSize;
                    break;
                case CustomConditionItemFormat.TwoFour:
                    Width = widthSize * 4;
                    Height = heightSize * 2;
                    break;
            }
            ItemType = ConditionItem.Type;
            Title = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", ConditionItem.ID), ConditionItem.Name);
            InitValue();
        }

        private void InitValue()
        {
            if (ConditionItem == null) { return; }
            try
            {
                CustomConditionItemType type = ConditionItem.Type;
                switch (type)
                {
                    case CustomConditionItemType.TimeFromTo:
                        //Value01   开始时间
                        //Value02   结束时间
                        Value01 = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                        Value02 = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                        break;
                    case CustomConditionItemType.DurationFromTo:
                        //Value01   最短时长
                        //Value02   最长时长
                        Value01 = "00:00:00";
                        Value02 = "00:30:00";
                        break;
                    case CustomConditionItemType.MultiText:
                        //Value01   临时ID
                        Value01 = string.Empty;
                        break;
                    case CustomConditionItemType.LikeText:
                        //Value01   文本内容
                        Value01 = string.Empty;
                        IsLike = true;
                        break;
                    case CustomConditionItemType.CheckSelect:
                        //根据Param获得子项
                        InitConditionItemSubItems();
                        break;
                    case CustomConditionItemType.TimeTypeFromTo:
                        //Value01   开始时间
                        //Value02   结束时间
                        Value01 = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                        Value02 = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                        TxtTimeFromToCount = 1;
                        InitTimeTypeBasicItems();
                        InitTimeTypeFromToItems();
                        break;
                    case CustomConditionItemType.AutoLikeText:
                        //Value01   文本内容
                        Value01 = string.Empty;
                        break;
                    case CustomConditionItemType.ButtonTreeAddMultiText:
                        Value01 = string.Empty;
                        Value02 = string.Empty;
                        break;
                    case CustomConditionItemType.ComboxboxSelect:
                        InitScoreSheetItems();
                        if (mComboScoreSheet != null)
                        {
                            var tempIndex = mComboScoreSheet.SelectedIndex;

                            var temp = mListScoreSheetItems.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboScoreSheet != null)
                                {
                                    mComboScoreSheet.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.CheckSelectText:
                        //InitConditionItemSubItems();
                        IsYesChecked = false;
                        IsNoChecked = false;
                        Value01 = "0";
                        Value02 = "100";
                        break;
                    case CustomConditionItemType.RadionBoxCombox:
                        InitABCDOrgSkillInf();
                        if (mComboOrgSkillGroup != null)
                        {
                            var tempIndex = mComboOrgSkillGroup.SelectedIndex;

                            var temp = mListOrgOrSkillGroup.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboOrgSkillGroup != null)
                                {
                                    mComboOrgSkillGroup.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.RadionBoxCombox1:
                        InitABCDOrgSkillInf();
                        if (mComboOrgSkillGroup != null)
                        {
                            var tempIndex = mComboOrgSkillGroup.SelectedIndex;

                            var temp = mListOrgOrSkillGroup.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboOrgSkillGroup != null)
                                {
                                    mComboOrgSkillGroup.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.AutoMultiLikeText:
                        //Value01   文本内容
                        Value01 = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitConditionItemSubItems()
        {
            try
            {
                mListConditionItemSubItems.Clear();
                if (ConditionItem == null) { return; }
                string strInfos = ConditionItem.Param;
                if (string.IsNullOrEmpty(strInfos)) { return; }
                string[] arrInfos = strInfos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrInfos.Length; i++)
                {
                    ConditionItemSubItem item = new ConditionItemSubItem();
                    item.Name = arrInfos[i];
                    item.IsChecked = false;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("3102C{0}{1}", ConditionItem.ID, item.Name),
                        item.Name);
                    mListConditionItemSubItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitTimeTypeBasicItems()
        {
            try
            {
                mListTimeTypeBasicItems.Clear();
                ConditionItemSubItem item = new ConditionItemSubItem();
                item.Name = "Today";
                item.Value = 0;
                item.IsChecked = true;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP007{0}", item.Name), "Today");
                mListTimeTypeBasicItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "LastTwoDays";
                item.Value = 1;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP007{0}", item.Name), "LastTwoDays");
                mListTimeTypeBasicItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "ThisWeek";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP007{0}", item.Name), "This Week");
                mListTimeTypeBasicItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "ThisMonth";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP007{0}", item.Name), "This Month");
                mListTimeTypeBasicItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitTimeTypeFromToItems()
        {
            try
            {
                mListTimeTypeFromToItems.Clear();
                ConditionItemSubItem item = new ConditionItemSubItem();
                item.Name = "Day";
                item.Value = 0;
                item.IsChecked = true;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP008{0}", item.Name), "Day");
                mListTimeTypeFromToItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "Week";
                item.Value = 1;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP008{0}", item.Name), "Week");
                mListTimeTypeFromToItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "Month";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP008{0}", item.Name), "Month");
                mListTimeTypeFromToItems.Add(item);
                item = new ConditionItemSubItem();
                item.Name = "Year";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP008{0}", item.Name), "Year");
                mListTimeTypeFromToItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitScoreSheetItems()
        {
            try
            {
                mListScoreSheetItems.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetAllScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    BasicScoreSheetItem item = new BasicScoreSheetItem(info);
                    if (item.Flag == 0)
                    {
                        item.StrScore = CurrentApp.GetLanguageInfo("3102TIP001IsScored0", "No Score");
                    }
                    else
                    {
                        item.StrScore = item.Score.ToString();
                    }
                    ConditionItemSubItem itemNeed = new ConditionItemSubItem();
                    //在界面上显示的评分表名
                    itemNeed.Display = item.Title;
                    //需要放入查询的评分表ID（long）;
                    itemNeed.QueryValue = item.ScoreSheetID;
                    itemNeed.Value = i;
                    mListScoreSheetItems.Add(itemNeed);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitABCDOrgSkillInf()
        {
            try
            {
                mListOrgOrSkillGroup.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetOrgSkillGroupInf;
                webRequest.Session = CurrentApp.Session;
                if (ConditionItem.ID == S3102Consts.CON_SERVICEATTITUDE)
                {
                    webRequest.ListData.Add("3110000000000000001");
                }
                if (ConditionItem.ID == S3102Consts.CON_RECORDDURATIONEXCEPT)
                {
                    webRequest.ListData.Add("3110000000000000006");
                }
                if (ConditionItem.ID == S3102Consts.CON_REPEATEDCALL)
                {
                    webRequest.ListData.Add("3110000000000000003");
                }
                if (ConditionItem.ID == S3102Consts.CON_CALLPEAK)
                {
                    webRequest.ListData.Add("3110000000000000004");
                }
                if (ConditionItem.ID == S3102Consts.CON_PROFESSIONAlLEVEL)
                {
                    webRequest.ListData.Add("3110000000000000002");
                }
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ABCD_OrgSkillGroup>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ABCD_OrgSkillGroup info = optReturn.Data as ABCD_OrgSkillGroup;
                    ConditionItemSubItem itemNeed = new ConditionItemSubItem();
                    itemNeed.Display = info.OrgSkillGroupName;
                    itemNeed.QueryValue = info.OrgSkillGroupID;
                    itemNeed.Value = i;
                    itemNeed.Name = info.InColumn.ToString();
                    mListOrgOrSkillGroup.Add(itemNeed);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //加载机构下的子机构
        private void InitOrg(string temp)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetOrgList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(temp);
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    mListOrg.Add(strInfo);
                    InitOrg(strInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAgent()
        {
            //加载所在机构下的坐席
            try
            {
                mListAgent.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetAgentList;
                webRequest.Session = CurrentApp.Session;
                for (int i = 0; i < mListOrg.Count; i++)
                {
                    webRequest.ListData.Add(mListOrg[i]);
                }
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                mListAgent.Add("N/A");
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    mListAgent.Add(strInfo);
                }
                if (mListAgent != null)
                {
                    string tempID = PutTempData(mListAgent);
                    ConditionResult.Value06 = tempID;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitExtension()
        {
            //加载所在机构下的分机
            try
            {
                mListExtension.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetExtensionList;
                webRequest.Session = CurrentApp.Session;
                for (int i = 0; i < mListOrg.Count; i++)
                {
                    webRequest.ListData.Add(mListOrg[i]);
                }
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    mListExtension.Add(strInfo);
                }
                if (mListExtension != null)
                {
                    string tempID = PutTempData(mListExtension);
                    ConditionResult.Value07 = tempID;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetValue()
        {
            if (ConditionItem == null
                || ConditionResult == null)
            {
                return;
            }
            try
            {
                IsChecked = ConditionResult.IsEnable;
                CustomConditionItemType type = ConditionItem.Type;
                string strItemValues;
                string strValue01;
                string strValue02;
                string strValue03;
                string strValue04;
                string strValue05;
                DateTime dtValue;
                int intValue;
                switch (type)
                {
                    case CustomConditionItemType.TimeFromTo:
                        Value01 = ConditionResult.Value01;
                        Value02 = ConditionResult.Value02;
                        if (string.IsNullOrEmpty(Value01))
                        {
                            Value01 = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                        }
                        if (string.IsNullOrEmpty(Value02))
                        {
                            Value02 = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                        }

                        break;
                    case CustomConditionItemType.DurationFromTo:
                        if (int.TryParse(ConditionResult.Value01, out intValue))
                        {
                            Value01 = Converter.Second2Time(intValue);
                        }
                        if (int.TryParse(ConditionResult.Value02, out intValue))
                        {
                            Value02 = Converter.Second2Time(intValue);
                        }
                        if (string.IsNullOrEmpty(Value01))
                        {
                            Value01 = string.Format("00:00:00");
                        }
                        if (String.IsNullOrEmpty(Value02))
                        {
                            Value02 = string.Format("00:30:00");
                        }
                        break;
                    case CustomConditionItemType.MultiText:
                        Value01 = InitSubItemContent();
                        break;
                    case CustomConditionItemType.LikeText:
                        Value01 = ConditionResult.Value01;
                        IsLike = ConditionResult.Value02 == "Y";
                        break;
                    case CustomConditionItemType.CheckSelect:
                        strItemValues = ConditionResult.Value01;
                        if (!string.IsNullOrEmpty(strItemValues))
                        {
                            for (int i = 0; i < mListConditionItemSubItems.Count; i++)
                            {
                                if (strItemValues.Length > i)
                                {
                                    mListConditionItemSubItems[i].IsChecked = strItemValues.Substring(i, 1) == "1";
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.TimeTypeFromTo:
                        //Value01   开始时间
                        //Value02   结束时间
                        //Value03   时间类型(0:开始到结束时间；1：常用时间范围；2：指定数量的的时间范围）
                        //Value04   1：时间范围类型（0：今天；1：最近两天；2：本周；3：本月）2：数量
                        //Value05   2：时间范围类型（0：天；1：周；2：月；3：年）

                        strValue01 = ConditionResult.Value01;
                        strValue02 = ConditionResult.Value02;

                        if (string.IsNullOrEmpty(strValue01)
                            || !DateTime.TryParse(strValue01, out dtValue))
                        {
                            Value01 = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                        }
                        else
                        {
                            Value01 = Convert.ToDateTime(ConditionResult.Value01).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (string.IsNullOrEmpty(strValue02)
                            || !DateTime.TryParse(strValue02, out dtValue))
                        {
                            Value02 = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                        }
                        else
                        {
                            Value02 = Convert.ToDateTime(ConditionResult.Value02).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        if (mTabControlType != null)
                        {
                            mTabControlType.SelectedIndex = 0;
                        }
                        if (mComboTimeType0 != null)
                        {
                            mComboTimeType0.SelectedIndex = 0;
                        }
                        if (mComboTimeType1 != null)
                        {
                            mComboTimeType1.SelectedIndex = 0;
                        }

                        strValue03 = ConditionResult.Value03;
                        if (int.TryParse(strValue03, out intValue))
                        {
                            if (mTabControlType != null)
                            {
                                mTabControlType.SelectedIndex = intValue;
                            }
                        }
                        if (strValue03 == "1")
                        {
                            //常用
                            strValue04 = ConditionResult.Value04;
                            if (mComboTimeType0 != null)
                            {
                                var temp =
                                    mListTimeTypeBasicItems.FirstOrDefault(t => t.Value.ToString() == strValue04);
                                if (temp != null)
                                {
                                    temp.IsChecked = true;
                                    if (mComboTimeType0 != null)
                                    {
                                        mComboTimeType0.SelectedItem = temp;
                                    }
                                }
                            }
                        }
                        if (strValue03 == "2")
                        {
                            //范围
                            //Value04   单位
                            //Value05   数量
                            strValue04 = ConditionResult.Value04;
                            strValue05 = ConditionResult.Value05;
                            if (mComboTimeType1 != null)
                            {
                                var temp =
                                   mListTimeTypeFromToItems.FirstOrDefault(t => t.Value.ToString() == strValue04);
                                if (temp != null)
                                {
                                    temp.IsChecked = true;
                                    if (mComboTimeType1 != null)
                                    {
                                        mComboTimeType1.SelectedItem = temp;
                                    }
                                }
                            }
                            if (int.TryParse(strValue05, out intValue))
                            {
                                TxtTimeFromToCount = Convert.ToInt32(strValue05);
                            }
                            else
                            {
                                TxtTimeFromToCount = 1;
                            }
                        }
                        break;
                    case CustomConditionItemType.AutoLikeText:
                        Value01 = ConditionResult.Value01;
                        break;
                    case CustomConditionItemType.AutoMultiLikeText:
                        Value01 = ConditionResult.Value01;
                        break;
                    case CustomConditionItemType.ButtonTreeAddMultiText:
                        Value01 = ConditionResult.Value01;
                        Value02 = ConditionResult.Value02;
                        break;
                    case CustomConditionItemType.ComboxboxSelect:
                        if (mComboScoreSheet != null)
                        {
                            var tempIndex = ConditionResult.Value02;
                            if (tempIndex == null)
                            {
                                tempIndex = "0";
                            }

                            var temp = mListScoreSheetItems.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboScoreSheet != null)
                                {
                                    mComboScoreSheet.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.CheckSelectText:
                        strItemValues = ConditionResult.Value01;
                        if (!string.IsNullOrEmpty(strItemValues))
                        {
                            IsYesChecked = strItemValues.Substring(0, 1) == "1";
                            IsNoChecked = strItemValues.Substring(1, 1) == "1";
                            if (IsYesChecked)
                            {
                                Value01 = ConditionResult.Value02;
                                Value02 = ConditionResult.Value03;
                            }
                        }
                        break;
                    case CustomConditionItemType.RadionBoxCombox:
                        switch (ConditionResult.Value05)
                        {
                            case "1":
                                IsR1Checked = true;
                                IsR2Checked = false;
                                IsR3Checked = false;
                                break;
                            case "2":
                                IsR2Checked = true;
                                IsR1Checked = false;
                                IsR3Checked = false;
                                break;
                            case "3":
                                IsR3Checked = true;
                                IsR1Checked = false;
                                IsR2Checked = false;
                                break;
                        }
                        if (mComboOrgSkillGroup != null)
                        {
                            var tempIndex = ConditionResult.Value02;
                            if (tempIndex == null)
                            {
                                tempIndex = "0";
                            }
                            var temp = mListOrgOrSkillGroup.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboOrgSkillGroup != null)
                                {
                                    mComboOrgSkillGroup.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.RadionBoxCombox1:
                        switch (ConditionResult.Value05)
                        {
                            case "1":
                                IsR1Checked = true;
                                IsR2Checked = false;
                                IsR3Checked = false;
                                IsR4Checked = false;
                                break;
                            case "2":
                                IsR2Checked = true;
                                IsR1Checked = false;
                                IsR3Checked = false;
                                IsR4Checked = false;
                                break;
                            case "3":
                                IsR3Checked = true;
                                IsR1Checked = false;
                                IsR2Checked = false;
                                IsR4Checked = false;
                                break;
                            case "4":
                                IsR4Checked = true;
                                IsR1Checked = false;
                                IsR2Checked = false;
                                IsR3Checked = false;
                                break;
                        }
                        if (mComboOrgSkillGroup != null)
                        {
                            var tempIndex = ConditionResult.Value02;
                            if (tempIndex == null)
                            {
                                tempIndex = "0";
                            }
                            var temp = mListOrgOrSkillGroup.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboOrgSkillGroup != null)
                                {
                                    mComboOrgSkillGroup.SelectedItem = temp;
                                }
                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                ShowException(ex.ToString());
            }
        }

        private string InitSubItemContent()
        {
            //获取子项
            string strReturn = string.Empty;
            try
            {
                if (ConditionItem == null
                    || ConditionResult == null)
                {
                    return strReturn;
                }
                mListSubItems.Clear();
                if (!string.IsNullOrEmpty(ConditionResult.Value01))
                {
                    //根据临时ID从数据库获取
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3102Codes.GetConditionSubItem;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(ConditionResult.Value01);
                    webRequest.ListData.Add(ConditionResult.QueryID.ToString());
                    webRequest.ListData.Add(ConditionResult.ConditionItemID.ToString());
                    Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return strReturn;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("Fail. WebReturn ListData is null"));
                        return strReturn;
                    }

                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<QueryConditionSubItem>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Error.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return strReturn;
                        }
                        QueryConditionSubItem item = optReturn.Data as QueryConditionSubItem;
                        if (item == null)
                        {
                            ShowException(string.Format("Error. SubItem is null"));
                            return strReturn;
                        }
                        mListSubItems.Add(item);
                    }
                }
                //如果主页面有勾选管理对象，追加进去
                if (ListSelectedObjects != null)
                {
                    List<ObjectItem> listItems =
                        ListSelectedObjects.Where(s => s.ObjType.ToString() == ConditionItem.Param).ToList();
                    if (listItems.Count > 0)
                    {
                        for (int i = 0; i < listItems.Count; i++)
                        {
                            ObjectItem item = listItems[i];
                            switch (item.ObjType)
                            {
                                //已勾选的分机或坐席追加进去
                                case ConstValue.RESOURCE_REALEXT:
                                case ConstValue.RESOURCE_AGENT:
                                case ConstValue.RESOURCE_EXTENSION:
                                    QueryConditionSubItem subItem = new QueryConditionSubItem();
                                    subItem.QueryID = ConditionResult.QueryID;
                                    subItem.ConditionItemID = ConditionItem.ID;
                                    subItem.Value02 = item.ObjID.ToString();
                                    subItem.Value01 = item.Name;
                                    subItem.Value03 = item.Description;
                                    subItem.Value04 = item.FullName;
                                    QueryConditionSubItem temp =
                                        mListSubItems.FirstOrDefault(
                                            s =>
                                                s.QueryID == subItem.QueryID &&
                                                s.ConditionItemID == subItem.ConditionItemID &&
                                                s.Value01 == subItem.Value01);
                                    if (temp == null)
                                    {
                                        mListSubItems.Add(subItem);
                                    }
                                    IsChecked = true;
                                    break;
                            }
                        }
                    }
                }
                string strSubItems = string.Empty;
                for (int i = 0; i < mListSubItems.Count; i++)
                {
                    strSubItems += string.Format("{0},", mListSubItems[i].Value01);
                }
                if (strSubItems.Length > 0)
                {
                    strSubItems = strSubItems.Substring(0, strSubItems.Length - 1);
                }
                strReturn = strSubItems;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return strReturn;
        }

        public void GetValue()//获得Value值,每个条件项目里面都有多个Value值
        {
            try
            {
                if (ConditionItem == null)
                {
                    return;
                }
                if (ConditionResult == null)
                {
                    ConditionResult = new QueryConditionDetail();
                    ConditionResult.ConditionItemID = ConditionItem.ID;
                }
                ConditionResult.IsEnable = IsChecked;//检查是否在前面勾上了checkbox.如果勾上了,那么这个条件就是要用做查询条件的
                if (IsChecked != true)
                {
                    ConditionResult.Value01 = string.Empty;
                    ConditionResult.Value02 = string.Empty;
                    ConditionResult.Value03 = string.Empty;
                    ConditionResult.Value04 = string.Empty;
                    ConditionResult.Value05 = string.Empty;
                    return;
                }
                CustomConditionItemType type = ConditionItem.Type;//勾上的是哪个
                string strItemValues = string.Empty;
                string strValue03;
                switch (type)
                {
                    case CustomConditionItemType.TimeFromTo:
                        ConditionResult.Value01 = Convert.ToDateTime(Value01).ToString("yyyy-MM-dd HH:mm:ss");
                        ConditionResult.Value02 = Convert.ToDateTime(Value02).ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case CustomConditionItemType.DurationFromTo:
                        ConditionResult.Value01 = Converter.Time2Second(Value01).ToString();
                        ConditionResult.Value02 = Converter.Time2Second(Value02).ToString();
                        break;
                    case CustomConditionItemType.MultiText:
                        GetSubItemContent(Value01);
                        break;
                    case CustomConditionItemType.LikeText:
                        ConditionResult.Value01 = Value01;
                        ConditionResult.Value02 = IsLike ? "Y" : "N";
                        break;
                    case CustomConditionItemType.CheckSelect:
                        for (int i = 0; i < mListConditionItemSubItems.Count; i++)
                        {
                            if (mListConditionItemSubItems[i].IsChecked)
                            {
                                strItemValues += "1";
                            }
                            else
                            {
                                strItemValues += "0";
                            }
                        }
                        ConditionResult.Value01 = strItemValues;
                        break;
                    case CustomConditionItemType.TimeTypeFromTo:
                        if (mTabControlType != null)
                        {
                            strValue03 = mTabControlType.SelectedIndex.ToString();
                            ConditionResult.Value03 = strValue03;
                            switch (strValue03)
                            {
                                case "0":
                                    ConditionResult.Value04 = string.Empty;
                                    break;
                                case "1":
                                    if (mComboTimeType0 != null)
                                    {
                                        var temp = mComboTimeType0.SelectedItem as ConditionItemSubItem;
                                        if (temp != null)
                                        {
                                            ConditionResult.Value04 = temp.Value.ToString();
                                            DateTime now = DateTime.Now;
                                            DateTime nextMonth = now.AddMonths(1);
                                            int week = Convert.ToInt32(now.DayOfWeek);
                                            int daydiff = (-1) * week + 1;
                                            int dayadd = 7 - week;
                                            switch (temp.Value)
                                            { //时间相关的~**
                                                case 0:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        DateTime.Now.ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59",
                                                        DateTime.Now.ToString("yyyy-MM-dd"));
                                                    break;
                                                case 1:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59",
                                                        DateTime.Now.ToString("yyyy-MM-dd"));
                                                    break;
                                                case 2:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        now.AddDays(daydiff).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59",
                                                        now.AddDays(dayadd).ToString("yyyy-MM-dd"));
                                                    break;
                                                case 3:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                       now.AddDays(-(now.Day) + 1).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59",
                                                        nextMonth.AddDays(-(now.Day)).ToString("yyyy-MM-dd"));
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case "2":
                                    if (mComboTimeType1 != null)
                                    {
                                        var temp = mComboTimeType1.SelectedItem as ConditionItemSubItem;
                                        if (temp != null)
                                        {
                                            ConditionResult.Value04 = temp.Value.ToString();
                                            ConditionResult.Value05 = TxtTimeFromToCount.ToString();
                                            int intCount = Convert.ToInt32(TxtTimeFromToCount);
                                            DateTime now = DateTime.Now;
                                            switch (temp.Value)
                                            {
                                                case 0:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        now.AddDays(-intCount).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                                    break;
                                                case 1:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        now.AddDays(-intCount * 7).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                                    break;
                                                case 2:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        now.AddMonths(-intCount).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                                    break;
                                                case 3:
                                                    Value01 = string.Format("{0} 00:00:00",
                                                        now.AddYears(-intCount).ToString("yyyy-MM-dd"));
                                                    Value02 = string.Format("{0} 23:59:59", now.ToString("yyyy-MM-dd"));
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    ShowException(string.Format("StrValue03 invalid"));
                                    return;
                            }
                            ConditionResult.Value01 = Convert.ToDateTime(Value01).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            ConditionResult.Value02 = Convert.ToDateTime(Value02).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        break;
                    case CustomConditionItemType.AutoLikeText:
                        ConditionResult.Value01 = Value01;
                        break;
                    case CustomConditionItemType.AutoMultiLikeText:
                        ConditionResult.Value01 = Value01;
                        break;
                    case CustomConditionItemType.ButtonTreeAddMultiText:
                        //Value01 存的是在界面上显示的质检员名称以及登陆账号  Value02是查询时用到的质检员ID的一个字符串，其中每个质检员用“,”隔开
                        if (Value02 == null)
                        {
                            break;
                        }
                        ConditionResult.Value01 = Value01;
                        ConditionResult.Value02 = Value02;
                        break;
                    case CustomConditionItemType.ComboxboxSelect:
                        if (mComboScoreSheet != null)
                        {
                            var temp = mComboScoreSheet.SelectedItem as ConditionItemSubItem;
                            if (temp != null)
                            {
                                //Value01是19位的评分表ID；Value02是评分表展示的时候在SelectedItem里的位置；Value03是表示评分表名也就是在界面上显示的
                                ConditionResult.Value01 = temp.QueryValue.ToString();
                                ConditionResult.Value02 = temp.Value.ToString();
                                ConditionResult.Value03 = temp.Display.ToString();
                            }
                        }
                        break;
                    case CustomConditionItemType.CheckSelectText:
                        //ConditionResult.Value01  是否评分的勾选
                        //ConditionResult.Value02  第一个分数
                        //ConditionResult.Value03  第二个分数
                        if (IsYesChecked)
                        {
                            strItemValues += "1";
                            ConditionResult.Value02 = Value01;
                            ConditionResult.Value03 = Value02;
                        }
                        else
                        {
                            strItemValues += "0";
                        }

                        if (IsNoChecked)
                        {
                            strItemValues += "1";
                        }
                        else
                        {
                            strItemValues += "0";
                        }
                        ConditionResult.Value01 = strItemValues;
                        break;
                    case CustomConditionItemType.RadionBoxCombox:
                        string strValue_ = string.Empty;
                        if (IsR1Checked)
                        {
                            IsR2Checked = false;
                            IsR3Checked = false;
                            strValue_ = "1";
                        }
                        if (IsR2Checked)
                        {
                            IsR1Checked = false;
                            IsR3Checked = false;
                            strValue_ = "2";
                        }
                        if (IsR3Checked)
                        {
                            IsR1Checked = false;
                            IsR2Checked = false;
                            strValue_ = "3";
                        }
                        if (mComboOrgSkillGroup != null)
                        {
                            var temp = mComboOrgSkillGroup.SelectedItem as ConditionItemSubItem;
                            if (temp != null)
                            {
                                //Value01是19位的机构ID
                                //Value02是机构展示的时候在SelectedItem里的位置
                                //Value03是表示这个机构绑定的ABCD参数大项在的T_31_054表中统计结果的哪一列
                                //Value04是表示评分表名也就是在界面上显示的
                                //Value05是表示前面的RadioButton选的是哪一个
                                //Value06是坐席存入临时表的临时表ID
                                //Value07是分机存入临时表的临时表ID
                                ConditionResult.Value01 = temp.QueryValue.ToString();
                                ConditionResult.Value02 = temp.Value.ToString();
                                string tempColum = string.Format("C{0}", int.Parse(temp.Name.ToString()).ToString("000"));
                                ConditionResult.Value03 = tempColum;
                                ConditionResult.Value04 = temp.Display.ToString();
                                ConditionResult.Value05 = strValue_;
                                InitOrg(ConditionResult.Value01);
                                mListOrg.Add(ConditionResult.Value01);
                                if (PageParent.PageParent.GroupingWay.IndexOf("A") >= 0)
                                {
                                    InitAgent();
                                }
                                if (PageParent.PageParent.GroupingWay.IndexOf("E") >= 0)
                                {
                                    InitExtension();
                                }
                            }
                        }
                        break;
                    case CustomConditionItemType.RadionBoxCombox1:
                        string strValue__ = string.Empty;
                        if (IsR1Checked)
                        {
                            IsR2Checked = false;
                            IsR3Checked = false;
                            IsR4Checked = false;
                            strValue__ = "1";
                        }
                        if (IsR2Checked)
                        {
                            IsR1Checked = false;
                            IsR3Checked = false;
                            IsR4Checked = false;
                            strValue__ = "2";
                        }
                        if (IsR3Checked)
                        {
                            IsR1Checked = false;
                            IsR2Checked = false;
                            IsR4Checked = false;
                            strValue__ = "3";
                        }
                        if (IsR4Checked)
                        {
                            IsR1Checked = false;
                            IsR2Checked = false;
                            IsR3Checked = false;
                            strValue__ = "4";
                        }
                        if (mComboOrgSkillGroup != null)
                        {
                            var temp = mComboOrgSkillGroup.SelectedItem as ConditionItemSubItem;
                            if (temp != null)
                            {
                                //Value01是19位的机构ID
                                //Value02是机构展示的时候在SelectedItem里的位置
                                //Value03是表示这个机构绑定的ABCD参数大项在的T_31_054表中统计结果的哪一列
                                //Value04是表示评分表名也就是在界面上显示的
                                //Value05是表示前面的RadioButton选的是哪一个
                                //Value06是坐席存入临时表的临时表ID
                                //Value07是分机存入临时表的临时表ID
                                ConditionResult.Value01 = temp.QueryValue.ToString();
                                ConditionResult.Value02 = temp.Value.ToString();
                                string tempColum = string.Format("C{0}", int.Parse(temp.Name.ToString()).ToString("000"));
                                ConditionResult.Value03 = tempColum;
                                ConditionResult.Value04 = temp.Display.ToString();
                                ConditionResult.Value05 = strValue__;
                                InitOrg(ConditionResult.Value01);
                                mListOrg.Add(ConditionResult.Value01);
                                if (PageParent.PageParent.GroupingWay.IndexOf("A") >= 0)
                                {
                                    InitAgent();
                                }
                                if (PageParent.PageParent.GroupingWay.IndexOf("E") >= 0)
                                {
                                    InitExtension();
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public bool CheckInput()//检查输入的条件,在记录流水号，通道号，时间，机器id不能有特殊字符.  加了点点   by 汤澈
        {
            try
            {
                if (ConditionItem == null)
                {
                    return false;
                }
                long id = ConditionItem.ID;
                string strName = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", ConditionItem.ID), ConditionItem.Name);
                if (IsChecked != true) { return true; }
                CustomConditionItemType type = ConditionItem.Type;
                string strValue03;
                DateTime dtValue;
                switch (id)
                {
                    case S3102Consts.CON_TIMETYPEFROMTO:
                        if (mTabControlType != null)
                        {
                            strValue03 = mTabControlType.SelectedIndex.ToString();
                            switch (strValue03)
                            {
                                case "0":
                                    if (!DateTime.TryParse(Value01, out dtValue)
                                        || !DateTime.TryParse(Value02, out dtValue)
                                        || DateTime.Parse(Value01) >= DateTime.Parse("2100-12-31 23:59:59")
                                        || DateTime.Parse(Value02) >= DateTime.Parse("2100-12-31 23:59:59")
                                        || DateTime.Parse(Value01) <= DateTime.Parse("1900-01-01 00:00:00")
                                        || DateTime.Parse(Value02) <= DateTime.Parse("1900-01-01 00:00:00")
                                        || DateTime.Parse(Value01) > DateTime.Parse(Value02))
                                    {
                                        ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                                        return false;
                                    }
                                    break;
                                case "1":

                                    break;
                                case "2":
                                    if (mComboTimeType1 != null)
                                    {
                                        var temp = mComboTimeType1.SelectedItem as ConditionItemSubItem;
                                        if (temp != null)
                                        {
                                            int intCount = Convert.ToInt32(TxtTimeFromToCount);
                                            if (intCount < 0)
                                            {
                                                ShowException(CurrentApp.GetLanguageInfo("3102N034", "Don't input Negative"));
                                                return false;
                                            }
                                            DateTime now = DateTime.Now;
                                            DateTime OriginalTime = DateTime.Parse("1900-01-01 00:00:00");
                                            TimeSpan ts = now - OriginalTime;
                                            switch (temp.Value)
                                            {
                                                case 0:
                                                    if (intCount > ts.Days)
                                                    {
                                                        ShowException(CurrentApp.GetLanguageInfo("3102N030", "输入的天数过长"));
                                                        return false;
                                                    }
                                                    break;
                                                case 1:
                                                    if (intCount > ts.Days / 7)
                                                    {
                                                        ShowException(CurrentApp.GetLanguageInfo("3102N031", "输入的周数过长"));
                                                        return false;
                                                    }
                                                    break;
                                                case 2:
                                                    if (intCount > ts.Days / 30)
                                                    {
                                                        ShowException(CurrentApp.GetLanguageInfo("3102N032", "输入的月数过长"));
                                                        return false;
                                                    }
                                                    break;
                                                case 3:
                                                    if (intCount > ts.Days / 365)
                                                    {
                                                        ShowException(CurrentApp.GetLanguageInfo("3102N033", "输入的年数过长"));
                                                        return false;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    ShowException(string.Format("StrValue03 invalid"));
                                    return false;
                            }
                            //ConditionResult.Value01 = Convert.ToDateTime(Value01).ToString("yyyy-MM-dd HH:mm:ss");
                            //ConditionResult.Value02 = Convert.ToDateTime(Value02).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        break;
                    case S3102Consts.CON_DURATIONFROMTO://检查录音时长
                        if (!DateTime.TryParse(Value01, out dtValue)
                            || !DateTime.TryParse(Value02, out dtValue)
                            || DateTime.Parse(Value01) > DateTime.Parse(Value02))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }

                        break;
                    case S3102Consts.CON_VOICEID_MULTITEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }
                        string VOICEID_content = Value01;
                        List<string> VOICEID_listValueCheck = new List<string>();
                        string[] arrContent = VOICEID_content.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < arrContent.Length; i++)
                        {
                            VOICEID_listValueCheck.Add(arrContent[i]);
                        }
                        long VOICEID_longValue;
                        for (int i = 0; i < VOICEID_listValueCheck.Count; i++)
                        {
                            if (!long.TryParse(VOICEID_listValueCheck[i], out VOICEID_longValue))
                            {
                                ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                                return false;
                            }
                        }
                        break;

                    case S3102Consts.CON_PARTICIPANTNUM:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_CHANNELID_MULTITEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }
                        string CHANNELID_content = Value01;
                        List<string> CHANNELID_listValueCheck = new List<string>();
                        string[] CHANNELID_arrContent = CHANNELID_content.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < CHANNELID_arrContent.Length; i++)
                        {
                            CHANNELID_listValueCheck.Add(CHANNELID_arrContent[i]);
                        }
                        long CHANNELID_longValue;
                        for (int i = 0; i < CHANNELID_listValueCheck.Count; i++)
                        {
                            if (!long.TryParse(CHANNELID_listValueCheck[i], out CHANNELID_longValue))
                            {
                                ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                                return false;
                            }
                        }
                        break;
                    case S3102Consts.CON_RECORDREFERENCE_MULTITEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_SERIALID_MULTITEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                            return false;
                        }
                        string SERIALID_content = Value01;
                        List<string> SERIALID_listValueCheck = new List<string>();
                        string[] SERIALID_arrContent = SERIALID_content.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < SERIALID_arrContent.Length; i++)
                        {
                            SERIALID_listValueCheck.Add(SERIALID_arrContent[i]);
                        }
                        long SERIALID_longValue;
                        for (int i = 0; i < SERIALID_listValueCheck.Count; i++)
                        {
                            if (!long.TryParse(SERIALID_listValueCheck[i], out SERIALID_longValue))
                            {
                                ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "Input Invalid")));
                                return false;
                            }
                        }
                        break;
                    case S3102Consts.CON_MEMOAUTO_LIKETEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "不能为空")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_BOOKMARKTITLE_LIKETEXT:
                        if (string.IsNullOrWhiteSpace(Value01))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "不能为空")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_ISSCORED_CHECKSELECT:
                        int testTemp;
                        if (IsYesChecked == true)
                        {
                            if (!int.TryParse(Value01, out testTemp) || !int.TryParse(Value02, out testTemp))
                            {
                                ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N016", "输入超时")));
                                return false;
                            }
                            if (int.Parse(Value01) > int.Parse(Value02))
                            {
                                ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N0333", "第一个不能大于第二个")));
                                return false;
                            }
                        }
                        break;
                    case S3102Consts.CON_PROFESSIONAlLEVEL:
                    case S3102Consts.CON_SERVICEATTITUDE:
                        if (IsR1Checked == false && IsR2Checked == false && IsR3Checked == false)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中单选框")));
                            return false;
                        }
                        if (mComboOrgSkillGroup == null)
                        {
                            break;
                        }
                        if (mComboOrgSkillGroup.SelectedItem == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中机构")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_RECORDDURATIONEXCEPT:
                        if (IsR1Checked == false && IsR2Checked == false && IsR3Checked == false)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中单选框")));
                            return false;
                        }
                        if (mComboOrgSkillGroup == null)
                        {
                            break;
                        }
                        if (mComboOrgSkillGroup.SelectedItem == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中机构")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_CALLPEAK:
                        if (IsR1Checked == false && IsR2Checked == false && IsR3Checked == false && IsR4Checked == false)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中单选框")));
                            return false;
                        }
                        if (mComboOrgSkillGroup == null)
                        {
                            break;
                        }
                        if (mComboOrgSkillGroup.SelectedItem == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中机构")));
                            return false;
                        }
                        break;
                    case S3102Consts.CON_REPEATEDCALL:
                        if (IsR1Checked == false && IsR2Checked == false && IsR3Checked == false)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中单选框")));
                            return false;
                        }
                        if (mComboOrgSkillGroup == null)
                        {
                            break;
                        }
                        if (mComboOrgSkillGroup.SelectedItem == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3102N000000", "没有选中机构")));
                            return false;
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }//添加的检查函数px

        private void GetSubItemContent(string strContent)//这个函数是把比如再输入框里面输入了多个分机或坐席，然后就是把多个分机和坐席切割成单个记录存入到一个 string 的list里面
        {
            try
            {
                List<string> listValue01 = new List<string>();
                string[] arrContent = strContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrContent.Length; i++)
                {
                    listValue01.Add(arrContent[i]);
                }
                for (int i = 0; i < listValue01.Count; i++)
                {
                    var subItem = mListSubItems.FirstOrDefault(s => s.Value01 == listValue01[i]);
                    if (subItem == null)
                    {
                        //不在mListSubItems中，说明是手动在输入框中输入的
                        subItem = new QueryConditionSubItem();
                        if (ConditionResult != null)
                        {
                            subItem.QueryID = ConditionResult.QueryID;
                        }
                        subItem.ConditionItemID = ConditionItem.ID;
                        subItem.Value01 = listValue01[i];
                        //如果是分机或坐席，从管理对象列表中筛选出，同时获得Value02，Value03,Value04值
                        if (ConditionItem.Param == ConstValue.RESOURCE_EXTENSION.ToString()
                            || ConditionItem.Param == ConstValue.RESOURCE_AGENT.ToString()
                            || ConditionItem.Param == ConstValue.RESOURCE_REALEXT.ToString())
                        {
                            if (ListAllObjects != null)
                            {
                                var temp =
                                    ListAllObjects.FirstOrDefault(
                                        o => o.ObjType.ToString() == ConditionItem.Param && o.Name == listValue01[i]);
                                if (temp != null)
                                {
                                    subItem.Value02 = temp.ObjID.ToString();

                                    //如果是分机的话这个就是全名
                                    subItem.Value03 = temp.Description;

                                    //如果是分机的话那么就是IP
                                    subItem.Value04 = temp.FullName;
                                }
                            }
                        }
                        mListSubItems.Add(subItem);
                    }
                }
                for (int i = mListSubItems.Count - 1; i >= 0; i--)
                {
                    //移除
                    if (!listValue01.Contains(mListSubItems[i].Value01))
                    {
                        mListSubItems.Remove(mListSubItems[i]);
                    }
                }
                //重新设置编号
                for (int i = 0; i < mListSubItems.Count; i++)
                {
                    mListSubItems[i].Number = i;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public List<QueryConditionSubItem> GetSubItems()
        {
            return mListSubItems;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            Title = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", ConditionItem.ID), ConditionItem.Name);
            TxtLike = CurrentApp.GetLanguageInfo("31021120", "Like");
            TxtTimeType0 = CurrentApp.GetLanguageInfo("31021130", "Basic");
            TxtTimeType1 = CurrentApp.GetLanguageInfo("31021131", "From To");
            TxtTimeType2 = CurrentApp.GetLanguageInfo("31021132", "Custom");
            TxtButton = CurrentApp.GetLanguageInfo("31021140", "Add");

            TxtNoScored = CurrentApp.GetLanguageInfo("3102C3031401010000000017NO", "No");
            TxtYesScored = CurrentApp.GetLanguageInfo("3102C3031401010000000017YES", "Yes");

            if (ConditionItem.ID == S3102Consts.CON_SERVICEATTITUDE)
            {
                TxtR1 = CurrentApp.GetLanguageInfo("3102C3031401010000000022NICE", "NICE");
                TxtR2 = CurrentApp.GetLanguageInfo("3102C3031401010000000022BAD", "BAD");
                TxtR3 = CurrentApp.GetLanguageInfo("3102C3031401010000000022ALL", "ALL");
            }
            if (ConditionItem.ID == S3102Consts.CON_RECORDDURATIONEXCEPT)
            {
                TxtR1 = CurrentApp.GetLanguageInfo("3102C3031401010000000023NORMAL", "NORMAL");
                TxtR2 = CurrentApp.GetLanguageInfo("3102C3031401010000000023ABNORMAL", "ABNORMAL");
                TxtR3 = CurrentApp.GetLanguageInfo("3102C3031401010000000023ALL", "ALL");
            }
            if (ConditionItem.ID == S3102Consts.CON_REPEATEDCALL)
            {
                TxtR1 = CurrentApp.GetLanguageInfo("3102C3031401010000000028YES", "YES");
                TxtR2 = CurrentApp.GetLanguageInfo("3102C3031401010000000028NO", "NO");
                TxtR3 = CurrentApp.GetLanguageInfo("3102C3031401010000000028ALL", "ALL");
            }
            if (ConditionItem.ID == S3102Consts.CON_CALLPEAK)
            {
                TxtR1 = CurrentApp.GetLanguageInfo("3102C3031401010000000029HIGH", "HIGH");
                TxtR2 = CurrentApp.GetLanguageInfo("3102C3031401010000000029FLAT", "FLAT");
                TxtR3 = CurrentApp.GetLanguageInfo("3102C3031401010000000029LOW", "LOW");
                TxtR4 = CurrentApp.GetLanguageInfo("3102C3031401010000000029ALL", "ALL");
            }
            if (ConditionItem.ID == S3102Consts.CON_PROFESSIONAlLEVEL)
            {
                TxtR1 = CurrentApp.GetLanguageInfo("3102C3031401010000000031NICE", "NICE");
                TxtR2 = CurrentApp.GetLanguageInfo("3102C3031401010000000031BAD", "BAD");
                TxtR3 = CurrentApp.GetLanguageInfo("3102C3031401010000000031ALL", "ALL");
            }
            if (ConditionItem != null)
            {
                for (int i = 0; i < mListConditionItemSubItems.Count; i++)
                {
                    ConditionItemSubItem item = mListConditionItemSubItems[i];
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("3102C{0}{1}", ConditionItem.ID, item.Name),
                        item.Name);
                }
            }

            for (int i = 0; i < mListTimeTypeBasicItems.Count; i++)
            {
                ConditionItemSubItem item = mListTimeTypeBasicItems[i];
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP007{0}", item.Name), item.Name);
            }

            for (int i = 0; i < mListTimeTypeFromToItems.Count; i++)
            {
                ConditionItemSubItem item = mListTimeTypeFromToItems[i];
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP008{0}", item.Name), item.Name);
            }
        }

        public string PutTempData(List<string> Str)
        {
            try
            {
                Service11012Client ServiceClient = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(ServiceClient);
                WebRequest WReq = new WebRequest(); WReq.Session = CurrentApp.Session; WReq.Code = (int)RequestCode.WSInsertTempData;
                WReq.ListData.Add(string.Empty); WReq.ListData.Add(Str.Count.ToString());
                foreach (string str in Str)
                    WReq.ListData.Add(str);
                WebReturn WRet = ServiceClient.DoOperation(WReq);
                return WRet.Data;
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        #region AddValuesCommand

        private static RoutedUICommand mAddValuesCommand = new RoutedUICommand();

        public static RoutedUICommand AddValuesCommand
        {
            get { return mAddValuesCommand; }
        }

        #endregion

        public void DoOperation(List<ObjectItem> listItems)
        {
            //每次选择的时候将value1置为空,根据在树里面的选择的就给value01赋值
            Value01 = string.Empty;
            Value02 = string.Empty;
            if (listItems != null && listItems.Count > 0)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    Value01 += listItems[i].Description + ";";
                    if (i < listItems.Count - 1)
                    {
                        Value02 += listItems[i].ObjID + ",";
                    }
                    else
                    {
                        Value02 += listItems[i].ObjID;
                    }
                }
            }
        }

        //关键词查询就会用这个
        public void DoOperation_(List<ObjectItem> listItems)
        {
            //每次选择的时候将value1置为空,根据在树里面的选择的就给value01赋值
            Value01 = string.Empty;
            Value02 = string.Empty;
            if (listItems != null && listItems.Count > 0)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    Value01 += listItems[i].Description + ";";
                    if (i < listItems.Count - 1)
                    {
                        Value02 += "'" + listItems[i].Description + "'" + ",";
                    }
                    else
                    {
                        Value02 += "'" + listItems[i].Description + "'";
                    }
                }
            }
        }
    }
}
