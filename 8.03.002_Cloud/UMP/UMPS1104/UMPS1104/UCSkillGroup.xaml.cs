using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
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
using UMPS1104.WCFService00000;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1104
{
    /// <summary>
    /// UCSkillGroup.xaml 的交互逻辑
    /// </summary>
    public partial class UCSkillGroup
    {
        public ObjectItem SGTreeVItem;
        public string IStrCurrentMethod = string.Empty;
        public MainView IParentPage;
        //public S1104App CurrentApp;

        public UCSkillGroup()
        {
            InitializeComponent();

            this.Loaded += UCSkillGroup_Loaded;
        }

        void UCSkillGroup_Loaded(object sender, RoutedEventArgs e)
        {
            SetControlSkillGroup();
            ChangeLanguage();
            this.ButSkillGroupOK.Click += ButSkillGroupOK_Click;
            this.ButSkillGroupCancel.Click += ButSkillGroupCancel_Click;
        }

        private void SetControlSkillGroup()
        {
            if (SGTreeVItem == null || IStrCurrentMethod != "E") { return; }
            this.TexSkillGroupCode.Text = SGTreeVItem.FullName;
            this.TexSkillGroupName.Text = SGTreeVItem.Name;
            this.TexSkillGroupDescription.Text = SGTreeVItem.Description;
            if (SGTreeVItem.State == 1)
            {
                this.CheckboxSkillGroupStatu.IsChecked = true;
            }
            else
            {
                this.CheckboxSkillGroupStatu.IsChecked = false;
            }
        }

        void ButSkillGroupCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void ButSkillGroupOK_Click(object sender, RoutedEventArgs e)
        {
            //保存
            //GetDateSkillGroup();
            IParentPage.SaveSkillGroupEdited();
            //Close();
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

            this.LabSkillGroupCode.Content = CurrentApp.GetLanguageInfo("1104008", "Code");
            this.LabSkillGroupDescription.Content = CurrentApp.GetLanguageInfo("1104010", "Description");
            this.LabSkillGroupName.Content = CurrentApp.GetLanguageInfo("1104007", "Name");
            this.LabSkillGroupStatu.Content = CurrentApp.GetLanguageInfo("1104009", "statu");
            this.ButSkillGroupCancel.Content = CurrentApp.GetLanguageInfo("1104012", "Cancel");
            this.ButSkillGroupOK.Content = CurrentApp.GetLanguageInfo("1104011", "OK");
            this.CheckboxSkillGroupStatu.Content = CurrentApp.GetLanguageInfo("1104013", "启用");
        }

        #region 保存数据至数据库
        public List<string> GetDateSkillGroup()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrSkillCode = string.Empty, LStrSkillName = string.Empty;
            string LStrTypeDescriber = string.Empty;

            try
            {
                LStrSkillCode = TexSkillGroupCode.Text.Trim();
                if (string.IsNullOrEmpty(LStrSkillCode))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1104T007", ""));
                    return LListStrWcfArgs;
                }
                if (LStrSkillCode.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1104T005", ""));
                    return LListStrWcfArgs;
                }
                LStrSkillName = TexSkillGroupName.Text.Trim();
                if (string.IsNullOrEmpty(LStrSkillName))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1104T008", ""));
                    return LListStrWcfArgs;
                }
                if (LStrSkillName.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1104T006", ""));
                    return LListStrWcfArgs;
                }

                LStrTypeDescriber = TexSkillGroupDescription.Text.Trim();
                if (LStrTypeDescriber.Length > 1024)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1104T003", ""));
                    return LListStrWcfArgs;
                }

                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("2");                                                                                   //3
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //4
                { LListStrWcfArgs.Add(SGTreeVItem.ObjID.ToString()); }
                else { LListStrWcfArgs.Add("0"); }
                if (this.CheckboxSkillGroupStatu.IsChecked == true)                                                                    //5
                { LListStrWcfArgs.Add("1"); }
                else { LListStrWcfArgs.Add("0"); }
                LListStrWcfArgs.Add(LStrSkillCode);                                                                       //6
                LListStrWcfArgs.Add(CurrentApp.Session.UserID.ToString());                                               //7
                LListStrWcfArgs.Add(LStrTypeDescriber);                                                //8
                LListStrWcfArgs.Add(LStrSkillName);                                                                         //9
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //10
                { LListStrWcfArgs.Add(SGTreeVItem.LockMethod.ToString()); }
                else
                {
                    LListStrWcfArgs.Add((S1104App.IDataTable11009.Select("C000 = 2", "C002 ASC").Length + 1).ToString());
                }
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add(IStrCurrentMethod);
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
