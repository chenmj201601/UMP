using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using UMPS3104.Wcf00000;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3104
{
    /// <summary>
    /// ChangePassWord.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePassWord
    {
        public AgentIntelligentClient PageParent;

        public ChangePassWord()
        {
            InitializeComponent();
            ChangeLanguage();
            Loaded += ChangePassWord_Loaded;
        }

        void ChangePassWord_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;

            //必需修改密碼，關閉按鈕不可用
            if (App.PwdState == 0.0)
            {
                labMessage.Content = App.GetLanguageInfo("3104T00119", string.Format("新用户，请修改登录密码"));
                BtnClose.IsEnabled = false;
            }
            if (App.PwdState == 0.1)
            {
                labMessage.Content = App.GetLanguageInfo("3104T00120", string.Format("密码已经过期，请修改登录密码"));
                BtnClose.IsEnabled = false;
            }
        }


        public override void ChangeLanguage()
        {
            try
            {
                labOldPwd.Content = App.GetLanguageInfo("3104T00097", "OldPassWord");
                labNewPwd1.Content = App.GetLanguageInfo("3104T00098", "NewPassWord");
                labNewPwd2.Content = App.GetLanguageInfo("3104T00099", "Write Again");
                BtnConfirm.Content = App.GetLanguageInfo("3104T00070", "Confirm");
                BtnClose.Content = App.GetLanguageInfo("3104T00071", "Cancle");
            }
            catch (Exception ex)
            {

            }
        }
        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.PwdState < 1.0)
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("3104T00119", string.Format("新用户，请修改密码")));
                }
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

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            labMessage.Content = "";
            if (string.IsNullOrWhiteSpace(pbOldPwd.Password))
            {
                labMessage.Content = App.GetLanguageInfo("3104T00104", "OldPassWord Is Null");
                return;
            }
            if (string.IsNullOrWhiteSpace(pbNewPwd1.Password) ||string.IsNullOrWhiteSpace(pbNewPwd2.Password))//空密码
            {
                labMessage.Content = App.GetLanguageInfo("3104T00121", "Old Password Or New PassWord Is Null");
                return;
            }
            if (pbOldPwd.Password != App.Session.UserInfo.Password)
            {
                labMessage.Content = App.GetLanguageInfo("3104T00104", "Old PassWord is False,Please Write Correct  PassWord");
                return;
            }
            if (pbNewPwd1.Password != pbNewPwd2.Password)//两次输入的新密码不一致
            {
                labMessage.Content = App.GetLanguageInfo("3104T00122", "Two PassWord Is Not Consistent,Please Write Again");
                return;
            }
            if (App.renterID.Length != 5)
            {
                labMessage.Content = "RenterID Is Flase,Can't Change Password";
                return;
            }
            try
            {
                List<string> pwdString = new List<string>();
                pwdString.Add(App.Session.DBType.ToString());//数据库类型
                pwdString.Add(App.Session.DBConnectionString);//数据库连接串
                pwdString.Add(App.renterID);//租户编码
                pwdString.Add(App.Session.UserID.ToString());//用户编码
                pwdString.Add(pbOldPwd.Password);//原密码
                pwdString.Add(pbNewPwd1.Password);//新密码
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(44, pwdString);
                labMessage.Content = args.StringReturn;
                if (args.StringReturn == "")
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("3104T00093", string.Format("--Modify Sucessed")));
                    App.Session.UserInfo.Password = pbNewPwd1.Password;
                    App.PwdState = 999.00;
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                }
                else if (args.StringReturn.Substring(0, 1) == "W")
                {
                    int temp = Convert.ToInt32(args.StringReturn.Substring(6, 1)) + 3;
                    App.ShowExceptionMessage(App.GetLanguageInfo(string.Format("3104T0010{0}", temp), args.StringReturn));
                    return;
                }
            }
            catch (Exception ex)
            {
                App.WriteOperationLog("3104", ConstValue.OPT_RESULT_FAIL, Utils.FormatOptLogString("3104T00096"));
            }
            #region 写操作日志
            App.WriteOperationLog("3104", ConstValue.OPT_RESULT_SUCCESS, Utils.FormatOptLogString("3104T00096"));
            #endregion
        }

    }
}
