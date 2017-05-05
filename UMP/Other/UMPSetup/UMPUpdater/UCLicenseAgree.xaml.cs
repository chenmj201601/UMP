//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    03da5dde-36fe-41ee-81dd-a56ebe2b7100
//        CLR Version:              4.0.30319.18408
//        Name:                     UCLicenseAgree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCLicenseAgree
//
//        created by Charley at 2016/8/7 18:36:30
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Documents;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCLicenseAgree.xaml 的交互逻辑
    /// </summary>
    public partial class UCLicenseAgree : ILeftView
    {

        #region Members

        public UpdateWindow PageParent { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
        public InstallState InstallState { get; set; }

        private bool mIsInited;

        #endregion


        public UCLicenseAgree()
        {
            InitializeComponent();

            Loaded += UCLicenseAgree_Loaded;
            BtnPrevious.Click += BtnPrevious_Click;
            BtnNext.Click += BtnNext_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCLicenseAgree_Loaded(object sender, RoutedEventArgs e)
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
                int langID = App.LangID;
                string strName = string.Format("License_{0}.xaml", langID);
                string strUri = string.Format("/UMPUpdater;component/Languages/{0}", strName);
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(strUri, UriKind.RelativeOrAbsolute);
                var doc = resource["DocLicense"] as FlowDocument;
                if (doc != null)
                {
                    DocViewer.Document = doc;
                }

                ChangeLanguage();
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

        void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbAgree.IsChecked != true)
                {
                    ShowException(App.GetLanguageInfo("N003", string.Format("Please agree the license agreement before update!")));
                    return;
                }
                if (PageParent != null)
                {
                    PageParent.ToNext();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                int langID = App.LangID;
                string strName = string.Format("License_{0}.xaml", langID);
                string strUri = string.Format("/UMPUpdater;component/Languages/{0}", strName);
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(strUri, UriKind.RelativeOrAbsolute);
                var doc = resource["DocLicense"] as FlowDocument;
                if (doc != null)
                {
                    DocViewer.Document = doc;
                }

                CbAgree.Content = App.GetLanguageInfo("T009", string.Format("I accept the items of the license agreement"));

                BtnPrevious.Content = App.GetLanguageInfo("B001", "Previous");
                BtnNext.Content = App.GetLanguageInfo("B002", "Next");
                BtnClose.Content = App.GetLanguageInfo("B003", "Close");
            }
            catch { }
        }

        #endregion
    }
}
