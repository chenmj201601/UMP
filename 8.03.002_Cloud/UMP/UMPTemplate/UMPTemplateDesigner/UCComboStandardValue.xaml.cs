//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    255972af-646c-4479-b7cf-cb64b8775f04
//        CLR Version:              4.0.30319.18444
//        Name:                     UCComboBoxStandardValue
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCComboBoxStandardValue
//
//        created by Charley at 2014/6/17 9:46:50
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
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCComboBoxStandardValue.xaml 的交互逻辑
    /// </summary>
    public partial class UCComboStandardValue
    {
        public ItemStandard ItemStandard;

        public UCComboStandardValue()
        {
            InitializeComponent();
        }

        private void UCComboBoxStandardValue_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ItemStandard;

            Init();
        }

        public void Init()
        {
            if (ItemStandard == null) { return; }

            ComboStandard.ItemsSource = ItemStandard.ValueItems;
            if (ItemStandard.SelectValue != null)
            {
                StandardItem item = ItemStandard.ValueItems.FirstOrDefault(i => i.ID == ItemStandard.SelectValue.ID);
                ComboStandard.SelectedItem = item;
            }
        }

        private void ComboStandard_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = ComboStandard.SelectedItem as StandardItem;
            if (selectItem != null)
            {
                ItemStandard.SelectValue = selectItem;
                ItemStandard.Score = selectItem.Value;
            }
        }
    }
}
