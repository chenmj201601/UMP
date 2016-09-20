//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    32e38e3c-d29d-435d-8ba8-7b1078205e1c
//        CLR Version:              4.0.30319.42000
//        Name:                     TempView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1299
//        File Name:                TempView
//
//        created by Charley at 2016/1/24 19:05:42
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;

namespace UMPS1299
{
    /// <summary>
    /// TempView.xaml 的交互逻辑
    /// </summary>
    public partial class TempView
    {


        #region Memebers



        #endregion


        public TempView()
        {
            InitializeComponent();
        }

        protected override void Init()
        {
            try
            {
                PageName = "TempView";
                StylePath = "UMPS1299/TempView.xaml";

                base.Init();

                BackgroundWorker worker=new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    if (CurrentApp != null)
                    {
                        CurrentApp.SendLoadedMessage();
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
       
    }
}
