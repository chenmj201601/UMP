using Common11121;
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
using UMPS1112.Wcf11121;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.VCLDAP;

namespace UMPS1112
{
    /// <summary>
    /// UCDomainModifyPage.xaml 的交互逻辑
    /// </summary>
    public partial class UCDomainModifyPage
    {
        public MainView ParentPage;
        public int IsAdd;
        public BasicDomainInfo domainInfo;
        //public S1112App CurrentApp;

        public UCDomainModifyPage()
        {
            InitializeComponent();

            Loaded += UCDomainModifyPage_Loaded;
            this.BtnClose.Click += BtnClose_Click;
            this.BtnConfirm.Click += BtnConfirm_Click;
            this.TxtDomainName.LostFocus += TxtDomainName_LostFocus;
            this.TxtUserName.LostFocus += TxtUserName_LostFocus;
        }

        void TxtUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TxtUserName.Text = this.TxtUserName.Text.ToLower();
        }

        void TxtDomainName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TxtDomainName.Text = this.TxtDomainName.Text.ToLower();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValues())
            {
                SaveDomainInfo();
                PageClose();
            }
           
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PageClose();
        }

        private void PageClose()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void UCDomainModifyPage_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            LoadControlInfo();
        }

        private void LoadControlInfo()
        {
            if (IsAdd != 1 && domainInfo != null)
            {
                //将domaininfo信息填到对应窗框中显示
                this.TxtDomainName.Text = domainInfo.DomainName;
                this.TxtUserName.Text = domainInfo.DomainUserName;
                this.PassBoxUserPassword.Password = domainInfo.DomainUserPassWord;
                this.TexDomainDescription.Text = domainInfo.Description;
                if (domainInfo.IsActiveLogin)
                {
                    this.CheckDomainActiveLogin.IsChecked = true;
                }
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.LabelDescription.Content = CurrentApp.GetLanguageInfo("1112P006", "Description");
            this.LabelDomainName.Content = CurrentApp.GetLanguageInfo("1112P002", "DomainName");
            this.LabelIsAutoLogin.Content = CurrentApp.GetLanguageInfo("1112P005", "IsAutoLogin");
            this.LabelUserName.Content = CurrentApp.GetLanguageInfo("1112P003", "UserName");
            this.LabelUserPassword.Content = CurrentApp.GetLanguageInfo("1112P004", "Password");

            this.BtnClose.Content = CurrentApp.GetLanguageInfo("1112B002", "Cancel");
            this.BtnConfirm.Content = CurrentApp.GetLanguageInfo("1112B001", "OK");
            this.CheckDomainActiveLogin.Content = CurrentApp.GetLanguageInfo("1112P007", "启用");
        }

        private bool CheckValues()
        {
            string description = this.TexDomainDescription.Text.Trim();
            string domainName = this.TxtDomainName.Text.Trim();
            string username = this.TxtUserName.Text.Trim();
            string password = this.PassBoxUserPassword.Password.Trim();
            //1.域名称不能为空,且不重复，添加的时候在wcf里面判断
            //2.域账号和密码要可以查到域里的账号
            //3.域描述不能为空
            //4.密码不能有@
            //5.不能有大写字母
            if (domainName.Trim() == string.Empty)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1112T004", "域名为空"));
                return false;
            }
            if (description.Trim() == string.Empty)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1112T005", "域描述为空"));
                return false;
            }
            if (password.Trim().Equals("@"))
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1112T006", "密码不能有@字符"));
                return false;
            }
            bool IsUsed = false;
            try
            {
                string IStrADPath = string.Format("LDAP://{0}", domainName);

                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1112Codes.CheckDomainInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(IStrADPath);
                webRequest.ListData.Add(username);
                webRequest.ListData.Add(password);

                Service11121Client client = new Service11121Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11121"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (webReturn.Result)
                { IsUsed = true; }
                else
                {
                    CurrentApp.ShowExceptionMessage(string.Format("{0}:{1}",CurrentApp.GetLanguageInfo("1112T007", "域连接出错"),webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                //域不可用
                CurrentApp.ShowExceptionMessage(CurrentApp.GetLanguageInfo("1112T007", "域连接出错"));
                //S1112App.ShowExceptionMessage(string.Format("Get Users Info From LDAP Fail:{0}", ex.Message));
                CurrentApp.WriteLog(string.Format("Get all AD users fail.\t{0}", ex.Message));
                return false;
            }
            if (IsAdd == 1)
                domainInfo = new BasicDomainInfo();
            domainInfo.Description = description;
            domainInfo.DomainName = domainName;
            domainInfo.DomainUserName = username;
            domainInfo.DomainUserPassWord = password;
            domainInfo.IsActiveLogin = this.CheckDomainActiveLogin.IsChecked == true ? true : false;
            //domainInfo.IsActive = true;
            //domainInfo.IsDelete = false;
            return IsUsed;
        }

        private void SaveDomainInfo()
        {
            string OptCode = string.Empty;
            if (IsAdd == 1)
            {
                OptCode = "FO1112001";
            }
            else
            {
                OptCode = "FO1112002";
            }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1112Codes.SaveDomainInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(IsAdd.ToString());
                OperationReturn optR = XMLHelper.SeriallizeObject(domainInfo);
                if (!optR.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optR.Code, optR.Message));
                    return;
                }
                webRequest.ListData.Add(optR.Data as string);
                Service11121Client client = new Service11121Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11121"));
                //Service11121Client client = new Service11121Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == 99)
                    {
                        CurrentApp.ShowExceptionMessage(CurrentApp.GetLanguageInfo("", "域名已存在"));
                    }
                    else
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    return;
                }
                CurrentApp.GetLanguageInfo("1112T001", "域信息保存成功");
               //写操作日志
               
                string msg = string.Format("{0}:{1}", CurrentApp.GetLanguageInfo(OptCode, ""), domainInfo.DomainName);
                CurrentApp.WriteOperationLog(OptCode.Substring(2), ConstValue.OPT_RESULT_SUCCESS, msg);
                ParentPage.mListDomainInfo.Clear();
                ParentPage.InitDomainInfo();
            }
            catch (Exception ex)
            {
                if (OptCode != string.Empty)
                {
                    string msg = string.Format("{0}:{1}", CurrentApp.GetLanguageInfo(OptCode, ""), domainInfo.DomainName);
                    CurrentApp.WriteOperationLog(OptCode.Substring(2), ConstValue.OPT_RESULT_FAIL, msg);
                }
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

    }
}
