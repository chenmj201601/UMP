// ***********************************************************************
// Assembly         : UMPS3103
// Author           : Luoyihua
// Created          : 11-10-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 11-11-2014
// ***********************************************************************
// <copyright file="TaskFinishTimeChange.xaml.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
using UMPS3103.Wcf31031;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;

namespace UMPS3103
{
    /// <summary>
    /// TaskFinishTimeChange.xaml 的交互逻辑
    /// </summary>
    public partial class TaskFinishTimeChange
    {
        /// <summary>
        /// 父页面（任务详情页面）
        /// </summary>
        public TaskRecordDetail ParentPage;
        /// <summary>
        /// 需要更改的任务
        /// </summary>
        public UserTasksInfoShow SelectTask;
        public TaskFinishTimeChange()
        {
            InitializeComponent();
            this.Loaded += TaskFinishTimeChange_Loaded;
        }

        void TaskFinishTimeChange_Loaded(object sender, RoutedEventArgs e)
        {
            dtp_finishtime.Value = Convert.ToDateTime(SelectTask.DealLine);
            SetPageContent();
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-10 11:00:24
        /// <summary>
        /// 更改任务有效期
        /// </summary>
        /// <param name="modifytime">The modifytime.</param>
        /// <param name="taskid">The taskid.</param>
        private void ChangeDealTime(string modifytime,string taskid)
        {
            try
            {
                string dtnow = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.ModifyTaskDealLine;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(modifytime);
                webRequest.ListData.Add(taskid);
                webRequest.ListData.Add(dtnow);

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_MODIFYTASKFINISHTIME.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                   ShowException(string.Format("{0}.\t{1}\t{2}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), webReturn.Code, webReturn.Message));
                    return;
                }
                else
                {
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00034"), SelectTask.TaskName);
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_MODIFYTASKFINISHTIME.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00042", "Operation Succeed."));
                    ParentPage.ShowPopupPanel(false);
                }
            }
            catch (Exception ex)
            {
               ShowException(string.Format("{0}.\t{0}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), ex.Message));
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-10 11:00:24
        /// <summary>
        /// 确认更改
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DateTime modifydt = Convert.ToDateTime(dtp_finishtime.Value);
            if (modifydt.Year != 1)
            {
                modifydt = modifydt.ToUniversalTime();
                ChangeDealTime(modifydt.ToString("yyyy-MM-dd HH:mm:ss"), SelectTask.TaskID.ToString());
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-10 11:00:24
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.ShowPopupPanel(false);
        }

        private void SetPageContent()
        {
            labFinishTime.Content = CurrentApp.GetLanguageInfo("3103T00035", "Expired At");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3103T00036", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3103T00037", "Close");
        }
    }
}
