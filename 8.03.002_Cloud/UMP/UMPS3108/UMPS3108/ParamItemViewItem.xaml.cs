using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Common31081;
using UMPS3108.Models;
using VoiceCyber.UMP.Communications;
using UMPS3108.Wcf31081;
using VoiceCyber.Common;

namespace UMPS3108
{
    /// <summary>
    /// ABCDItem.xaml 的交互逻辑
    /// </summary>
    public partial class ParamItemViewItem
    {
        private StatisticalParamItem ParamItem;
        //public CustomConditionItem ConditionItem;
        public StatisticalParamItemDetail ParamItemValue;

        private ObservableCollection<ParamItemSubItem> mListConditionItemSubItems;
        private ObservableCollection<ParamItemSubItem> mListIsAgentHanged;
        private ObservableCollection<ParamItemSubItem> mListConditionDayPice;


        #region DependencyProperty

        public static readonly DependencyProperty CombinedParamItemProperty =
            DependencyProperty.Register("CombinedParamItem", typeof(CombinedParamItemModel), typeof(ParamItemViewItem), new PropertyMetadata(default(CombinedParamItemModel)));

        public CombinedParamItemModel CombinedParamItem
        {
            get { return (CombinedParamItemModel)GetValue(CombinedParamItemProperty); }
            set { SetValue(CombinedParamItemProperty, value); }//将新的值value 赋给CombinedParamItemProperty
        }

        public static readonly DependencyProperty CombinedParamItemItemProperty =
            DependencyProperty.Register("CombinedParamItemItem", typeof(StatisticalParamItem), typeof(ParamItemViewItem), new PropertyMetadata(default(StatisticalParamItem)));

        public StatisticalParamItem CombinedParamItemItem
        {
            get { return (StatisticalParamItem)GetValue(CombinedParamItemItemProperty); }
            set { SetValue(CombinedParamItemItemProperty, value); }
        }

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(StatisticalParamItemType), typeof(ParamItemViewItem), new PropertyMetadata(default(StatisticalParamItemType)));

        public StatisticalParamItemType ItemType
        {
            get { return (StatisticalParamItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(CombStatiParaItemsFormat), typeof(ParamItemViewItem), new PropertyMetadata(default(CombStatiParaItemsFormat)));

        public CombStatiParaItemsFormat Format
        {
            get { return (CombStatiParaItemsFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TooltipProperty =
            DependencyProperty.Register("Tooltip", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Tooltip
        {
            get { return (string)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(false));

        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty TB1Property =
            DependencyProperty.Register("TB1", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string TB1
        {
            get { return (string)GetValue(TB1Property); }
            set { SetValue(TB1Property, value); }
        }


        public static readonly DependencyProperty TB2Property =
            DependencyProperty.Register("TB2", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string TB2
        {
            get { return (string)GetValue(TB2Property); }
            set { SetValue(TB2Property, value); }
        }


        public static readonly DependencyProperty TB3Property =
            DependencyProperty.Register("TB3", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string TB3
        {
            get { return (string)GetValue(TB3Property); }
            set { SetValue(TB3Property, value); }
        }

        public static readonly DependencyProperty TB4Property =
            DependencyProperty.Register("TB4", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string TB4
        {
            get { return (string)GetValue(TB4Property); }
            set { SetValue(TB4Property, value); }
        }

        public static readonly DependencyProperty Value01Property =
            DependencyProperty.Register("Value01", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Value01
        {
            get { return (string)GetValue(Value01Property); }
            set { SetValue(Value01Property, value); }
        }

        public static readonly DependencyProperty Value02Property =
            DependencyProperty.Register("Value02", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Value02
        {
            get { return (string)GetValue(Value02Property); }
            set { SetValue(Value02Property, value); }
        }


        #region  重复呼入里面的文本属性
        public static readonly DependencyProperty WorkDayProperty =
            DependencyProperty.Register("WorkDay", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string WorkDay
        {
            get { return (string)GetValue(WorkDayProperty); }
            set { SetValue(WorkDayProperty, value); }
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X1
        {
            get { return (string)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X2
        {
            get { return (string)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X3Property =
            DependencyProperty.Register("X3", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X3
        {
            get { return (string)GetValue(X3Property); }
            set { SetValue(X3Property, value); }
        }

        public static readonly DependencyProperty X4Property =
            DependencyProperty.Register("X4", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X4
        {
            get { return (string)GetValue(X4Property); }
            set { SetValue(X4Property, value); }
        }

        public static readonly DependencyProperty X5Property =
            DependencyProperty.Register("X5", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X5
        {
            get { return (string)GetValue(X5Property); }
            set { SetValue(X5Property, value); }
        }

        public static readonly DependencyProperty X6Property =
            DependencyProperty.Register("X6", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X6
        {
            get { return (string)GetValue(X6Property); }
            set { SetValue(X6Property, value); }
        }

        public static readonly DependencyProperty X7Property =
            DependencyProperty.Register("X7", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string X7
        {
            get { return (string)GetValue(X7Property); }
            set { SetValue(X7Property, value); }
        }

        public static readonly DependencyProperty EveryDayWorkTimeProperty =
            DependencyProperty.Register("EveryDayWorkTime", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string EveryDayWorkTime
        {
            get { return (string)GetValue(EveryDayWorkTimeProperty); }
            set { SetValue(EveryDayWorkTimeProperty, value); }
        }

        public static readonly DependencyProperty MorningProperty =
            DependencyProperty.Register("Morning", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Morning
        {
            get { return (string)GetValue(MorningProperty); }
            set { SetValue(MorningProperty, value); }
        }

        public static readonly DependencyProperty AfternoomProperty =
            DependencyProperty.Register("Afternoom", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Afternoom
        {
            get { return (string)GetValue(AfternoomProperty); }
            set { SetValue(AfternoomProperty, value); }
        }
        #endregion

        # region 重复呼入里的其他属性
        public static readonly DependencyProperty IsCheckedX1Property =
            DependencyProperty.Register("IsCheckedX1", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX1
        {
            get { return (bool)GetValue(IsCheckedX1Property); }
            set { SetValue(IsCheckedX1Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX2Property =
            DependencyProperty.Register("IsCheckedX2", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX2
        {
            get { return (bool)GetValue(IsCheckedX2Property); }
            set { SetValue(IsCheckedX2Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX3Property =
            DependencyProperty.Register("IsCheckedX3", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX3
        {
            get { return (bool)GetValue(IsCheckedX3Property); }
            set { SetValue(IsCheckedX3Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX4Property =
            DependencyProperty.Register("IsCheckedX4", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX4
        {
            get { return (bool)GetValue(IsCheckedX4Property); }
            set { SetValue(IsCheckedX4Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX5Property =
            DependencyProperty.Register("IsCheckedX5", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX5
        {
            get { return (bool)GetValue(IsCheckedX5Property); }
            set { SetValue(IsCheckedX5Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX6Property =
            DependencyProperty.Register("IsCheckedX6", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX6
        {
            get { return (bool)GetValue(IsCheckedX6Property); }
            set { SetValue(IsCheckedX6Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX7Property =
            DependencyProperty.Register("IsCheckedX7", typeof(bool?), typeof(ParamItemViewItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX7
        {
            get { return (bool)GetValue(IsCheckedX7Property); }
            set { SetValue(IsCheckedX7Property, value); }
        }

        public static readonly DependencyProperty Txt01ValueProperty =
           DependencyProperty.Register("Txt01Value", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Txt01Value
        {
            get { return (string)GetValue(Txt01ValueProperty); }
            set { SetValue(Txt01ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt02ValueProperty =
            DependencyProperty.Register("Txt02Value", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Txt02Value
        {
            get { return (string)GetValue(Txt02ValueProperty); }
            set { SetValue(Txt02ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt03ValueProperty =
   DependencyProperty.Register("Txt03Value", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Txt03Value
        {
            get { return (string)GetValue(Txt03ValueProperty); }
            set { SetValue(Txt03ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt04ValueProperty =
            DependencyProperty.Register("Txt04Value", typeof(string), typeof(ParamItemViewItem), new PropertyMetadata(default(string)));

        public string Txt04Value
        {
            get { return (string)GetValue(Txt04ValueProperty); }
            set { SetValue(Txt04ValueProperty, value); }
        }

        #endregion

        #endregion


        #region Template
        private const string ComboxStatisticTime = "ComboxStatisticTime";
        private const string PART_ComboIsAgentHanged = "PART_ComboIsAgentHanged";
        private const string ComboxDayPice = "ComboxDayPice";

        private ComboBox mComboIsAgentHanged;
        private ComboBox mComBoxSubItems;
        private ComboBox mComBoxDayPice;

        public override void OnApplyTemplate()//这个是一个Template模板改变就会触发的事件（先这么理解）,不管什么模板发生了变化都会触发,模板就是UCConditionPreViewItem.xaml资源文件里面定义的template,一共有很多个,主要是找到我需要的名字的控件之后，然后将其ItemSource赋值
        {
            base.OnApplyTemplate();

            //要查找子集元素PART_ListSubItems的名称  
            //mListBoxSubItems = GetTemplateChild(PART_ListSubItems) as ListBox;
            //if (mListBoxSubItems != null)
            //{
            //    mListBoxSubItems.ItemsSource = mListConditionItemSubItems;
            //}
            mComBoxSubItems = GetTemplateChild(ComboxStatisticTime) as ComboBox;
            if (mComBoxSubItems != null)
            {
                mComBoxSubItems.ItemsSource = mListConditionItemSubItems;
            }

            mComboIsAgentHanged = GetTemplateChild(PART_ComboIsAgentHanged) as ComboBox;
            if (mComboIsAgentHanged != null)
            {
                mComboIsAgentHanged.ItemsSource = mListIsAgentHanged;
            }

            mComBoxDayPice = GetTemplateChild(ComboxDayPice) as ComboBox;
            if (mComBoxDayPice != null)
            {
                mComBoxDayPice.ItemsSource = mListConditionDayPice;
            }

            SetValue();
        }
        #endregion


        public ParamItemViewItem()
        {
            InitializeComponent();

            mListConditionItemSubItems = new ObservableCollection<ParamItemSubItem>();
            ParamItemValue = new StatisticalParamItemDetail();
            mListIsAgentHanged = new ObservableCollection<ParamItemSubItem>();
            mListConditionDayPice = new ObservableCollection<ParamItemSubItem>();

            Loaded += UCConditionPreViewItem_Loaded;
        }

        private void UCConditionPreViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            InitParamItemTxt();
            InitValue();
            InitParamItemValue();
            InitAgentHangedItems();
            InitTimeTypeFromToItems();
            SetValue();
        }

        private void Init()
        {
            if (CombinedParamItem == null)
            {
                return;
            }
            CurrentApp = CombinedParamItem.CurrentApp;
            CombinedParamItem.Apply();
            CombinedParamItemItem = CombinedParamItem.ParamItem;
            if (CombinedParamItemItem == null)
            {
                return;
            }
            DataContext = CombinedParamItem;
            IsSelected = false;
            CombStatiParaItemsFormat format = CombinedParamItemItem.Formart;

            Format = format;
            if (CombinedParamItem.StatisticalParamItemID == "3140000000000000012")
            {
                Format = CombStatiParaItemsFormat.Unkown;
            }
            //if (CombinedParamItem.StatisticalParamItemID == "3140000000000000010" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000011" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000009" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000014" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000015" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000016" ||
            //    CombinedParamItem.StatisticalParamItemID == "3140000000000000017")
            //{
            //    Format = CombStatiParaItemsFormat.FourTwo;
            //}
            if (CombinedParamItem.StatisticalParamItemID == "3140000000000000014" ||
                CombinedParamItem.StatisticalParamItemID == "3140000000000000015" ||
                CombinedParamItem.StatisticalParamItemID == "3140000000000000016" ||
                CombinedParamItem.StatisticalParamItemID == "3140000000000000017")
            {
                Format = CombStatiParaItemsFormat.FourTwo;
            }
            if (CombinedParamItem.StatisticalParamItemID == "3140000000000000013")
            {
                Format = CombStatiParaItemsFormat.FiveTwo;
            }
            int widthSize = 200;
            int heightSize = 35;
            switch (Format)
            {
                case CombStatiParaItemsFormat.OneFour:
                    Width = widthSize * 4;
                    Height = heightSize;
                    break;
                case CombStatiParaItemsFormat.OneTwo:
                    Width = widthSize * 2;
                    Height = heightSize;
                    break;
                case CombStatiParaItemsFormat.OneOne:
                    Width = widthSize;
                    Height = heightSize;
                    break;
                case CombStatiParaItemsFormat.TwoFour:
                    Width = widthSize * 4;
                    Height = heightSize * 2;
                    break;
                case CombStatiParaItemsFormat.ThreeFour:
                    Width = widthSize * 4;
                    Height = heightSize * 3;
                    break;
                case CombStatiParaItemsFormat.Unkown:
                    Width = widthSize * 7;
                    Height = heightSize * 5;
                    break;
                case CombStatiParaItemsFormat.FourTwo:
                    Width = widthSize * 2;
                    Height = heightSize * 4;
                    break;
                case CombStatiParaItemsFormat.FiveTwo:
                    Width = widthSize * 2;
                    Height = heightSize * 5;
                    break;
            }
            ItemType = CombinedParamItemItem.Type;//自定义条件类型
            Title = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", CombinedParamItemItem.StatisticalParamItemID), CombinedParamItemItem.StatisticalParamItemName);//条件的名字
            Tooltip = CurrentApp.GetLanguageInfo(string.Format("3108T{0}", CombinedParamItemItem.StatisticalParamItemID), CombinedParamItemItem.StatisticalParamItemName);//条件的名字
        }

        //初始化各个参数小项里面的文本内容
        private void InitParamItemTxt()
        {
            TB1 = CurrentApp.GetLanguageInfo("3108011", "统计区段：");
            TB4 = CurrentApp.GetLanguageInfo("3108012", "时间单位：");
            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000010 ||
                CombinedParamItemItem.StatisticalParamItemID == 3140000000000000009)
            {
                TB1 = CurrentApp.GetLanguageInfo("3108001", "平均值的统计区段:");
                TB2 = CurrentApp.GetLanguageInfo("3108003", "平均值:");
            }

            else if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000011)
            {
                TB1 = CurrentApp.GetLanguageInfo("3108001", "平均值的统计区段:");
                TB2 = CurrentApp.GetLanguageInfo("3108004", "百分比(%):");
            }

            else
            {
                TB1 = CurrentApp.GetLanguageInfo("3108010", "标准差的统计区段:");
                TB2 = CurrentApp.GetLanguageInfo("3108002", "标准差：");
            }
            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000013)
            {
                TB1 = CurrentApp.GetLanguageInfo("3108011", "统计区段:");
                TB2 = CurrentApp.GetLanguageInfo("3108005", "一天分成多长片段");
                TB3 = CurrentApp.GetLanguageInfo("3108006", "设置高峰大于平均值的百分比：");
                InitDayPice();
            }
            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000012)
            {
                TB1 = CurrentApp.GetLanguageInfo("3108011", "统计区段:");
                TB2 = CurrentApp.GetLanguageInfo("3108007", "小时");
                TB3 = CurrentApp.GetLanguageInfo("3108008", "次");
                WorkDay = CurrentApp.GetLanguageInfo("310801P013", "工作日选择");
                EveryDayWorkTime = CurrentApp.GetLanguageInfo("310801P014", "每天工作时间");
                X1 = CurrentApp.GetLanguageInfo("310801P006", "星期一");
                X2 = CurrentApp.GetLanguageInfo("310801P007", "星期二");
                X3 = CurrentApp.GetLanguageInfo("310801P008", "星期三");
                X4 = CurrentApp.GetLanguageInfo("310801P009", "星期四");
                X5 = CurrentApp.GetLanguageInfo("310801P010", "星期五");
                X6 = CurrentApp.GetLanguageInfo("310801P011", "星期六");
                X7 = CurrentApp.GetLanguageInfo("310801P012", "星期日");
                Morning = CurrentApp.GetLanguageInfo("310801P015", "上午");
                Afternoom = CurrentApp.GetLanguageInfo("310801P016", "下午");
            }
        }

        private void InitValue()
        {
            if (CombinedParamItem == null) { return; }
            try
            {
                StatisticalParamItemType type = CombinedParamItem.Type;
                switch (type)
                {
                    case StatisticalParamItemType.SimpleText:
                        Value01 = string.Empty;
                        break;
                    case StatisticalParamItemType.DropDownEnum:
                        //InitAgentHangedItems();
                        //if (mComboIsAgentHanged != null)
                        //{
                        //    var tempIndex = mComboIsAgentHanged.SelectedIndex;
                        //    var temp = mListIsAgentHanged.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                        //    if (temp != null)
                        //    {
                        //        temp.IsChecked = true;
                        //        if (mComboIsAgentHanged != null)
                        //        {
                        //            mComboIsAgentHanged.SelectedItem = temp;
                        //        }
                        //    }
                        //}
                        Value01 = string.Empty;
                        break;
                    case StatisticalParamItemType.ComplexText:
                        //存写入T_31_044的值
                        Value01 = string.Empty;
                        //统计时长以及统计时长的单位
                        Value02 = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetValue()
        {
            if (CombinedParamItem == null) { return; }
            try
            {
                IsChecked = ParamItemValue.IsUsed == "Y";
                StatisticalParamItemType type = CombinedParamItem.Type;
                switch (type)
                {
                    case StatisticalParamItemType.SimpleText:
                        Value01 = ParamItemValue.Value1;
                        //Value01 = ParamItemValue.Value1;
                        break;
                    case StatisticalParamItemType.DropDownEnum:
                        //InitAgentHangedItems();
                        if (mComboIsAgentHanged != null)
                        {
                            var tempIndex = ParamItemValue.Value1;
                            if (tempIndex == string.Empty)
                            {
                                //因为在T_31_044表的 坐席先挂机字段存的值  是  1或者0  所以  写个2没有影响
                                tempIndex = "2";
                            }
                            var temp = mListIsAgentHanged.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                            if (temp != null)
                            {
                                temp.IsChecked = true;
                                if (mComboIsAgentHanged != null)
                                {
                                    mComboIsAgentHanged.SelectedItem = temp;
                                }
                            }
                        }
                        break;
                    case StatisticalParamItemType.ComplexText:
                        string[] tagetString = ChaiValue(ParamItemValue.Value1, ";");
                        //string otherValue = string.Empty;
                        string aaa = string.Empty;
                        string bbb = string.Empty;
                        if (tagetString != null)
                        {
                            for (int i = 0; i < tagetString.Length; i++)
                            {
                                if (i == 0)
                                {
                                    aaa = tagetString[i];
                                }
                                if (i == 1)
                                {
                                    bbb = tagetString[i];
                                }
                            }
                        }
                        //重复呼入次数有点特殊
                        if (CombinedParamItem.StatisticalParamItemID == "3140000000000000012")
                        {
                            Value01 = aaa;
                            Value02 = bbb;
                            string a = ParamItemValue.Value2;
                            string[] aa = ChaiValue(a, ",");
                            if (aa != null)
                            {
                                for (int i = 0; i < aa.Length; i++)
                                {
                                    switch (aa[i])
                                    {
                                        case "1":
                                            IsCheckedX1 = true;
                                            break;
                                        case "2":
                                            IsCheckedX2 = true;
                                            break;
                                        case "3":
                                            IsCheckedX3 = true;
                                            break;
                                        case "4":
                                            IsCheckedX4 = true;
                                            break;
                                        case "5":
                                            IsCheckedX5 = true;
                                            break;
                                        case "6":
                                            IsCheckedX6 = true;
                                            break;
                                        case "7":
                                            IsCheckedX7 = true;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            string b = ParamItemValue.Value3;
                            string[] bb = ChaiValue(b, ",");
                            if (bb == null)
                            {
                                return;
                            }
                            for (int i = 0; i < bb.Length; i++)
                            {
                                if (i == 0)
                                {
                                    Txt01Value = bb[i];
                                }
                                if (i == 1)
                                {
                                    Txt02Value = bb[i];
                                }
                                if (i == 2)
                                {
                                    Txt03Value = bb[i];
                                }
                                if (i == 3)
                                {
                                    Txt04Value = bb[i];
                                }
                            }
                        }
                        else
                        {
                            Value01 = aaa;
                            //时间数值
                            Value02 = ParamItemValue.Value2;
                            //时间单位
                            if (mComBoxSubItems != null)
                            {
                                //单位存在value3里面  时间单位1、年，2、月 ， 3、周，4、天，5、小时,6、分钟
                                var tempIndex = ParamItemValue.Value3;
                                //这里绝对不能用tempIndex == string.empty 因为  tempIndex是
                                if (tempIndex == null)
                                {
                                    tempIndex = string.Empty;
                                }
                                var temp = mListConditionItemSubItems.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                                if (temp != null)
                                {
                                    temp.IsChecked = true;
                                    if (mComBoxSubItems != null)
                                    {
                                        mComBoxSubItems.SelectedItem = temp;
                                    }
                                }
                            }
                            //时间切片  只在呼叫高峰里面
                            if (mComBoxDayPice != null)
                            {
                                var tempIndex = bbb;
                                if (tempIndex == null)
                                {
                                    tempIndex = string.Empty;
                                }
                                var temp = mListConditionDayPice.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                                if (temp != null)
                                {
                                    temp.IsChecked = true;
                                    if (mComBoxDayPice != null)
                                    {
                                        mComBoxDayPice.SelectedItem = temp;
                                    }
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

        public bool GetValue(StatisticalParamItem tempItem)
        {
            try
            {
                if (CombinedParamItem == null) { return false; }
                if (CheckInput() == false)
                {
                    return false;
                }
                if (tempItem.StatisticalParamItemID.ToString() == CombinedParamItem.StatisticalParamItemID)
                {
                    if (IsChecked == true)
                    {
                        ParamItemValue.IsUsed = "Y";//检查是否在前面勾上了checkbox.如果勾上了,那么这个条件就是已启用的.
                    }
                    else
                    {
                        ParamItemValue.IsUsed = "N";
                        return true;
                    }
                    StatisticalParamItemType type = CombinedParamItem.Type;
                    switch (type)
                    {
                        case StatisticalParamItemType.SimpleText:
                            ParamItemValue.Value1 = Value01;
                            break;
                        case StatisticalParamItemType.DropDownEnum:
                            if (mComboIsAgentHanged != null)
                            {
                                var temp = mComboIsAgentHanged.SelectedItem as ParamItemSubItem;
                                if (temp != null)
                                {
                                    ParamItemValue.Value1 = temp.Value.ToString();
                                    //ParamItemValue.Value2 = temp.Display;
                                }
                            }
                            break;
                        case StatisticalParamItemType.ComplexText:
                            if (CombinedParamItem.StatisticalParamItemID == "3140000000000000012")
                            {
                                ParamItemValue.Value1 = Value01 + ";" + Value02;
                                string strtemp = string.Empty;
                                strtemp = GetWorkDay(strtemp);
                                ParamItemValue.Value2 = strtemp;
                                ParamItemValue.Value3 = Txt01Value + "," + Txt02Value + "," + Txt03Value + "," + Txt04Value;
                            }
                            else
                            {
                                ParamItemValue.Value1 = Value01;
                                //需要统计的时长的值 
                                ParamItemValue.Value2 = Value02;
                                if (mComBoxSubItems != null)
                                {
                                    var temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                                    if (temp != null)
                                    {
                                        //统计时间的单位
                                        ParamItemValue.Value3 = temp.Value.ToString();
                                        //ParamItemValue.Value2 = temp.Display;
                                    }
                                }
                                if (mComBoxDayPice != null)
                                {
                                    var temp = mComBoxDayPice.SelectedItem as ParamItemSubItem;
                                    if (temp != null)
                                    {
                                        ParamItemValue.Value1 = ParamItemValue.Value1 + ";" + temp.Value.ToString();
                                    }
                                }
                            }
                            break;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void InitTimeTypeFromToItems()
        {
            try
            {
                mListConditionItemSubItems.Clear();
                ParamItemSubItem item = new ParamItemSubItem();
                //item.Name = "Year";
                //item.Value = 1;
                //item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C001"), "Year");
                //mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Month";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C002"), "Month");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Week";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C003"), "Week");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Day";
                item.Value = 4;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C004"), "Day");
                mListConditionItemSubItems.Add(item);
                //item = new ParamItemSubItem();
                //item.Name = "Hour";
                //item.Value = 5;
                //item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C005"), "Hour");
                //mListConditionItemSubItems.Add(item);
                //item = new ParamItemSubItem();
                //item.Name = "Minute";
                //item.Value = 6;
                //item.IsChecked = true;
                //item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C006"), "Minute");
                //mListConditionItemSubItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAgentHangedItems()
        {
            try
            {
                mListIsAgentHanged.Clear();
                //for (int i = 0; i < 2; i++)
                //{
                //    ParamItemSubItem tempitem = new ParamItemSubItem();
                //    if (i == 0)
                //    {
                //        tempitem.Name = "NO";
                //        tempitem.Display = CurrentApp.GetLanguageInfo("3108C008", "不是");//到时搞语言包
                //        tempitem.Value = i;
                //    }
                //    if (i == 1)
                //    {
                //        tempitem.Name = "Yes";
                //        tempitem.Display = CurrentApp.GetLanguageInfo("3108C007", "是");
                //        tempitem.Value = i;
                //    }
                //    mListIsAgentHanged.Add(tempitem);
                //}
                ParamItemSubItem tempitem = new ParamItemSubItem();
                tempitem.Name = "Yes";
                tempitem.Display = CurrentApp.GetLanguageInfo("3108C007", "是");
                tempitem.Value = 1;
                mListIsAgentHanged.Add(tempitem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitParamItemValue()
        {
            if (CombinedParamItem == null)
            {
                return;
            }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetAllParamItemsValue;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CombinedParamItem.ID.ToString());
                webRequest.ListData.Add(CombinedParamItem.StatisticalParamItemID.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    if (i == 0)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParamItemDetail>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        StatisticalParamItemDetail item = optReturn.Data as StatisticalParamItemDetail;//注意看这里的类型
                        if (item == null)
                        {
                            ShowException(string.Format("Fail. CustomConditionItem is null"));
                            return;
                        }
                        ParamItemValue = item;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void InitDayPice()
        {
            try
            {
                mListConditionDayPice.Clear();
                ParamItemSubItem item = new ParamItemSubItem();
                item.Name = "5";
                item.Value = 0;
                item.IsChecked = true;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C009"), "5 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "15";
                item.Value = 1;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C010"), "15 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "30";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C011"), "30 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "60";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C012"), "60 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "120";
                item.Value = 4;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C013"), "120 mins");
                mListConditionDayPice.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool CheckInput()
        {
            try
            {
                if (CombinedParamItem == null)
                {
                    return false;
                }
                string id = CombinedParamItem.StatisticalParamItemID;
                long ID = long.Parse(id);
                StatisticalParamItemType type = CombinedParamItem.Type;
                double a;
                DateTime datetime;
                ParamItemSubItem temp = new ParamItemSubItem();
                string strName = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", CombinedParamItem.StatisticalParamItemID), CombinedParamItem.Name);
                if (IsChecked != true) { return true; }
                switch (type)
                {
                    case StatisticalParamItemType.ComplexText:
                        if (id != S3108Consts.CON_RepeatedCallinTimes.ToString())
                        {
                            var temp_ = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                            if (temp_ == null)
                            {
                                return false;
                            }
                            if (temp_.Value == 2 && double.Parse(Value02) > 12)
                            {
                                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                                return false;
                            }
                            if (temp_.Value == 3 && double.Parse(Value02) > 52)
                            {
                                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                                return false;
                            }
                            if (temp_.Value == 4 && double.Parse(Value02) > 365)
                            {
                                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                                return false;
                            }
                        }
                        break;
                }
                switch (ID)
                {
                    case S3108Consts.CON_CollisionDuration:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_CollisionPercent:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_HoldDuration:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_HoldPercent:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_HoldTimes:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_TransferTimes:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_IsAgentHanged:
                        temp = mComboIsAgentHanged.SelectedItem as ParamItemSubItem;
                        if (temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_AfterDealDurationSec:
                        if (!double.TryParse(Value01, out a) || double.Parse(Value01) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_AfDeDurMoreAvaDeDurSec:
                        //temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            //!double.TryParse(Value02, out a) ||
                            //double.Parse(Value02) < 0 ||
                            temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_CallDurationCompareAva:
                        //temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0)
                        //!double.TryParse(Value02, out a) ||
                        //double.Parse(Value02) < 0 ||
                        //temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_CallDurationComparePec:
                        //temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0)
                        //||
                        //!double.TryParse(Value02, out a) ||
                        //double.Parse(Value02) < 0 ||
                        //temp == null
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_RepeatedCallinTimes:
                        if (!IsCheckedX1 && !IsCheckedX2 && !IsCheckedX3 && !IsCheckedX4 && !IsCheckedX5 && !IsCheckedX6 && !IsCheckedX7)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        if (!DateTime.TryParse(Txt01Value, out datetime) ||
                            !DateTime.TryParse(Txt02Value, out datetime) ||
                            !DateTime.TryParse(Txt02Value, out datetime) ||
                            !DateTime.TryParse(Txt02Value, out datetime))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        if (DateTime.Parse(Txt01Value) > DateTime.Parse(Txt02Value) || DateTime.Parse(Txt03Value) > DateTime.Parse(Txt04Value))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        if (DateTime.Parse(Txt01Value) < DateTime.Parse("00:00:00") ||
                            DateTime.Parse(Txt02Value) >= DateTime.Parse("12:00:00") ||
                            DateTime.Parse(Txt03Value) < DateTime.Parse("12:00:00") ||
                            DateTime.Parse(Txt04Value) > DateTime.Parse("23:59:59"))
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_CallPeak:
                        temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        var temp_ = mComBoxDayPice.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0 ||
                            temp == null ||
                            temp_ == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_ACSpeExceptProportion:
                        temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0 ||
                            temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_RecordDurationExcept:
                        temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0 ||
                            temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_AfterDealDurationExcept:
                        temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0 ||
                            temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
                            return false;
                        }
                        break;
                    case S3108Consts.CON_ExceptionScore:
                        temp = mComBoxSubItems.SelectedItem as ParamItemSubItem;
                        if (!double.TryParse(Value01, out a) ||
                            double.Parse(Value01) < 0 ||
                            !double.TryParse(Value02, out a) ||
                            double.Parse(Value02) < 0 ||
                            temp == null)
                        {
                            ShowException(string.Format("{1}\r\n\r\n{0}", strName, CurrentApp.GetLanguageInfo("3108T003", "Input Invalid")));
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
        }

        private string GetWorkDay(string temp)
        {
            if (IsCheckedX1 == true)
            {
                temp += "1" + ",";
            }
            if (IsCheckedX2 == true)
            {
                temp += "2" + ",";
            }
            if (IsCheckedX3 == true)
            {
                temp += "3" + ",";
            }
            if (IsCheckedX4 == true)
            {
                temp += "4" + ",";
            }
            if (IsCheckedX5 == true)
            {
                temp += "5" + ",";
            }
            if (IsCheckedX6 == true)
            {
                temp += "6" + ",";
            }
            if (IsCheckedX7 == true)
            {
                temp += "7" + ",";
            }
            return temp;
        }
        //拆Value temp是要拆的字符串 temp2是分隔的符号
        private string[] ChaiValue(string temp, string temp2)
        {
            string[] Chaitempstr;
            if (temp != string.Empty && temp != null)
            {
                Chaitempstr = temp.Split(temp2.ToCharArray());
                return Chaitempstr;
            }
            else
            {
                return null;
            }
        }
    }
}