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
using UMPS1104.Models;
using VoiceCyber.UMP.Controls;

namespace UMPS1104
{
    /// <summary>
    /// UCOrgType.xaml 的交互逻辑
    /// </summary>
    public partial class UCOrgType
    {
        public ObjectItem OTTreeVItem;
        public string IStrCurrentMethod = string.Empty;
        public MainView IParentPage;
        //public S1104App CurrentApp;

        public UCOrgType()
        {
            InitializeComponent();

            this.Loaded += UCOrgType_Loaded;
        }

        void UCOrgType_Loaded(object sender, RoutedEventArgs e)
        {
            SetControlOrgType();
            ChangeLanguage();
            this.ButOrgTypeOK.Click += ButOrgTypeOK_Click;
            this.ButOrgTypeCancel.Click += ButOrgTypeCancel_Click;
        }

        private void SetControlOrgType()
        {
            if (OTTreeVItem == null || IStrCurrentMethod != "E") { return; }
            this.TexOrgTypeName.Text = OTTreeVItem.Name;
            this.TexOrgTypeDescription.Text = OTTreeVItem.Description;
            if (OTTreeVItem.State == 1)
            {
                this.CheckboxOrgTypeStatu.IsChecked = true;
            }
            else
            {
                this.CheckboxOrgTypeStatu.IsChecked = false;
            }
        }

        void ButOrgTypeCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void ButOrgTypeOK_Click(object sender, RoutedEventArgs e)
        {
            //保存
            IParentPage.SaveOrgTypeEdited();
        }

        private void Close()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.LabOrgTypeDescription.Content = CurrentApp.GetLanguageInfo("1104010", "Description");
            this.LabOrgTypeName.Content = CurrentApp.GetLanguageInfo("1104007", "Name");
            this.LabOrgTypeStatu.Content = CurrentApp.GetLanguageInfo("1104009", "statu");
            this.ButOrgTypeCancel.Content = CurrentApp.GetLanguageInfo("1104012", "Cancel");
            this.ButOrgTypeOK.Content = CurrentApp.GetLanguageInfo("1104011", "OK");
            this.CheckboxOrgTypeStatu.Content = CurrentApp.GetLanguageInfo("1104013", "启用");
        }

        #region 保存数据至数据库
        public List<string> GetDateOrgType()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrTypeDescriber = string.Empty;

            try
            {
                string LStrOrgTypeName = TexOrgTypeName.Text.Trim();
                if (string.IsNullOrEmpty(LStrOrgTypeName))
                {
                    ShowException(CurrentApp.GetLanguageInfo("1104T008", ""));
                    return LListStrWcfArgs;
                }
                if (LStrOrgTypeName.Length > 128)
                {
                    ShowException(CurrentApp.GetLanguageInfo("1104T006", ""));
                    return LListStrWcfArgs;
                }
                LStrTypeDescriber = TexOrgTypeDescription.Text.Trim();
                if (LStrTypeDescriber.Length > 1024)
                {
                    ShowException(CurrentApp.GetLanguageInfo("1104T003", ""));
                    return LListStrWcfArgs;
                }
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("1");                                                                                   //3
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //4
                { LListStrWcfArgs.Add(OTTreeVItem.ObjID.ToString()); }
                else { LListStrWcfArgs.Add("0"); }
                if (this.CheckboxOrgTypeStatu.IsChecked == true)                                                                    //5
                { LListStrWcfArgs.Add("1"); }
                else { LListStrWcfArgs.Add("0"); }
                LListStrWcfArgs.Add(LStrOrgTypeName);                                                                       //6
                LListStrWcfArgs.Add(CurrentApp.Session.UserID.ToString());                                               //7
                LListStrWcfArgs.Add(LStrTypeDescriber);                                                //8
                LListStrWcfArgs.Add("");                                                                                    //9
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //10
                { LListStrWcfArgs.Add(OTTreeVItem.LockMethod); }
                else
                {
                    LListStrWcfArgs.Add((S1104App.IDataTable11009.Select("C000 = 1", "C002 ASC").Length + 1).ToString());
                }
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add(IStrCurrentMethod);                                                                     //12
                return LListStrWcfArgs;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return LListStrWcfArgs;
            }
        }

        #endregion
    }
}
