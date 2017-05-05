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
using System.Windows.Forms;
using System.ComponentModel;
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.Data;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_ExportLan.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ExportLan : System.Windows.Controls.UserControl
    {
        BackgroundWorker worker = null;
        ReturnResult RAllLanguagesResult = null;

        public UC_ExportLan()
        {
            InitializeComponent();
            this.Loaded += UC_ExportLan_Loaded;
        }

        void UC_ExportLan_Loaded(object sender, RoutedEventArgs e)
        {
            btnBrowse.Click += btnBrowse_Click;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            chkIsEncryption.Click += chkIsEncryption_Checked;
            cmbLanID.SelectionChanged += cmbLanID_SelectionChanged;
        }

        void cmbLanID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtLanFile.Text = System.IO.Path.Combine(txtLanFile.DataContext.ToString(), "UMP" + (cmbLanID.SelectedItem as ComboBoxItem).DataContext.ToString() + ".YLP");
        }

        void chkIsEncryption_Checked(object sender, RoutedEventArgs e)
        {
            if (chkIsEncryption.IsChecked == true)
            {
                txtPwd.IsEnabled = true;
                txtConfirmPwd.IsEnabled = true;
            }
            else
            {
                txtPwd.Password = string.Empty;
                txtConfirmPwd.Password = string.Empty;
                txtConfirmPwd.IsEnabled = false;
                txtPwd.IsEnabled = false;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //把支持的语言添加到combobox
            if (RAllLanguagesResult.BoolReturn)
            {
                if (RAllLanguagesResult.DataSetReturn.Tables.Count > 0)
                {
                    ComboBoxItem item = null;
                    DataRow row = null;
                    for (int i = 0; i < RAllLanguagesResult.DataSetReturn.Tables[0].Rows.Count; i++)
                    {
                        row = RAllLanguagesResult.DataSetReturn.Tables[0].Rows[i];
                        item = new ComboBoxItem();
                        item.Content = row["C005"].ToString() + " (" + row["C002"].ToString() + ")";
                        item.DataContext = row["C002"].ToString();
                        cmbLanID.Items.Add(item);
                    }
                }
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //获得所有支持的语言类型
            RAllLanguagesResult = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 1, App.GCurrentDBServer);
        }

        /// <summary>
        /// 选择保存文件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog LFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            LFolderBrowserDialog.Description = this.TryFindResource("Message001").ToString();

            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "YLP文件|*.YLP|所有文件|*.*";

            //dialog.Title = App.Current.Resources.MergedDictionaries[1]["SelectFolder"].ToString();
            //dialog.RestoreDirectory = true;
            DialogResult diaResult = LFolderBrowserDialog.ShowDialog();
            if (diaResult == DialogResult.Cancel)
            {
                return;
            }
            txtLanFile.Text = LFolderBrowserDialog.SelectedPath;
            txtLanFile.DataContext = LFolderBrowserDialog.SelectedPath;
        }
    }
}
