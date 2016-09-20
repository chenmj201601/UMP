//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    08e4ead6-beab-4300-bf3f-44c58bc7b45b
//        CLR Version:              4.0.30319.42000
//        Name:                     AppContainer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                AppContainer
//
//        created by Charley at 2016/2/2 17:27:05
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace UMPS1201
{
    public class AppContainer : ContentControl
    {
        public static readonly DependencyProperty IsInitedProperty =
            DependencyProperty.Register("IsInited", typeof (bool), typeof (AppContainer), new PropertyMetadata(default(bool)));

        public bool IsInited
        {
            get { return (bool) GetValue(IsInitedProperty); }
            set { SetValue(IsInitedProperty, value); }
        }

        public static readonly DependencyProperty AppTitleProperty =
            DependencyProperty.Register("AppTitle", typeof (string), typeof (AppContainer), new PropertyMetadata(default(string)));


        public string AppTitle
        {
            get { return (string) GetValue(AppTitleProperty); }
            set { SetValue(AppTitleProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof (string), typeof (AppContainer), new PropertyMetadata(default(string)));

        public string Icon
        {
            get { return (string) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
