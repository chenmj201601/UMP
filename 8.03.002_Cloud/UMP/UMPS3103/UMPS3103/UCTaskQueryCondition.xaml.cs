using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using UMPS3103.Models;
using UMPS3103.Wcf31031;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Common;
using VoiceCyber.NAudio.Controls;

namespace UMPS3103
{
    /// <summary>
    /// UCTaskQueryCondition.xaml 的交互逻辑
    /// </summary>
    public partial class UCTaskQueryCondition
    {
        public TaskTrack PageParent;
        public UCTaskQueryCondition()
        {
            InitializeComponent();
            Loaded += UCTaskQueryCondition_Loaded;
        }

        void UCTaskQueryCondition_Loaded(object sender, RoutedEventArgs e)
        {
            dateTimeStart.Value = DateTime.Now.AddDays(-15);
            dateTimeEnd.Value = DateTime.Now.AddDays(15);
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            SetPopContent();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                if (dateTimeStart.Value == null || dateTimeEnd.Value == null || dateTimeEnd.Value < dateTimeStart.Value)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00144", "Invalid Datetime."), CurrentApp.AppName);
                    return;
                }
                string dtstart = Convert.ToDateTime(dateTimeStart.Value).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                string dtend = Convert.ToDateTime(dateTimeEnd.Value).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                string finish = "";
                if (rbtnUnfinshed.IsChecked == true)
                    finish = "0";
                else if (rbtnFinshed.IsChecked == true)
                    finish = "1";
                else
                    finish = "2";
                PageParent.InitTasks("2", finish, dtstart, dtend);
                #region 写操作日志
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKTRACK.ToString(), ConstValue.OPT_RESULT_SUCCESS, Utils.FormatOptLogString("3103T00133"));
                #endregion

                Close();
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Close()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void SetPopContent()
        {
            lbdatastart.Content = CurrentApp.GetLanguageInfo("3103T00145", "DeadLine Time Start");
            lbdatasend.Content = CurrentApp.GetLanguageInfo("3103T00146", "DeadLine Time End");
            lbtafininfo.Content = CurrentApp.GetLanguageInfo("3103T00147", "Complete Status");

            rbtnAll.Content = CurrentApp.GetLanguageInfo("3103T00131", "All");
            rbtnFinshed.Content = CurrentApp.GetLanguageInfo("3103T00019", "Finished");
            rbtnUnfinshed.Content = CurrentApp.GetLanguageInfo("3103T00020", "Unfinished");

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }
    }
}
