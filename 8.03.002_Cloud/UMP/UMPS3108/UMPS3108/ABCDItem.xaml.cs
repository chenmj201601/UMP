using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VoiceCyber.UMP.Common31081;

namespace UMPS3108
{
    /// <summary>
    /// ABCDItem.xaml 的交互逻辑
    /// </summary>
    public partial class ABCDItem
    {
        private ObservableCollection<ParamItemSubItem> mListConditionItemSubItems;
        private ObservableCollection<ParamItemSubItem> mListConditionDayPice;
        private ObservableCollection<ParamItemSubItem> mListIsAgentHanged;
        private CombStatiParaItemsFormat format;

        public StatisticalParamItem ResultValues;


        private List<string> ListValue;
        private List<string> ListWeek;

        #region DependencyProperty

        public static readonly DependencyProperty ConditionItemItemProperty =
            DependencyProperty.Register("StatisticItem", typeof(StatisticalParamItem), typeof(ABCDItem), new PropertyMetadata(default(StatisticalParamItem)));

        public StatisticalParamItem StatisticItem
        {
            get { return (StatisticalParamItem)GetValue(ConditionItemItemProperty); }
            set { SetValue(ConditionItemItemProperty, value); }//将新的值value 赋给ConditionItemItemProperty
        }


        public static readonly DependencyProperty ConditionItemProperty =
           DependencyProperty.Register("StatisticItemModel", typeof(StatisticalParamItemModel), typeof(ABCDItem), new PropertyMetadata(default(StatisticalParamItemModel)));

        public StatisticalParamItemModel StatisticItemModel
        {
            get { return (StatisticalParamItemModel)GetValue(ConditionItemProperty); }
            set { SetValue(ConditionItemProperty, value); }//将新的值value 赋给ConditionItemItemProperty
        }


        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(StatisticalParamItemType), typeof(ABCDItem), new PropertyMetadata(default(StatisticalParamItemType)));

        public StatisticalParamItemType ItemType
        {
            get { return (StatisticalParamItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }


        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(CombStatiParaItemsFormat), typeof(ABCDItem), new PropertyMetadata(default(CombStatiParaItemsFormat)));

        public CombStatiParaItemsFormat Format
        {
            get { return (CombStatiParaItemsFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        public static readonly DependencyProperty TB1Property =
          DependencyProperty.Register("TB1", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string TB1
        {
            get { return (string)GetValue(TB1Property); }
            set { SetValue(TB1Property, value); }
        }


        public static readonly DependencyProperty TB2Property =
            DependencyProperty.Register("TB2", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string TB2
        {
            get { return (string)GetValue(TB2Property); }
            set { SetValue(TB2Property, value); }
        }


        public static readonly DependencyProperty TB3Property =
            DependencyProperty.Register("TB3", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string TB3
        {
            get { return (string)GetValue(TB3Property); }
            set { SetValue(TB3Property, value); }
        }


        public static readonly DependencyProperty Value01Property =
            DependencyProperty.Register("Value01", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Value01
        {
            get { return (string)GetValue(Value01Property); }
            set { SetValue(Value01Property, value); }
        }

        public static readonly DependencyProperty Value02Property =
            DependencyProperty.Register("Value02", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Value02
        {
            get { return (string)GetValue(Value02Property); }
            set { SetValue(Value02Property, value); }
        }


        #region  呼叫高峰里面的文本属性
        public static readonly DependencyProperty WorkDayProperty =
            DependencyProperty.Register("WorkDay", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string WorkDay
        {
            get { return (string)GetValue(WorkDayProperty); }
            set { SetValue(WorkDayProperty, value); }
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X1
        {
            get { return (string)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X2
        {
            get { return (string)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X3Property =
            DependencyProperty.Register("X3", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X3
        {
            get { return (string)GetValue(X3Property); }
            set { SetValue(X3Property, value); }
        }

        public static readonly DependencyProperty X4Property =
            DependencyProperty.Register("X4", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X4
        {
            get { return (string)GetValue(X4Property); }
            set { SetValue(X4Property, value); }
        }

        public static readonly DependencyProperty X5Property =
            DependencyProperty.Register("X5", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X5
        {
            get { return (string)GetValue(X5Property); }
            set { SetValue(X5Property, value); }
        }

        public static readonly DependencyProperty X6Property =
            DependencyProperty.Register("X6", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X6
        {
            get { return (string)GetValue(X6Property); }
            set { SetValue(X6Property, value); }
        }

        public static readonly DependencyProperty X7Property =
            DependencyProperty.Register("X7", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string X7
        {
            get { return (string)GetValue(X7Property); }
            set { SetValue(X7Property, value); }
        }

        public static readonly DependencyProperty EveryDayWorkTimeProperty =
            DependencyProperty.Register("EveryDayWorkTime", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string EveryDayWorkTime
        {
            get { return (string)GetValue(EveryDayWorkTimeProperty); }
            set { SetValue(EveryDayWorkTimeProperty, value); }
        }

        public static readonly DependencyProperty MorningProperty =
            DependencyProperty.Register("Morning", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Morning
        {
            get { return (string)GetValue(MorningProperty); }
            set { SetValue(MorningProperty, value); }
        }

        public static readonly DependencyProperty AfternoomProperty =
            DependencyProperty.Register("Afternoom", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Afternoom
        {
            get { return (string)GetValue(AfternoomProperty); }
            set { SetValue(AfternoomProperty, value); }
        }
        #endregion

        # region 呼叫高峰里的其他属性
        public static readonly DependencyProperty IsCheckedX1Property =
            DependencyProperty.Register("IsCheckedX1", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX1
        {
            get { return (bool)GetValue(IsCheckedX1Property); }
            set { SetValue(IsCheckedX1Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX2Property =
            DependencyProperty.Register("IsCheckedX2", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX2
        {
            get { return (bool)GetValue(IsCheckedX2Property); }
            set { SetValue(IsCheckedX2Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX3Property =
            DependencyProperty.Register("IsCheckedX3", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX3
        {
            get { return (bool)GetValue(IsCheckedX3Property); }
            set { SetValue(IsCheckedX3Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX4Property =
            DependencyProperty.Register("IsCheckedX4", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX4
        {
            get { return (bool)GetValue(IsCheckedX4Property); }
            set { SetValue(IsCheckedX4Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX5Property =
            DependencyProperty.Register("IsCheckedX5", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX5
        {
            get { return (bool)GetValue(IsCheckedX5Property); }
            set { SetValue(IsCheckedX5Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX6Property =
            DependencyProperty.Register("IsCheckedX6", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX6
        {
            get { return (bool)GetValue(IsCheckedX6Property); }
            set { SetValue(IsCheckedX6Property, value); }
        }

        public static readonly DependencyProperty IsCheckedX7Property =
            DependencyProperty.Register("IsCheckedX7", typeof(bool?), typeof(ABCDItem), new PropertyMetadata(default(bool)));

        public bool IsCheckedX7
        {
            get { return (bool)GetValue(IsCheckedX7Property); }
            set { SetValue(IsCheckedX7Property, value); }
        }

        public static readonly DependencyProperty Txt01ValueProperty =
           DependencyProperty.Register("Txt01Value", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Txt01Value
        {
            get { return (string)GetValue(Txt01ValueProperty); }
            set { SetValue(Txt01ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt02ValueProperty =
            DependencyProperty.Register("Txt02Value", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Txt02Value
        {
            get { return (string)GetValue(Txt02ValueProperty); }
            set { SetValue(Txt02ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt03ValueProperty =
   DependencyProperty.Register("Txt03Value", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Txt03Value
        {
            get { return (string)GetValue(Txt03ValueProperty); }
            set { SetValue(Txt03ValueProperty, value); }
        }

        public static readonly DependencyProperty Txt04ValueProperty =
            DependencyProperty.Register("Txt04Value", typeof(string), typeof(ABCDItem), new PropertyMetadata(default(string)));

        public string Txt04Value
        {
            get { return (string)GetValue(Txt04ValueProperty); }
            set { SetValue(Txt04ValueProperty, value); }
        }

        #endregion
        #endregion


        public ABCDItem()
        {
            InitializeComponent();
            //SetValue();

            mListConditionItemSubItems = new ObservableCollection<ParamItemSubItem>();
            mListConditionDayPice = new ObservableCollection<ParamItemSubItem>();
            mListIsAgentHanged = new ObservableCollection<ParamItemSubItem>();
            ComBoxDayPiceNum = 0;
            ComBoxSubItemNum = 0;
            ComBoxIsAgentHangedNum = 0;
            ListValue = new List<string>();
            ListWeek = new List<string>();

            Loaded += ABCDItem_Loaded;
        }

        #region Template

        private const string ComboxStatisticTime = "ComboxStatisticTime";//这个是一个ListBox的名字,也就是查询条件里面的呼入呼出选择,所以每次进入
        private const string ComboxDayPice = "ComboxDayPice";
        private const string ComboxIsAgentHanged = "PART_ComboIsAgentHanged";

        private ComboBox mComBoxSubItems;
        private ComboBox mComBoxDayPice;
        private ComboBox mComboIsAgentHanged;

        private int ComBoxSubItemNum;
        private int ComBoxDayPiceNum;
        private int ComBoxIsAgentHangedNum;

        public override void OnApplyTemplate()//这个是一个Template模板改变就会触发的事件（先这么理解）,不管什么模板发生了变化都会触发,模板就是UCConditionPreViewItem.xaml资源文件里面定义的template,一共有很多个,主要是找到我需要的名字的控件之后，然后将其ItemSource赋值
        {
            base.OnApplyTemplate();

            //要查找子集元素PART_ListSubItems的名称  
            mComBoxSubItems = GetTemplateChild(ComboxStatisticTime) as ComboBox;
            if (mComBoxSubItems != null)
            {
                mComBoxSubItems.ItemsSource = mListConditionItemSubItems;
            }

            mComBoxDayPice = GetTemplateChild(ComboxDayPice) as ComboBox;
            if (mComBoxDayPice != null)
            {
                mComBoxDayPice.ItemsSource = mListConditionDayPice;
            }

            mComboIsAgentHanged = GetTemplateChild(ComboxIsAgentHanged) as ComboBox;
            if (mComboIsAgentHanged != null)
            {
                mComboIsAgentHanged.ItemsSource = mListIsAgentHanged;
            }
            SetValue();
        }

        #endregion

        private void ABCDItem_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
        private void Init()
        {
            if (StatisticItemModel == null)
            {
                return;
            }
            CurrentApp = StatisticItemModel.CurrentApp;
            StatisticItemModel.Apply();
            //ResultValues = StatisticItem;
            //if (ConditionItem == null)
            //{
            //    return;
            //}
            DataContext = StatisticItemModel;
            IsChecked = false;
            format = StatisticItemModel.Formart;
            Format = format;
            if (StatisticItemModel.StatisticalParamItemID == 3140000000000000012)
            {
                Format = CombStatiParaItemsFormat.Unkown;
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
                //新加的一个规格
                case CombStatiParaItemsFormat.ThreeFour:
                    Width = widthSize * 4;
                    Height = heightSize * 3;
                    break;
                case CombStatiParaItemsFormat.Unkown:
                    Width = widthSize * 7;
                    Height = heightSize * 5;
                    break;
            }
            ItemType = StatisticItemModel.Type;//自定义条件类型
            Title = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", StatisticItemModel.StatisticalParamItemID), StatisticItemModel.StatisticalParamItemName);//条件的名字
            InitValue();
        }

        private void InitValue()
        {
            if (StatisticItemModel == null) { return; }
            try
            {
                StatisticalParamItemType type = StatisticItemModel.Type;

                switch (type)
                {
                    case StatisticalParamItemType.SimpleText:

                        break;
                    case StatisticalParamItemType.DropDownEnum:
                        InitAgentHangedItems();
                        if (mComboIsAgentHanged != null)
                        {
                            var tempIndex = mComboIsAgentHanged.SelectedIndex;

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
                        InitTimeTypeFromToItems();
                        if (StatisticItemModel.ValueType == "2")
                        {
                            TB1 = CurrentApp.GetLanguageInfo("3108001", "StatisticTime:");
                        }
                        else
                        {
                            TB1 = CurrentApp.GetLanguageInfo("3108009", "StatisticTime:");
                        }

                        if (format == CombStatiParaItemsFormat.TwoFour)
                        {
                            if (StatisticItemModel.StatisticalParamItemID == 3110000000000000010)
                            {
                                TB2 = CurrentApp.GetLanguageInfo("3108003", "平均值:");
                            }
                            else if (StatisticItemModel.StatisticalParamItemID == 3110000000000000011)
                            {
                                TB2 = CurrentApp.GetLanguageInfo("3108004", "百分比(%):");
                            }
                            else
                            {
                                TB2 = CurrentApp.GetLanguageInfo("3108002", "标准差：");
                            }
                        }
                        else if (format == CombStatiParaItemsFormat.ThreeFour)
                        {
                            TB2 = CurrentApp.GetLanguageInfo("3108005", "一天分成多长片段");
                            TB3 = CurrentApp.GetLanguageInfo("3108006", "设置高峰大于平均值的百分比：");
                            InitDayPice();
                        }
                        else
                        {
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
                        break;
                }
                SetValue();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetValue()
        {
            if (StatisticItemModel == null)
            {
                return;
            }
            try
            {
                if (StatisticItemModel.Value != null && StatisticItemModel.Value != "??")
                {
                    IsChecked = StatisticItemModel.IsUsed;
                    StatisticalParamItemType type = StatisticItemModel.Type;

                    switch (type)
                    {
                        case StatisticalParamItemType.ComplexText:
                            ValueHandle();
                            if ((format == CombStatiParaItemsFormat.TwoFour) && mComBoxSubItems != null)//比较、异常
                            {
                                Value01 = ListValue[0];
                                Value02 = StatisticItemModel.StatisticDuration;
                                mComBoxSubItems.SelectedIndex = ComBoxSubItemNum;
                            }
                            else if (format == CombStatiParaItemsFormat.ThreeFour && mComBoxSubItems != null && mComBoxDayPice != null)//呼叫高峰
                            {
                                Value01 = ListValue[0];
                                Value02 = StatisticItemModel.StatisticDuration;
                                mComBoxSubItems.SelectedIndex = ComBoxSubItemNum;
                                mComBoxDayPice.SelectedIndex = Convert.ToInt32(ListValue[1]);
                            }
                            else if (StatisticItemModel.StatisticalParamItemID == 3140000000000000012)//重呼
                            {
                                Value01 = ListValue[0];
                                //RepeatCallValues();
                                Value02 = ListValue[1];
                                if (ListValue.Count < 6)
                                {
                                    return;
                                }
                                for (int i = 2; i < ListValue.Count(); i++)
                                {
                                    if (i == 2)
                                    {
                                        Txt01Value = ListValue[i];
                                    }
                                    if (i == 3)
                                    {
                                        Txt02Value = ListValue[i];
                                    }
                                    if (i == 4)
                                    {
                                        Txt03Value = ListValue[i];
                                    }
                                    if (i == 5)
                                    {
                                        Txt04Value = ListValue[i];
                                    }
                                }
                                if (ListWeek != null)
                                {
                                    for (int i = 0; i < ListWeek.Count(); i++)
                                    {
                                        switch (ListWeek[i])
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
                            }
                            break;

                        case StatisticalParamItemType.DropDownEnum:
                            if (mComboIsAgentHanged != null)
                            {
                                string value = StatisticItemModel.Value;
                                value = value.Substring(0, value.Length - 2);
                                mComboIsAgentHanged.SelectedIndex = Convert.ToInt32(value);
                            }
                            break;
                        case StatisticalParamItemType.SimpleText:
                            string Tvalue = StatisticItemModel.Value;
                            Tvalue = Tvalue.Substring(0, Tvalue.Length - 2);
                            Value01 = Tvalue;
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //获得Value值,每个条件项目里面都有多个Value值
        public void GetValue()
        {
            try
            {
                TransModelToItem();
                if (StatisticItemModel == null)
                {
                    return;
                }
                ResultValues.IsUsed = IsChecked; //检查是否在前面勾上了checkbox.如果勾上了,那么这个条件就是要用做查询条件的
                if (!CheckValues())
                {
                    //ShowInformation(CurrentApp.GetLanguageInfo("3108T003", "输入内容格式不对"));
                    return;
                }
                if (IsChecked != true)
                {
                    ResultValues.Value = string.Empty;
                    return;
                }
                StatisticalParamItemType type = StatisticItemModel.Type;
                string getValues = string.Empty;
                switch (type)
                {
                    case StatisticalParamItemType.ComplexText:
                        if (format == CombStatiParaItemsFormat.TwoFour)
                        {
                            getValues = Value01;
                            ResultValues.StatisticDuration = Value02;
                            ResultValues.StatisticUnit = mComBoxSubItems.SelectedIndex + 1;
                        }
                        else if (format == CombStatiParaItemsFormat.ThreeFour)
                        {
                            getValues = string.Format("{0};", Value01);
                            ResultValues.StatisticDuration = Value02;
                            ResultValues.StatisticUnit = mComBoxSubItems.SelectedIndex + 1;
                            getValues += (mComBoxDayPice.SelectedIndex).ToString();
                        }
                        else
                        {
                            getValues = Value01 + ";";
                            getValues += Value02;
                        }
                        break;

                    case StatisticalParamItemType.DropDownEnum:
                        getValues = mComboIsAgentHanged.SelectedIndex.ToString();
                        break;

                    case StatisticalParamItemType.SimpleText:
                        getValues = Value01;
                        break;
                }
                //将list变回string
                ResultValues.Value = getValues;
                return;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return;
            }
        }

        private void InitTimeTypeFromToItems()
        {
            try
            {
                mListConditionItemSubItems.Clear();
                ParamItemSubItem item = new ParamItemSubItem();
                item.Name = "Year";
                item.Value = 0;
                item.Display = CurrentApp.GetLanguageInfo("3108C001", "Year");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Month";
                item.Value = 1;
                item.Display = CurrentApp.GetLanguageInfo("3108C002", "Month");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Week";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo("3108C003", "Week");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Day";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo("3108C004", "Day");
                mListConditionItemSubItems.Add(item);
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
                for (int i = 0; i < 2; i++)
                {
                    ParamItemSubItem tempitem = new ParamItemSubItem();
                    if (i == 0)
                    {
                        tempitem.Name = "NO";
                        tempitem.Display = CurrentApp.GetLanguageInfo("3108C008", "不是");//到时搞语言包
                        tempitem.Value = i;
                    }
                    if (i == 1)
                    {
                        tempitem.Name = "Yes";
                        tempitem.Display = CurrentApp.GetLanguageInfo("3108C007", "是");
                        tempitem.Value = i;
                    }
                    mListIsAgentHanged.Add(tempitem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Display = CurrentApp.GetLanguageInfo("3108C009", "5 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "15";
                item.Value = 1;
                item.Display = CurrentApp.GetLanguageInfo("3108C010", "15 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "30";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo("3108C011", "30 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "60";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo("3108C012", "60 mins");
                mListConditionDayPice.Add(item);
                item = new ParamItemSubItem();
                item.Name = "120";
                item.Value = 4;
                item.Display = CurrentApp.GetLanguageInfo("3108C013", "120 mins");
                mListConditionDayPice.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //拆分Value
        private void ValueHandle()
        {
            string values = StatisticItemModel.Value;
            //if (values != null && values != "")
            //{
            string[] TempList = values.Split("?".ToCharArray());
            if (TempList.Count() < 1) { return; }
            string[] LStrParam = TempList[0].Split(";".ToCharArray());
            for (int i = 0; i < LStrParam.Count(); i++)
            {
                string[] LStrParamItem = LStrParam[i].Split(",".ToCharArray());
                foreach (string str in LStrParamItem)
                {
                    ListValue.Add(str);
                }
            }
            if (TempList[1].Length != 0 && TempList[2].Length != 0)
            {
                string[] Week = TempList[1].Split(",".ToCharArray());
                for (int i = 0; i < Week.Count(); i++)
                {
                    ListWeek.Add(Week[i]);
                }
                string[] Time = TempList[2].Split(",".ToCharArray());
                for (int j = 0; j < Time.Count(); j++)
                {
                    ListValue.Add(Time[j]);
                }
            }
            if (StatisticItemModel.StatisticUnit > 0 && StatisticItemModel.StatisticUnit < 5)
            {
                ComBoxSubItemNum = StatisticItemModel.StatisticUnit - 1;
                //ListValue.Add(StatisticItemModel.StatisticDuration);
            }
        }

        private void RepeatCallValues()
        {
            string[] TempList = ListValue[1].Split("?".ToCharArray());
            ListValue.Add(TempList[0]);
            string[] Week = TempList[1].Split(",".ToCharArray());
            for (int i = 0; i < Week.Count(); i++)
            {
                ListWeek.Add(Week[i]);
            }
            string[] Time = TempList[2].Split(",".ToCharArray());
            for (int j = 0; j < Time.Count(); j++)
            {
                ListValue.Add(Time[j]);
            }
        }

        private void TransModelToItem()
        {
            ResultValues = new StatisticalParamItem();

            //ResultValues.IsCombine = StatisticItemModel.IsCombine;
            //ResultValues.Description = StatisticItemModel.Description;
            ResultValues.StatisticalParamID = StatisticItemModel.StatisticalParamID;
            ResultValues.StatisticalParamItemID = StatisticItemModel.StatisticalParamItemID;
            ResultValues.StatisticalParamItemName = StatisticItemModel.StatisticalParamItemName;
            ResultValues.TabIndex = StatisticItemModel.TabIndex;
            //ResultValues.TabName = StatisticItemModel.TabName;
            ResultValues.SortID = StatisticItemModel.SortID;
            ResultValues.Formart = StatisticItemModel.Formart;
            ResultValues.Type = StatisticItemModel.Type;
            //ResultValues.Value = StatisticItemModel.Value;
            ResultValues.ConditionItem = StatisticItem;

            //ResultValues.StatisticDuration = StatisticItemModel.StatisticDuration;
            //ResultValues.StatisticUnit = StatisticItemModel.StatisticUnit;
            //ResultValues.IsUsed = StatisticItemModel.IsUsed;
            //ResultValues.Value = StatisticItemModel.Value;
        }

        private bool CheckValues()
        {
            bool IsOK = true; int Num = 0;
            switch (StatisticItemModel.Type)
            {
                case StatisticalParamItemType.ComplexText:
                    if (IsChecked == true)
                    {
                        if (!int.TryParse(Value01, out Num))//|| !int.TryParse(Value02, out Num))
                        { IsOK = false; break; }
                        else
                        {
                            Num = int.Parse(Value01);
                            if (Num < 0)
                            { IsOK = false; break; }
                        }
                        if (StatisticItemModel.StatisticalParamItemID == 3140000000000000012)//repeatcall
                        {
                            Num = int.TryParse(Value02, out Num) ? Num : 0;
                            if (Num == 0)
                            { IsOK = false; break; }
                        }
                    }
                    break;

                case StatisticalParamItemType.SimpleText:
                    if (IsChecked == true)
                    {
                        if (!int.TryParse(Value01, out Num))
                        { IsOK = false; break; }
                        else
                        {
                            Num = int.Parse(Value01);
                            if (Num < 0)
                            { IsOK = false; break; }
                        }
                    }
                    break;
            }
            return IsOK;
        }
    }
}
