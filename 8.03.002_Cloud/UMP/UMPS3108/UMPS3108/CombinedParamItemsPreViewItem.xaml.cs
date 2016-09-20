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
    /// CombinedParamItemsPreViewItem.xaml 的交互逻辑
    /// </summary>
    public partial class CombinedParamItemsPreViewItem
    {
        //private ObservableCollection<ConditionItemSubItem> mListConditionItemSubItems;

        #region DependencyProperty

        public static readonly DependencyProperty CombinedParamItemProperty =
            DependencyProperty.Register("CombinedParamItem", typeof(CombinedParamItemModel), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(CombinedParamItemModel)));

        public CombinedParamItemModel CombinedParamItem
        {
            get { return (CombinedParamItemModel)GetValue(CombinedParamItemProperty); }
            set { SetValue(CombinedParamItemProperty, value); }//将新的值value 赋给CombinedParamItemProperty
        }

        public static readonly DependencyProperty CombinedParamItemItemProperty =
            DependencyProperty.Register("CombinedParamItemItem", typeof(StatisticalParamItem), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(StatisticalParamItem)));

        public StatisticalParamItem CombinedParamItemItem
        {
            get { return (StatisticalParamItem)GetValue(CombinedParamItemItemProperty); }
            set { SetValue(CombinedParamItemItemProperty, value); }
        }


        public static readonly DependencyProperty ItemTypeProperty =
           DependencyProperty.Register("ItemType", typeof(StatisticalParamItemType), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(StatisticalParamItemType)));

        public StatisticalParamItemType ItemType
        {
            get { return (StatisticalParamItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(CombStatiParaItemsFormat), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(CombStatiParaItemsFormat)));

        public CombStatiParaItemsFormat Format
        {
            get { return (CombStatiParaItemsFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TxtLikeProperty =
            DependencyProperty.Register("TxtLike", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TxtLike//大概就是近似查询吧
        {
            get { return (string)GetValue(TxtLikeProperty); }
            set { SetValue(TxtLikeProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool?), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(false));

        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty TxtTimeType0Property =
          DependencyProperty.Register("TxtTimeType0", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType0
        {
            get { return (string)GetValue(TxtTimeType0Property); }
            set { SetValue(TxtTimeType0Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType1Property =
            DependencyProperty.Register("TxtTimeType1", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType1
        {
            get { return (string)GetValue(TxtTimeType1Property); }
            set { SetValue(TxtTimeType1Property, value); }
        }

        public static readonly DependencyProperty TxtTimeType2Property =
            DependencyProperty.Register("TxtTimeType2", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TxtTimeType2
        {
            get { return (string)GetValue(TxtTimeType2Property); }
            set { SetValue(TxtTimeType2Property, value); }
        }


        public static readonly DependencyProperty TB1Property =
            DependencyProperty.Register("TB1", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TB1
        {
            get { return (string)GetValue(TB1Property); }
            set { SetValue(TB1Property, value); }
        }


        public static readonly DependencyProperty TB2Property =
            DependencyProperty.Register("TB2", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TB2
        {
            get { return (string)GetValue(TB2Property); }
            set { SetValue(TB2Property, value); }
        }


        public static readonly DependencyProperty TB4Property =
            DependencyProperty.Register("TB4", typeof(string), typeof(CombinedParamItemsPreViewItem), new PropertyMetadata(default(string)));

        public string TB4
        {
            get { return (string)GetValue(TB4Property); }
            set { SetValue(TB4Property, value); }
        }

        #endregion


        #region template
        private const string PART_ListSubItems = "PART_ListSubItems";//这个是一个ListBox的名字,也就是查询条件里面的呼入呼出选择,所以每次

        private ListBox mListBoxSubItems;


        public override void OnApplyTemplate()//这个是一个Template模板改变就会触发的事件（先这么理解）,不管什么模板发生了变化都会触发,模板就是UCConditionPreViewItem.xaml资源文件里面定义的template,一共有很多个,主要是找到我需要的名字的控件之后，然后将其ItemSource赋值
        {
            base.OnApplyTemplate();

            //要查找子集元素PART_ListSubItems的名称  
            //mListBoxSubItems = GetTemplateChild(PART_ListSubItems) as ListBox;
            //if (mListBoxSubItems != null)
            //{
            //    mListBoxSubItems.ItemsSource = mListConditionItemSubItems;
            //}

        }
        #endregion

        public CombinedParamItemsPreViewItem()
        {
            InitializeComponent();

            Loaded += UCConditionPreViewItem_Loaded;

        }


        private void UCConditionPreViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            if (CombinedParamItem == null)
            {
                return;
            }
            CombinedParamItem.Apply();
            CurrentApp = CombinedParamItem.CurrentApp;
            CombinedParamItemItem = CombinedParamItem.ParamItem;
            if (CombinedParamItemItem == null)
            {
                return;
            }
            DataContext = CombinedParamItem;
            IsSelected = false;
            CombStatiParaItemsFormat format = CombinedParamItemItem.Formart;
            Format = format;
            int widthSize = 150;
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
                case CombStatiParaItemsFormat.FourTwo:
                    Width = widthSize * 2;
                    Height = heightSize * 4;
                    break;
            }
            ItemType = CombinedParamItemItem.Type;//自定义条件类型
            Title = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", CombinedParamItemItem.StatisticalParamItemID), CombinedParamItemItem.StatisticalParamItemName);//条件的名字
            InitParamItemTxt();
        }

        private void InitParamItemTxt()
        {
            TB4 = CurrentApp.GetLanguageInfo("3108012", "时间单位：");
            TB1 = CurrentApp.GetLanguageInfo("3108001", "Statistical Durition");
            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000009)
            {
                TB2 = CurrentApp.GetLanguageInfo("3108003", "Avarage Value");
            }

            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000010)
            {
                TB2 = CurrentApp.GetLanguageInfo("3108003", "Avarage Value");
            }

            if (CombinedParamItemItem.StatisticalParamItemID == 3140000000000000011)
            {
                TB2 = CurrentApp.GetLanguageInfo("3108004", "Percent(%):");
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

