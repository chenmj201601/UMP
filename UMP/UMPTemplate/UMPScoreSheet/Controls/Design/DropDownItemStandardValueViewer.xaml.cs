//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a7ff1d47-d6b2-4c7d-92aa-3a1878ecd33c
//        CLR Version:              4.0.30319.18444
//        Name:                     DropDownItemStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                DropDownItemStandardValueViewer
//
//        created by Charley at 2014/6/18 16:53:34
//        http://www.voicecyber.com
//
//======================================================================

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

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// DropDownItemStandardValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class DropDownItemStandardValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// ItemStandard
        /// </summary>
        public ItemStandard ItemStandard;
        /// <summary>
        /// ViewClassic
        /// </summary>
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// 设置信息
        /// </summary>
        public List<ScoreSetting> Settings { get; set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public List<ScoreLangauge> Languages { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        public int LangID { get; set; }

        private ObservableCollection<StandardItem> mListStandardItems;
      
        public DropDownItemStandardValueViewer()
        {
            InitializeComponent();

            mListStandardItems = new ObservableCollection<StandardItem>();
        }

        private void DropDownItemStandardValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ItemStandard;
            if (ItemStandard != null)
            {
                ItemStandard.OnPropertyChanged += ItemStandard_OnPropertyChanged;
            }
            Init();
        }

        void ItemStandard_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ValueItems")
            {
                Init();
            }
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (ItemStandard == null) { return; }
            InitStandardItems();
            ComboStandard.ItemsSource = mListStandardItems;
            if (ItemStandard.SelectValue != null)
            {
                StandardItem item = mListStandardItems.FirstOrDefault(i => i.ID == ItemStandard.SelectValue.ID);
                ComboStandard.SelectedItem = item;
            }
        }

        private void InitStandardItems()
        {
            mListStandardItems.Clear();
            if (ItemStandard != null)
            {
                for (int i = 0; i < ItemStandard.ValueItems.Count; i++)
                {
                    mListStandardItems.Add(ItemStandard.ValueItems[i]);
                }
            }
        }

        private void ComboStandard_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = ComboStandard.SelectedItem as StandardItem;
            if (selectItem != null)
            {
                ItemStandard.SelectValue = selectItem;
                ItemStandard.Score = selectItem.Value;

                PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                args.ScoreObject = ItemStandard;
                args.PropertyName = "Score";
                ItemStandard.PropertyChanged(ItemStandard, args);
            }
        }

        private void SubViewerLoaded()
        {
            if (ViewerLoaded != null)
            {
                ViewerLoaded();
            }
        }
    }
}
