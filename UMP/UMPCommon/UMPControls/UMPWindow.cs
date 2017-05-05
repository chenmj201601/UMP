//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c40716c5-7379-4b66-b297-ee2ff57be1bc
//        CLR Version:              4.0.30319.18444
//        Name:                     UMPWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                UMPWindow
//
//        created by Charley at 2015/2/2 9:33:20
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 表示一个窗体，用来代替Window
    /// </summary>
    public class UMPWindow : Window, IUMPPage
    {
        public static readonly DependencyProperty PageNameProperty =
            DependencyProperty.Register("PageName", typeof (string), typeof (UMPWindow), new PropertyMetadata(default(string)));

        public string PageName
        {
            get { return (string) GetValue(PageNameProperty); }
            set { SetValue(PageNameProperty, value); }
        }

        public static readonly DependencyProperty LangTypeInfoProperty =
            DependencyProperty.Register("LangTypeInfo", typeof (LangTypeInfo), typeof (UMPWindow), new PropertyMetadata(default(LangTypeInfo)));

        public LangTypeInfo LangTypeInfo
        {
            get { return (LangTypeInfo) GetValue(LangTypeInfoProperty); }
            set { SetValue(LangTypeInfoProperty, value); }
        }

        public static readonly DependencyProperty StylePathProperty =
            DependencyProperty.Register("StylePath", typeof (string), typeof (UMPWindow), new PropertyMetadata(default(string)));

        public string StylePath
        {
            get { return (string) GetValue(StylePathProperty); }
            set { SetValue(StylePathProperty, value); }
        }

        public static readonly DependencyProperty ThemeInfoProperty =
            DependencyProperty.Register("ThemeInfo", typeof (ThemeInfo), typeof (UMPWindow), new PropertyMetadata(default(ThemeInfo)));

        public ThemeInfo ThemeInfo
        {
            get { return (ThemeInfo) GetValue(ThemeInfoProperty); }
            set { SetValue(ThemeInfoProperty, value); }
        }

        public static readonly DependencyProperty AppServerInfoProperty =
            DependencyProperty.Register("AppServerInfo", typeof (AppServerInfo), typeof (UMPWindow), new PropertyMetadata(default(AppServerInfo)));

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
