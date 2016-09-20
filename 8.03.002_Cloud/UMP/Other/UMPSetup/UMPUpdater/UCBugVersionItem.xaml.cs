//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fdedcc13-9e46-4ca9-8308-c7221882f71f
//        CLR Version:              4.0.30319.18408
//        Name:                     UCBugVersionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCBugVersionItem
//
//        created by Charley at 2016/8/7 17:02:27
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using UMPUpdater.Models;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCBugVersionItem.xaml 的交互逻辑
    /// </summary>
    public partial class UCBugVersionItem
    {

        public UCBugVersionItem()
        {
            InitializeComponent();

            Loaded += UCBugVersionItem_Loaded;
        }

        void UCBugVersionItem_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                if (BugVersionItem == null) { return;}
                BugVersionItem.Viewer = this;
                CreateBugInfoList();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateBugInfoList()
        {
            try
            {
                if (BugVersionItem == null) { return; }
                DocBugInfoList.Blocks.Clear();
                Paragraph para;
                Run run;
                List list;
                ListItem listItem;
                var groups = BugVersionItem.ListBugInfoItems.GroupBy(b => b.ModuleID);
                foreach (var group in groups)
                {
                    int moduleID = group.Key;
                    para = new Paragraph();
                    para.FontWeight = FontWeights.Bold;
                    run = new Run();
                    run.Text = App.GetLanguageInfo(string.Format("M{0}", moduleID.ToString("0000")), moduleID.ToString());
                    para.Inlines.Add(run);
                    DocBugInfoList.Blocks.Add(para);
                    list = new List();
                    foreach (var bug in group)
                    {
                        var info = bug.Info as UpdateModule;
                        if (info == null) { continue;}
                        listItem = new ListItem();
                        para = new Paragraph();
                        run = new Run();
                        run.Text = App.GetLanguageInfo(string.Format("MC{0}", info.SerialNo), info.Content);
                        para.Inlines.Add(run);
                        //run = new Run();
                        //run.Text = string.Format("[{0}]", bug.SerialNo);
                        //para.Inlines.Add(run);
                        listItem.Blocks.Add(para);
                        list.ListItems.Add(listItem);
                    }
                    DocBugInfoList.Blocks.Add(list);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region BugVersionItem

        public static readonly DependencyProperty BugVersionItemProperty =
            DependencyProperty.Register("BugVersionItem", typeof (BugVersionItem), typeof (UCBugVersionItem), new PropertyMetadata(default(BugVersionItem),OnBugVersionItemChanged));

        public BugVersionItem BugVersionItem
        {
            get { return (BugVersionItem) GetValue(BugVersionItemProperty); }
            set { SetValue(BugVersionItemProperty, value); }
        }

        private static void OnBugVersionItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (UCBugVersionItem) d;
            if (uc != null)
            {
                uc.OnBugVersionItemChanged((BugVersionItem) e.OldValue, (BugVersionItem) e.NewValue);
            }
        }

        public void OnBugVersionItemChanged(BugVersionItem oldValue, BugVersionItem newValue)
        {
            if (newValue != null)
            {
                CreateBugInfoList();
            }
        }

        #endregion


        #region Basics

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region ChangeLanguage

        public void ChangeLanguage()
        {
            try
            {
                CreateBugInfoList();
            }
            catch { }
        }

        #endregion

    }
}
