//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2385d3a9-dedd-4072-9eeb-48b6559e3f58
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPPage
//
//        created by Charley at 2014/8/20 21:14:32
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 表示一个页面，扩展WPF的Page，用来代替Page
    /// </summary>
    public class UMPPage : Page, IUMPPage
    {
        public static readonly DependencyProperty PageNameProperty =
            DependencyProperty.Register("PageName", typeof(string), typeof(UMPPage), new PropertyMetadata(default(string)));

        public string PageName
        {
            get { return (string)GetValue(PageNameProperty); }
            set { SetValue(PageNameProperty, value); }
        }

        //public static readonly DependencyProperty LangIDProperty =
        //    DependencyProperty.Register("LangID", typeof(int), typeof(UMPPage), new PropertyMetadata(default(int)));

        //public int LangID
        //{
        //    get { return (int)GetValue(LangIDProperty); }
        //    set { SetValue(LangIDProperty, value); }
        //}

        public static readonly DependencyProperty LangTypeInfoProperty =
            DependencyProperty.Register("LangTypeInfo", typeof (LangTypeInfo), typeof (UMPPage), new PropertyMetadata(default(LangTypeInfo)));

        public LangTypeInfo LangTypeInfo
        {
            get { return (LangTypeInfo) GetValue(LangTypeInfoProperty); }
            set { SetValue(LangTypeInfoProperty, value); }
        }

        public static readonly DependencyProperty StylePathProperty =
            DependencyProperty.Register("StylePath", typeof(string), typeof(UMPPage), new PropertyMetadata(default(string)));

        public string StylePath
        {
            get { return (string)GetValue(StylePathProperty); }
            set { SetValue(StylePathProperty, value); }
        }

        public static readonly DependencyProperty ThemeInfoProperty =
            DependencyProperty.Register("ThemeInfo", typeof(ThemeInfo), typeof(UMPPage), new PropertyMetadata(default(ThemeInfo)));

        public ThemeInfo ThemeInfo
        {
            get { return (ThemeInfo)GetValue(ThemeInfoProperty); }
            set { SetValue(ThemeInfoProperty, value); }
        }

        public static readonly DependencyProperty AppServerInfoProperty =
            DependencyProperty.Register("AppServerInfo", typeof (AppServerInfo), typeof (UMPPage), new PropertyMetadata(default(AppServerInfo)));

        public AppServerInfo AppServerInfo
        {
            get { return (AppServerInfo) GetValue(AppServerInfoProperty); }
            set { SetValue(AppServerInfoProperty, value); }
        }

        public virtual void ChangeTheme()
        {
            //To Change Theme
            if (ThemeInfo != null)
            {
                try
                {
                    Resources.MergedDictionaries.Clear();
                    ResourceDictionary resource;
                    string uri;
                    bool bControl = false;

                    if (AppServerInfo != null)
                    {
                        //优先从服务器获取资源文件
                        try
                        {
                            uri = string.Format("{0}://{1}:{2}/Themes/{3}/Control.xaml",
                               AppServerInfo.Protocol,
                               AppServerInfo.Address,
                               AppServerInfo.Port,
                               ThemeInfo.Name);
                            resource = new ResourceDictionary();
                            resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                            Resources.MergedDictionaries.Add(resource);
                            bControl = true;
                        }
                        catch { }
                    }
                    if (!bControl)
                    {
                        //如果从服务器加载资源失败，从已编译到程序集里的默认资源加载资源
                        try
                        {
                            uri = string.Format("/Themes/{0}/Control.xaml",
                             ThemeInfo.Name);
                            resource = new ResourceDictionary();
                            resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                            Resources.MergedDictionaries.Add(resource);
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        public virtual void ChangeLanguage()
        {
            //To Change Language
        }
    }
}
