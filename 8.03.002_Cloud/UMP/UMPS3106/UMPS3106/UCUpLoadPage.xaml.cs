using Common3106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    /// UCUpLoadPage.xaml 的交互逻辑
    /// </summary>
    public partial class UCUpLoadPage
    {
        public TutorialRepertoryMainView ParentPage;
        public FolderTree FolderInfo;
        FilesItemInfo fileItemInfo;
        /// <summary>
        /// 带后缀的文件名
        /// </summary>
        string HaveExtension = string.Empty;
        public string WebFolderPath;
        public List<string> FilesName;


        public UCUpLoadPage()
        {
            InitializeComponent();
            fileItemInfo = new FilesItemInfo();
            this.Loaded += UCUpLoadPage_Loaded;
            butSelectFiles.Click += butSelectFiles_Click;
        }


        void UCUpLoadPage_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            txtPath.Text = WebFolderPath;
        }


        #region 触发事件
        void butSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    txtSelectFiles.Text = file.FileName;
                    HaveExtension = file.SafeFileName;
                    txtFilesName.Text = System.IO.Path.GetFileNameWithoutExtension(file.SafeFileName);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtFilesName.Text))
                {
                    if (FilesName.Contains(txtFilesName.Text))
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3106T00029", "Had One Files,Please Select Another File."));
                        return;
                    }
                    fileItemInfo.FolderID = FolderInfo.FolderID;
                    fileItemInfo.FileName = txtFilesName.Text;
                    fileItemInfo.FilePath = System.IO.Path.Combine(WebFolderPath, HaveExtension);
                    fileItemInfo.FileDescription = tbFilesDescription.Text;
                    fileItemInfo.FromType = "0";
                    fileItemInfo.FileType = System.IO.Path.GetExtension(HaveExtension) == ".wav" ? "1" : "0";
                    bool flag = ParentPage.UpLoadFiles(fileItemInfo, txtSelectFiles.Text);
                    if (flag)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3106T00014", "Upload Sucessed!"));
                    }
                    else
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Upload Failed."));
                    }
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00011", "Please Select File"));
                    return;
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
        #endregion

        public  void ChangeLanguage()
        {

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("FO3106005", "Upload");
                }
                labPath.Content = CurrentApp.GetLanguageInfo("3106T00010", "Now Folder Path :");
                labSelectFiles.Content = CurrentApp.GetLanguageInfo("3106T00011", "Select File");
                labFilesName.Content = CurrentApp.GetLanguageInfo("3106T00003", "File Name");
                labFilesDescription.Content = CurrentApp.GetLanguageInfo("3106T00005", "File Description");
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3106T00012", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3106T00013", "Cancel");
            }
            catch { }
        }
    }
}
