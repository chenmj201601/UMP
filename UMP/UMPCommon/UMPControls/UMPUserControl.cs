//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    336bb521-b436-4915-b663-e330dcd1a77b
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPUserControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPUserControl
//
//        created by Charley at 2014/8/20 21:15:14
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 表示一个用户控件，用来代替UserControl
    /// </summary>
    public class UMPUserControl : UserControl, IUMPPage
    {
        public static readonly DependencyProperty PageNameProperty =
            DependencyProperty.Register("PageName", typeof(string), typeof(UMPUserControl), new PropertyMetadata(default(string)));

        public string PageName
        {
            get { return (string)GetValue(PageNameProperty); }
            set { SetValue(PageNameProperty, value); }
        }

        public static readonly DependencyProperty LangTypeInfoProperty =
            DependencyProperty.Register("LangTypeInfo", typeof (LangTypeInfo), typeof (UMPUserControl), new PropertyMetadata(default(LangTypeInfo)));

        public LangTypeInfo LangTypeInfo
        {
            get { return (LangTypeInfo) GetValue(LangTypeInfoProperty); }
            set { SetValue(LangTypeInfoProperty, value); }
        }

        public static readonly DependencyProperty StylePathProperty =
            DependencyProperty.Register("StylePath", typeof(string), typeof(UMPUserControl), new PropertyMetadata(default(string)));

        public string StylePath
        {
            get { return (string)GetValue(StylePathProperty); }
            set { SetValue(StylePathProperty, value); }
        }

        public static readonly DependencyProperty ThemeInfoProperty =
            DependencyProperty.Register("ThemeInfo", typeof(ThemeInfo), typeof(UMPUserControl), new PropertyMetadata(default(ThemeInfo)));

        public ThemeInfo ThemeInfo
        {
            get { return (ThemeInfo)GetValue(ThemeInfoProperty); }
            set { SetValue(ThemeInfoProperty, value); }
        }

        public static readonly DependencyProperty AppServerInfoProperty =
            DependencyProperty.Register("AppServerInfo", typeof (AppServerInfo), typeof (UMPUserControl), new PropertyMetadata(default(AppServerInfo)));

        public AppServerInfo AppServerInfo
        {
            get { return (AppServerInfo) GetValue(AppServerInfoProperty); }
            set { SetValue(AppServerInfoProperty, value); }
        }

        public UMPApp CurrentApp;

        public virtual void ChangeTheme()
        {
            //To Change Theme
            if (ThemeInfo != null)
            {
                try
                {
                    Resources.MergedDictionaries.Clear();

                    //ResourceDictionary resource;
                    //string uri;
                    //bool bControl = false;

                    //if (AppServerInfo != null)
                    //{
                    //    //优先从服务器获取资源文件
                    //    try
                    //    {
                    //        uri = string.Format("{0}://{1}:{2}/Themes/{3}/Control.xaml",
                    //           AppServerInfo.Protocol,
                    //           AppServerInfo.Address,
                    //           AppServerInfo.Port,
                    //           ThemeInfo.Name);
                    //        resource = new ResourceDictionary();
                    //        resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    //        Resources.MergedDictionaries.Add(resource);
                    //        bControl = true;
                    //    }
                    //    catch { }
                    //}
                    //if (!bControl)
                    //{
                    //    //如果从服务器加载资源失败，从已编译到程序集里的默认资源加载资源
                    //    try
                    //    {
                    //        uri = string.Format("/Themes/{0}/Control.xaml",
                    //         ThemeInfo.Name);
                    //        resource = new ResourceDictionary();
                    //        resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    //        Resources.MergedDictionaries.Add(resource);
                    //    }
                    //    catch { }
                    //}
                }
                catch { }
            }
        }

        public virtual void ChangeLanguage()
        {
            //To Change Language
        }

        public void ShowException(string msg)
        {
            MessageBox.Show(msg, CurrentApp == null ? "UMP" : CurrentApp.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformation(string msg)
        {
            MessageBox.Show(msg, CurrentApp == null ? "UMP" : CurrentApp.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
