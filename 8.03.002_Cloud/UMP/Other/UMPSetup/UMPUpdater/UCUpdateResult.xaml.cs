//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f8241e4-7432-4647-b34f-3c2b8f0a6f3d
//        CLR Version:              4.0.30319.18408
//        Name:                     UCUpdateResult
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCUpdateResult
//
//        created by Charley at 2016/8/8 16:06:00
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Documents;
using VoiceCyber.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCUpdateResult.xaml 的交互逻辑
    /// </summary>
    public partial class UCUpdateResult : ILeftView
    {

        #region Members

        public UpdateWindow PageParent { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
        public InstallState InstallState { get; set; }

        private bool mIsInited;

        #endregion


        public UCUpdateResult()
        {
            InitializeComponent();

            Loaded += UCUpdateResult_Loaded;
            BtnPrevious.Click += BtnPrevious_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCUpdateResult_Loaded(object sender, RoutedEventArgs e)
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
                var result = !InstallState.IsOptFail;
                if (result)
                {
                    TxtUpdateResult.Text = string.Format("UMP update successful!");
                }
                else
                {
                    TxtUpdateResult.Text = string.Format("Sorry, update fail.");
                }
                PanelSuccessResult.Visibility = result ? Visibility.Visible : Visibility.Collapsed;
                PanelFailResult.Visibility = !result ? Visibility.Visible : Visibility.Collapsed;

             
                Paragraph para;
                Run run;
                List list;
                ListItem listItem;

                DocSuccessResult.Blocks.Clear();

                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("For the next, you may do some operation below:");
                para.Inlines.Add(run);
                DocSuccessResult.Blocks.Add(para);

                list = new List();
                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Lanch MAMT and bind UMP's Website");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Lanch UMP by UMP's shortcut or open index page of UMP website");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Open the update log and refere to the updating detail informations");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                DocSuccessResult.Blocks.Add(list);


                DocFailResult.Blocks.Clear();

                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Some tips below may help you:");
                para.Inlines.Add(run);
                DocFailResult.Blocks.Add(para);

                list = new List();
                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Open the update log and refer to updating detail information");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Confirm the UMP's database service started");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = string.Format("Confirm UMPService service started");
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);

                DocFailResult.Blocks.Add(list);

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Button Event Handler

        void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToPrevious();
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToClose();
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

            }
            catch { }
        }

        #endregion

    }
}
