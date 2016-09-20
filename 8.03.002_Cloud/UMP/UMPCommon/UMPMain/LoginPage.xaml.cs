//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5e74fc9a-7d9e-4c34-8f2d-d6654de1563c
//        CLR Version:              4.0.30319.18444
//        Name:                     LoginPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPMain
//        File Name:                LoginPage
//
//        created by Charley at 2014/8/25 14:43:43
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
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
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPMain
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage
    {
        private NetPipeHelper mNetPipeHelper;

        public LoginPage()
        {
            InitializeComponent();

            this.Loaded += LoginPage_Loaded;
            this.Unloaded += LoginPage_Unloaded;
        }

        void LoginPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mNetPipeHelper != null)
            {
                mNetPipeHelper.Stop();
            }
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            mNetPipeHelper = new NetPipeHelper(true, string.Empty);
            mNetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                mNetPipeHelper.Start();
                OnShowMessage(string.Format("Service started."));
            }
            catch (Exception ex)
            {
                OnShowMessage(string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            if (arg != null && arg.Code == 100)
            {
                string msgAnswer = string.Empty;

                SessionInfo session = arg.Session;
                if (session != null)
                {
                    msgAnswer += "SessionID:" + session.SessionID + "\t";
                    UserInfo user = session.UserInfo;
                    if (user != null)
                    {
                        msgAnswer += "UserName:" + user.UserName + "\t";
                    }
                }

                string str = "Msg:" + arg.Data;
                string answer = string.Format("Server response:{0}\t{1}", msgAnswer, str);
                WebReturn webReturn = new WebReturn();
                webReturn.Result = true;
                webReturn.Data = answer;
                return webReturn;

            }
            return null;
        }

        private void OnShowMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }
}
