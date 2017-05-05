//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    58f2d2e8-326b-4e5b-9d05-c54143ad0f16
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRightPanelView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCRightPanelView
//
//        created by Charley at 2016/8/2 11:14:10
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCRightPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class UCRightPanelView:IChildView
    {
        public List<InstallProduct> ListInstalledProducts;

        public DatabaseInfo DatabaseInfo;
        public UpdateInfo UpdateInfo;
        public InstallState InstallState;

        private bool mIsInited;

        public UCRightPanelView()
        {
            InitializeComponent();

            Loaded += UCRightPanelView_Loaded;
        }

        void UCRightPanelView_Loaded(object sender, RoutedEventArgs e)
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
                DocInformations.Blocks.Clear();
                if (UpdateInfo == null) { return; }
                if (InstallState == null) { return; }

                Paragraph para = new Paragraph();
                para.FontWeight = FontWeights.Bold;
                Run run = new Run();
                run.Text = App.GetLanguageInfo("T002","Prouducts installed on this machine");
                para.Inlines.Add(run);
                DocInformations.Blocks.Add(para);

                List list = new List();
                ListItem listItem;
                if (ListInstalledProducts == null
                    || ListInstalledProducts.Count <= 0)
                {
                    listItem = new ListItem();
                    para = new Paragraph();
                    run = new Run();
                    run.Text = App.GetLanguageInfo("T003","No product installed");
                    para.Inlines.Add(run);
                    listItem.Blocks.Add(para);
                    list.ListItems.Add(listItem);
                }
                else
                {
                    for (int i = 0; i < ListInstalledProducts.Count; i++)
                    {
                        var product = ListInstalledProducts[i];
                        string str = string.Format("{0} {1}", product.ProductName, product.Version);
                        listItem = new ListItem();
                        para = new Paragraph();
                        run = new Run();
                        run.Text = str;
                        para.Inlines.Add(run);
                        listItem.Blocks.Add(para);
                        list.ListItems.Add(listItem);
                    }
                }
                DocInformations.Blocks.Add(list);

                para = new Paragraph();
                run = new Run();
                run.Text = App.GetLanguageInfo("T004","Will update to version");
                run.FontWeight = FontWeights.Bold;
                para.Inlines.Add(run);
                DocInformations.Blocks.Add(para);

                list = new List();
                listItem = new ListItem();
                para = new Paragraph();
                run = new Run();
                run.Text = UpdateInfo.Version;
                para.Inlines.Add(run);
                listItem.Blocks.Add(para);
                list.ListItems.Add(listItem);
                DocInformations.Blocks.Add(list);

                para = new Paragraph();
                run = new Run();
                run.Text = App.GetLanguageInfo("T005","Database information");
                run.FontWeight = FontWeights.Bold;
                para.Inlines.Add(run);
                DocInformations.Blocks.Add(para);

                list = new List();
                if (!InstallState.IsDatabaseCreated)
                {
                    listItem = new ListItem();
                    para = new Paragraph();
                    run = new Run();
                    run.Text = App.GetLanguageInfo("T006","Database not created");
                    para.Inlines.Add(run);
                    listItem.Blocks.Add(para);
                    list.ListItems.Add(listItem);
                }
                else
                {
                    if (DatabaseInfo == null) { return; }

                    listItem = new ListItem();
                    para = new Paragraph();
                    run = new Run();
                    run.Text = string.Format("{0}-{1}:{2}-{3}-{4}",
                        DatabaseInfo.TypeID == 2 ?
                        "MSSQL" : DatabaseInfo.TypeID == 3 ?
                        "ORCL" : string.Empty,
                        DatabaseInfo.Host,
                        DatabaseInfo.Port,
                        DatabaseInfo.DBName,
                        DatabaseInfo.LoginName);
                    para.Inlines.Add(run);
                    listItem.Blocks.Add(para);
                    list.ListItems.Add(listItem);

                    if (!InstallState.IsLogined)
                    {
                        listItem = new ListItem();
                        para = new Paragraph();
                        run = new Run();
                        run.Text = App.GetLanguageInfo("T007","Please login before update");
                        para.Inlines.Add(run);
                        Hyperlink link = new Hyperlink();
                        link.Click += BtnLogin_Click;
                        run = new Run();
                        run.Text = App.GetLanguageInfo("B005","Login");
                        link.Inlines.Add(run);
                        para.Inlines.Add(link);
                        listItem.Blocks.Add(para);
                        list.ListItems.Add(listItem);
                    }
                    else
                    {
                        listItem = new ListItem();
                        para = new Paragraph();
                        run = new Run();
                        run.Text = string.Format(App.GetLanguageInfo("T008", "{0} login successful"), InstallState.LoginAccount);
                        para.Inlines.Add(run);
                        listItem.Blocks.Add(para);
                        list.ListItems.Add(listItem);
                    }
                }
                DocInformations.Blocks.Add(list);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginWindow login = new LoginWindow();
                login.DatabaseInfo = DatabaseInfo;
                var result = login.ShowDialog();
                if (result != true) { return; }
                UserInfo userInfo = login.UserInfo;
                if (userInfo == null) { return;}
                if (InstallState == null) { return;}
                InstallState.IsLogined = true;
                InstallState.LoginAccount = userInfo.Account;
                Init();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


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
                Init();
            }
            catch { }
        }

        #endregion

    }
}
