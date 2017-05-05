using System;
using System.Collections.Generic;
using System.IO;
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
using UMPServicePackCommon;
using VoiceCyber.Common;

namespace UMPServicePack.UserControls
{
    /// <summary>
    /// UC_BackupPathChoose.xaml 的交互逻辑
    /// </summary>
    public partial class UC_BackupPathChoose : UserControl
    {
        public MainWindow main = null;

        public UC_BackupPathChoose()
        {
            InitializeComponent();
            Loaded += UC_BackupPathChoose_Loaded;
        }

        void UC_BackupPathChoose_Loaded(object sender, RoutedEventArgs e)
        {
            #region 控件事件
            btnBrowse.Click += btnBrowse_Click;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            btnTermination.Click += btnTermination_Click;
            #endregion

            #region 是否需要重命名Ntidrv
            if (App.dicAppInstalled.Keys.Contains(ConstDefines.UMPSoftRecord) && App.dicAppInstalled.Keys.Contains(ConstDefines.IxPatch))
            {
                List<UpdateFile> lstFiles = App.updateInfo.ListFiles.Where(p => p.FileName.ToLower() == "ntidrv.dll").ToList();
                if (lstFiles.Count <= 0)
                {
                    grpNtidrv.Visibility = Visibility.Collapsed;
                    return;
                }
                grpNtidrv.Visibility = Visibility.Visible;
                //判断softrecord的ntidrv是否需要更新 更新则显示改名
                List<UpdateFile> lstFilesByPackage = lstFiles.Where(p => p.Package == ConstDefines.UMPSoftRecord).ToList();
                if (lstFilesByPackage.Count > 0)
                {
                    dpIP.Visibility = Visibility.Visible;
                }
                else
                {
                    dpIP.Visibility = Visibility.Collapsed;
                }
                lstFilesByPackage = lstFiles.Where(p =>p.Package == ConstDefines.IxPatch).ToList();
                if (lstFilesByPackage.Count > 0)
                {
                    dpSnyway.Visibility = Visibility.Visible;
                }
                else
                {
                    dpSnyway.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                grpNtidrv.Visibility = Visibility.Hidden;
            }
            #endregion
        }

        void btnTermination_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                var result = MessageBox.Show(App.GetLanguage("string55", "string55"), "UMP Service Pack", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            string strIPName = string.Empty;
            string strSnywayName = string.Empty;
            if (grpNtidrv.Visibility == System.Windows.Visibility.Visible)
            {
                if (dpIP.Visibility == System.Windows.Visibility.Visible)
                {
                    if (string.IsNullOrEmpty(txtIPNewName.Text))
                    {
                        App.ShowMessage(App.GetLanguage("string99", "string99"));
                        return;
                    }
                    strIPName = txtIPNewName.Text;
                    if (!strIPName.EndsWith(".dll"))
                    {
                        strIPName += ".dll";
                    }
                }
                if (dpSnyway.Visibility == System.Windows.Visibility.Visible)
                {
                    if (string.IsNullOrEmpty(txtSnywayNewName.Text))
                    {
                        App.ShowMessage(App.GetLanguage("string98", "string98"));
                        return;
                    }
                    strSnywayName = txtSnywayNewName.Text;
                    if (!strSnywayName.EndsWith(".dll"))
                    {
                        strSnywayName += ".dll";
                    }
                }
            }
            UC_Upgrade uc_Upgrade = new UC_Upgrade();
            uc_Upgrade.main = main;
            uc_Upgrade.BackupPath = txtPath.Text;
            uc_Upgrade.IsBackupUMP = !string.IsNullOrEmpty(txtPath.Text);
            uc_Upgrade.Ntidrv_IP_NewName = strIPName;
            uc_Upgrade.Ntidrv_Snyway_NewName = strSnywayName;
            main.borderUpdater.Child = uc_Upgrade;
        }

        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            UC_License uc_License = new UC_License();
            uc_License.main = main;
            main.borderUpdater.Child = uc_License;
        }

        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = dialog.SelectedPath.Trim();
            txtPath.Text = m_Dir + "\\" + App.updateInfo.Version;
        }

        private void GoToDone(bool bIsSuccess)
        {
            UC_Done uc_Done = new UC_Done();
            uc_Done.bIsSuccess = bIsSuccess;
            uc_Done.main = main;
            main.borderUpdater.Child = uc_Done;
        }
    }
}
