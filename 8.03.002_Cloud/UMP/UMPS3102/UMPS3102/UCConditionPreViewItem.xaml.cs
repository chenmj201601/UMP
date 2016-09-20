//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    531060c1-bba2-4f8b-9fcd-6cc92ace8220
//        CLR Version:              4.0.30319.18444
//        Name:                     UCConditionPreViewItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCConditionPreViewItem
//
//        created by Charley at 2014/11/23 11:24:38
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS3102.Models;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102
{
    /// <summary>
    /// UCConditionPreViewItem.xaml 的交互逻辑
    /// </summary>
    public partial class UCConditionPreViewItem
    {
        //这个是查询条件设计时的一个窗口，不需要在条件里面输值的，和UCCustomConditionDesign是一起的；和UCConditionItem是一对的

        private ObservableCollection<ConditionItemSubItem> mListConditionItemSubItems;

        #region DependencyProperty

        public static readonly DependencyProperty ConditionItemItemProperty =
            DependencyProperty.Register("ConditionItemItem", typeof(ConditionItemItem), typeof(UCConditionPreViewItem), new PropertyMetadata(default(ConditionItemItem)));

        public ConditionItemItem ConditionItemItem
        {
            get { return (ConditionItemItem)GetValue(ConditionItemItemProperty); }
            set { SetValue(ConditionItemItemProperty, value); }//将新的值value 赋给ConditionItemItemProperty
        }

        public static readonly DependencyProperty ConditionItemProperty =
            DependencyProperty.Register("ConditionItem", typeof(CustomConditionItem), typeof(UCConditionPreViewItem), new PropertyMetadata(default(CustomConditionItem)));

        public CustomConditionItem ConditionItem
        {
            get { return (CustomConditionItem)GetValue(ConditionItemProperty); }
            set { SetValue(ConditionItemProperty, value); }
        }

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(CustomConditionItemType), typeof(UCConditionPreViewItem), new PropertyMetadata(default(CustomConditionItemType)));

        public CustomConditionItemType ItemType
        {
            get { return (CustomConditionItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(CustomConditionItemFormat), typeof(UCConditionPreViewItem), new PropertyMetadata(default(CustomConditionItemFormat)));

        public CustomConditionItemFormat Format
        {
            get { return (CustomConditionItemFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TxtLikeProperty =
            DependencyProperty.Register("TxtLike", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string TxtLike//大概就是近似查询吧
        {
            get { return (string)GetValue(TxtLikeProperty); }
            set { SetValue(TxtLikeProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool?), typeof(UCConditionPreViewItem), new PropertyMetadata(false));

        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty TxtTimeType0Property =
          DependencyProperty.Register("TxtTimeType0", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType0
        {
            get { return (string)GetValue(TxtTimeType0Property); }
            set { SetValue(TxtTimeType0Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType1Property =
            DependencyProperty.Register("TxtTimeType1", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType1
        {
            get { return (string)GetValue(TxtTimeType1Property); }
            set { SetValue(TxtTimeType1Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType2Property =
            DependencyProperty.Register("TxtTimeType2", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType2
        {
            get { return (string)GetValue(TxtTimeType2Property); }
            set { SetValue(TxtTimeType2Property, value); }
        }

        //我加的~~~~~ 和质检条件相关的名称
        //public static readonly DependencyProperty TxtIsScoredProperty =
        //    DependencyProperty.Register("TxtIsScored", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        //public string TxtIsScored
        //{
        //    get { return (string)GetValue(TxtIsScoredProperty); }
        //    set { SetValue(TxtIsScoredProperty, value); }
        //}

        //public static readonly DependencyProperty TxtScoreRangetProperty =
        //    DependencyProperty.Register("TxtScoreRange", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        //public string TxtScoreRange
        //{
        //    get { return (string)GetValue(TxtScoreRangetProperty); }
        //    set { SetValue(TxtScoreRangetProperty, value); }
        //}

        //public static readonly DependencyProperty TxtScoreSheetProperty =
        //    DependencyProperty.Register("TxtScoreSheet", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        //public string TxtScoreSheet
        //{
        //    get { return (string)GetValue(TxtScoreSheetProperty); }
        //    set { SetValue(TxtScoreSheetProperty, value); }
        //}

        //public static readonly DependencyProperty TxtInspectorProperty =
        //    DependencyProperty.Register("TxtInspector", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        //public string TxtInspector
        //{
        //    get { return (string)GetValue(TxtInspectorProperty); }
        //    set { SetValue(TxtInspectorProperty, value); }
        //}

        public static readonly DependencyProperty TxtButtonProperty =
        DependencyProperty.Register("TxtButton", typeof(string), typeof(UCConditionPreViewItem), new PropertyMetadata(default(string)));

        public string TxtButton
        {
            get { return (string)GetValue(TxtButtonProperty); }
            set { SetValue(TxtButtonProperty, value); }
        }

        #endregion


        #region Template

        private const string PART_ListSubItems = "PART_ListSubItems";//这个是一个ListBox的名字,也就是查询条件里面的呼入呼出选择,所以每次进入

        //private const string Quality_PART_IsScoreListSubItems = "Quality_PART_IsScoreListSubItems";//质检条件里面的CheckBox复选框所归属的ListBox
        //private const string Quality_PART_TitleIsScoredListSubItems = "Quality_PART_TitleIsScoredListSubItems";//是否评分的TextBlock
        //private const string Quality_PART_TitleScoreRange = "Quality_PART_TitleScoreRange";//评分范围的TextBlock
        //private const string Quality_PART_TitleScoreSheet = "Quality_PART_TitleScoreSheet";//评分表的TextBlock
        //private const string Quality_PART_TitleInspector = "Quality_PART_TitleInspector";//质检员的TextBlock


        private ListBox mListBoxSubItems;

        //private ListBox mQuality_PART_IsScoreListSubItems;

        //private TextBlock mQuality_PART_TitleIsScoredListSubItems;
        //private TextBlock mQuality_PART_TitleScoreRange;
        //private TextBlock mQuality_PART_TitleScoreSheet;
        //private TextBlock mQuality_PART_TitleInspector;





        public override void OnApplyTemplate()//这个是一个Template模板改变就会触发的事件（先这么理解）,不管什么模板发生了变化都会触发,模板就是UCConditionPreViewItem.xaml资源文件里面定义的template,一共有很多个,主要是找到我需要的名字的控件之后，然后将其ItemSource赋值
        {
            base.OnApplyTemplate();

            //要查找子集元素PART_ListSubItems的名称  
            mListBoxSubItems = GetTemplateChild(PART_ListSubItems) as ListBox;
            if (mListBoxSubItems != null)
            {
                mListBoxSubItems.ItemsSource = mListConditionItemSubItems;
            }

            //mQuality_PART_IsScoreListSubItems = GetTemplateChild(Quality_PART_IsScoreListSubItems) as ListBox;
            //if (mQuality_PART_IsScoreListSubItems!=null)
            //{
            //    mQuality_PART_IsScoreListSubItems.ItemsSource = mListConditionItemSubItems;
            //}
        }

        #endregion


        public UCConditionPreViewItem()
        {
            InitializeComponent();

            mListConditionItemSubItems = new ObservableCollection<ConditionItemSubItem>();

            Loaded += UCConditionPreViewItem_Loaded;
        }

        void UCConditionPreViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            if (ConditionItemItem == null)
            {
                return;
            }
            CurrentApp = ConditionItemItem.CurrentApp;
            ConditionItemItem.Apply();
            ConditionItem = ConditionItemItem.ConditionItem;
            if (ConditionItem == null)
            {
                return;
            }
            DataContext = ConditionItemItem;
            IsSelected = false;
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

                //    //新加的一个规格
                //case CustomConditionItemFormat.ThreeFour:
                //    Width = widthSize * 4;
                //    Height = heightSize * 3;
                //    break;
            }
            ItemType = ConditionItem.Type;//自定义条件类型
            Title = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", ConditionItem.ID), ConditionItem.Name);//条件的名字
            TxtLike = CurrentApp.GetLanguageInfo("31021120", "Like");//类似 有的条件是需要进行类似查询的

            TxtButton = CurrentApp.GetLanguageInfo("31021140", "Add");

            //这是三种时间的查询
            TxtTimeType0 = CurrentApp.GetLanguageInfo("31021130", "Basic");//常用时间   比如  今天  这个月   今年
            TxtTimeType1 = CurrentApp.GetLanguageInfo("31021131", "From To");//范围时间  比如  1天  2月  3年 等
            TxtTimeType2 = CurrentApp.GetLanguageInfo("31021132", "Custom");//自定义时间范围  比如  2015-01-01 00:00:00到 2016-01-01 00:00:00
            InitConditionItemSubItems();
        }

        private void InitConditionItemSubItems()
        {
            try
            {
                mListConditionItemSubItems.Clear();
                if (ConditionItem == null) { return; }
                
                string strInfos = ConditionItem.Param;//Param是参数,对应的T_31_040的C010字段
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
                    //所以从这里可以看出这个mListConditionItemSubItems是存T_31_040里面某些特殊元组的,也就是这个查询条件T_31_040的C010不为空,然后将参数写入里面
                    //比如,有个叫DirectionCheckList的条件,他的C010是“Callin;Callout”,所以里面就是这两个复选框
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region RemoveCommand

        private static RoutedUICommand mRemoveCommand = new RoutedUICommand();

        public static RoutedUICommand RemoveCommand
        {
            get 
            {
                return mRemoveCommand; 
            }
        }

        #endregion
    }
}
