using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common1111;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Common;

namespace UMPS1111
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage
    {
        public MainView ParentPage;
        public ResourceInfo resource;
        private DateTime ST;
        private DateTime ET;
        //public S1111App CurrentApp;
        public SettingPage()
        {
            InitializeComponent();

            this.Loaded += SettingPage_Loaded;
            this.BtnClose.Click += BtnClose_Click;
            this.BtnConfirm.Click += BtnConfirm_Click;
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //DateTime st = Convert.ToDateTime("03/03/2001 12:15:12");
            //DateTime et = Convert.ToDateTime("03/03/2009 12:15:12");
            //绑定关系，设置时间。检查时间是否正确。然后保存到数据库，返回给父辈窗口：listRelation进行数据插入，重新显示树
            if (CheckTime())
            {
                if (ParentPage.SaveRelation(resource, ST.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"), ET.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss")))
                // if (ParentPage.SaveRelation(resource, st.ToUniversalTime().ToString("yyyy/MM/dd"), et.ToUniversalTime().ToString("yyyy/MM/dd")))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("1111N001", "susseed"));
                    #region 记录日志
                    string msg = string.Format("{0}{1}{2}", resource.TenantName, Utils.FormatOptLogString("FO1111001"), resource.ResourceName);
                    CurrentApp.WriteOperationLog(S1111Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
                    #endregion
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("1111N002", "fail"));
                    #region 记录日志
                    string msg = string.Format("{0}{1}{2}", resource.TenantName, Utils.FormatOptLogString("FO1111001"), resource.ResourceName);
                    CurrentApp.WriteOperationLog(S1111Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, msg);
                    #endregion
                }
            }
            else
            {
                //提示：输入时间有错误
                ShowInformation(CurrentApp.GetLanguageInfo("1111N006", "Input time error"));
                return;
            }
            PopupPanelClose();
        }

        private bool CheckTime()
        {
            ST = Convert.ToDateTime(dtValidTime.Text);
            ET = Convert.ToDateTime(dtInValidTime.Text);
            //DateTime ST = Convert.ToDateTime("03/03/2001 12:15:12");
            //DateTime ET = Convert.ToDateTime("03/03/2009 12:15:12");
            DateTime PS = Convert.ToDateTime("1/1/1753 12:00:00");
            DateTime PT = Convert.ToDateTime("12/31/9999 23:59:59"); 
            TimeSpan TS = ET - ST;
            TimeSpan TC = ST - PS;
            TimeSpan TT = PT - ET;
            if (TS.TotalSeconds > 0 && TC.TotalSeconds>=0&&TT.TotalSeconds>=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PopupPanelClose();
        }

        private void PopupPanelClose()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            TxtbRent.Text = resource.TenantName;
            TxtbResource.Text = resource.ToString();
            this.dtValidTime.Value = DateTime.Now;
            this.dtInValidTime.Value = DateTime.MaxValue;
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            this.LabelRent.Content = CurrentApp.GetLanguageInfo("1111P001", "Rent");
            this.LabelResource.Content = CurrentApp.GetLanguageInfo("1111P002", "resource");
            this.LabelValidTime.Content = CurrentApp.GetLanguageInfo("1111P003", "ValidTime");
            this.LabelInValidTime.Content = CurrentApp.GetLanguageInfo("1111P004", "InvalidTime");
            this.BtnConfirm.Content = CurrentApp.GetLanguageInfo("1111B003", "Comfirm");
            this.BtnClose.Content = CurrentApp.GetLanguageInfo("1111B004", "Close");
        }
    }
}
