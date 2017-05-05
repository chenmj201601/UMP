using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS3106.Models;
using VoiceCyber.UMP.Controls;

namespace UMPS3106
{
    /// <summary>
    /// UCRenamePage.xaml 的交互逻辑
    /// </summary>
    public partial class UCRenamePage 
    {
        public TutorialRepertoryMainView ParentPage;
        /// <summary>
        /// 0,新建文件夹; 1, 重命名文件夹; 
        /// </summary>
        public int OperationType;
        public FolderTree FolderInfo;
        public string FolderNameList;


        public UCRenamePage()
        {
            InitializeComponent();
            this.Loaded += UCRenamePage_Loaded;
        }

        void UCRenamePage_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
        }


        private void BtnConfirm_Click(object sender, RoutedEventArgs e)//还需做1：特殊字符的排除；
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbTaskName.Text)) { return; }
                if (tbTaskName.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00032", "Illegal Character"));
                    return;
                }
                if (FolderNameList.Contains(tbTaskName.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00031", "Had One Same Folder"));
                    return;
                }

                bool flag=ParentPage.FolderDBO(tbTaskName.Text,OperationType);
                if (!flag)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Operation Failed"));
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00014", "Operation Sucessed"));
                }
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
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

        public  void ChangeLanguage()
        {

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (OperationType == 0) { parent.Title = CurrentApp.GetLanguageInfo("3106T00020", "New Folder"); }
                    else { parent.Title = CurrentApp.GetLanguageInfo("3106T00021", "Rename Folder"); }
                }
                lbName.Content = CurrentApp.GetLanguageInfo("3106T00008", "FolderName");
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3106T00012", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3106T00013", "Cancel");
            }
            catch { }
        }
    }
}
