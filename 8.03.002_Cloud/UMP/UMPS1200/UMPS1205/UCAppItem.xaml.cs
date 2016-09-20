//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    728190fc-413f-4751-93d4-f9baec216cdf
//        CLR Version:              4.0.30319.42000
//        Name:                     UCAppItem
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205
//        File Name:                UCAppItem
//
//        created by Charley at 2016/3/1 13:29:40
//        http://www.voicecyber.com 
//
//======================================================================

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
using UMPS1205.Models;
using VoiceCyber.UMP.Common;

namespace UMPS1205
{
    /// <summary>
    /// UCAppItem.xaml 的交互逻辑
    /// </summary>
    public partial class UCAppItem
    {

        #region Members

        private bool mIsInited;

        #endregion


        public UCAppItem()
        {
            InitializeComponent();

            Loaded += UCAppItem_Loaded;
        }

        void UCAppItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                if (AppInfoItem == null) { return; }
                DataContext = AppInfoItem;
                ImageAppIcon.BeginInit();
                ImageAppIcon.Source = new BitmapImage(new Uri(AppInfoItem.Icon, UriKind.RelativeOrAbsolute));
                ImageAppIcon.EndInit();
                //TxtAppTitle.Text = AppInfoItem.Title;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public static readonly DependencyProperty AppInfoItemProperty =
            DependencyProperty.Register("AppInfoItem", typeof(AppInfoItem), typeof(UCAppItem), new PropertyMetadata(default(AppInfoItem)));

        public AppInfoItem AppInfoItem
        {
            get { return (AppInfoItem)GetValue(AppInfoItemProperty); }
            set { SetValue(AppInfoItemProperty, value); }
        }
    }
}
