using System;
using System.Windows;
using System.Windows.Input;
using UMPS3104.Wcf31041;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using  UMPS3104.Models;
using VoiceCyber.UMP.Common;
using UMPS3104.Commands;

namespace UMPS3104
{
    /// <summary>
    /// AppealPage.xaml 的交互逻辑
    /// </summary>
    public partial class AppealPage
    {
        public AgentIntelligentClient PageParent;
        public RecordInfoItem recordinfoitem;
        public RecordScoreInfoItem aScoreInfoItem;
        string warningText = App.GetLanguageInfo("3104T00092", "Please Write Appeal Reason!");
        public AppealPage()
        {
            InitializeComponent();
            ChangeLanguage();
            Loaded += AppealPage_Loaded;
        }

        private void ChangeLanguage()
        {
            appealTextBox.Text = warningText;
            btnConfirm.Content = App.GetLanguageInfo("3104T00070", "Confirm");
            btnClose.Content = App.GetLanguageInfo("3104T00071", "Cancle");
        }

        void AppealPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnConfirm.Click += btnConfirm_Click;
            btnClose.Click += btnClose_Click;
            appealTextBox.MouseEnter += appealTextBox_MouseEnter;
            appealTextBox.MouseLeave += appealTextBox_MouseLeave;
        }

        void appealTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (appealTextBox.Text == string.Empty && !appealTextBox.Focus())
                {
                    appealTextBox.Text = warningText;
                }
            }
            catch (Exception)
            {

            }
        }

        void appealTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (appealTextBox.Text == warningText)
            {
                appealTextBox.Clear();
            }
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception)
            {

            }
        }

        void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            btnConfirm.IsEnabled = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(appealTextBox.Text) && appealTextBox.Text != warningText)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3104Codes.GetSerialID;
                    webRequest.Session = App.Session;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("312");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //Service31041Client client = new Service31041Client();
                    Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                    WebReturn webReturn = client.UMPClientOperation(webRequest);
                    string serialID = webReturn.ListData[0];
                    WriteLog.WriteLogToFile("Appeal \t serialID", serialID);

                    switch (aScoreInfoItem.AppealMark)//显示已申诉就不可再次申诉，适用第一版
                    {
                        case "1":
                        case "2":
                            App.ShowInfoMessage(App.GetLanguageInfo("3104T00116", string.Format("The record has been a complaint")));
                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }
                            return;
                    }
                    /*因数据库未更新最新设定，仍使用Y申诉，N否 C表示申诉完成，最新设定：0,未申诉 ，1申诉，2表申诉完成
                    int appealMark = Convert.ToInt32(webReturn.DataSetData.Tables[0].Rows[0][1]);
                    if (appealMark == 1 || appealMark == 2)//显示已申诉就不可再次申诉，适用第一版
                    {
                        App.ShowInfoMessage(string.Format("The record has been a complaint"));
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                        return;
                    }*/

                    webRequest.Code = (int)S3104Codes.WriteAppeal;
                    webRequest.Session = App.Session;
                    webRequest.ListData.Clear();
                    webRequest.ListData.Add(serialID);//C001
                    webRequest.ListData.Add(aScoreInfoItem.ScoreID.ToString());//C002T_31_008.C001评分成绩表的成绩ID
                    webRequest.ListData.Add(recordinfoitem.SerialID.ToString());//C003 T_21_000.C002 录音记录表的ID
                    webRequest.ListData.Add(App.ListCtrolAgentInfos[0].AgentID);//C004 录音所属座席工号对应的资源编号
                    webRequest.ListData.Add(App.ListCtrolAgentInfos[0].AgentID);//C005 申诉人ID,如果是座席自己申诉的
                    webRequest.ListData.Add(S3104Consts.CON_AppealFlowID.ToString());//C006 申诉流程ID
                    webRequest.ListData.Add(S3104Consts.CON_AppealFlowItemID.ToString());//C007 申诉流程子项ID（通过ActionID+AppealFlowItemID得到该流程走到那一步了）
                    webRequest.ListData.Add("1");//C008 1为申诉，2为审批，3为复核
                    webRequest.ListData.Add("1");//C009 对于申诉(1座席申诉，2他人替申)
                    webRequest.ListData.Add("N");//C010 Y为申诉流程完毕，N 在处理流程中
                    webRequest.ListData.Add("1");//C011 当能多次申诉时启用，每再申诉一次+1，客户端第一版写1
                    webRequest.ListData.Add(System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));//C012 创建时间
                    webRequest.ListData.Add(appealTextBox.Text);//申诉备注
                    webReturn = client.UMPClientOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    else
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00093", "It's OK!"));
                        string strLog = string.Format("{0}{1}", Utils.FormatOptLogString("3104T00002"), recordinfoitem.SerialID);//申诉了XX录音
                        App.WriteOperationLog(S3104Consts.OPT_AgentAppeal.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                        var parent = Parent as PopupPanel;
                        PageParent.RecordScoreListView();
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                }
                else
                {
                    App.ShowExceptionMessage(warningText);
                }
            }
            catch (Exception ex)
            {
                string strLog = string.Format("{0}{1}", Utils.FormatOptLogString("3104T00001"), recordinfoitem.SerialID);//申诉XX录音
                App.WriteOperationLog(S3104Consts.OPT_AgentAppeal.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                App.ShowExceptionMessage(ex.Message);
            }
            btnConfirm.IsEnabled = true;
        }
    }
}
