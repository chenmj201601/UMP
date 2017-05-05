using System;
using System.Collections.Generic;
using System.Data;
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
using UMPS0001.PublicClasses;

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCLocateServerFolder : UserControl,S0001Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public string IStrSelectedFolderForReturn = string.Empty;

        public string IStrCurrentFolderType = string.Empty;
        private TreeViewItem ITreeViewItemExpand = null;

        public UCLocateServerFolder(string AStrCurrentFolderType)
        {
            InitializeComponent();
            IStrCurrentFolderType = AStrCurrentFolderType;
            this.Loaded += UCLocateServerFolder_Loaded;
        }

        public void ResetReloadOptions()
        {
            ITreeViewItemExpand = null;
            TreeViewServerFolder.Items.Clear();
        }

        public void ShowSubServerFolderEnd(DataTable ADataTableFolders)
        {
            string LStrSingleFolder = string.Empty;
            string LStrParentFolder = string.Empty;
            List<string> LListStrSplitPath = new List<string>();
            string LStrImagesPath = string.Empty;

            try
            {
                LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S0001");
                if (ADataTableFolders == null) { return; }
                if (ITreeViewItemExpand == null) { LStrParentFolder = string.Empty; }
                else { LStrParentFolder = ITreeViewItemExpand.DataContext.ToString(); }
                foreach (DataRow LDataRowSingleFolder in ADataTableFolders.Rows)
                {
                    TreeViewItem LTreeViewItemFolder = new TreeViewItem();
                    LStrSingleFolder = LDataRowSingleFolder[0].ToString();
                    if (LStrSingleFolder.Substring(0, 1) == "$") { continue; }
                    LTreeViewItemFolder.Header = LStrSingleFolder;
                    LTreeViewItemFolder.Tag = "0";
                    LTreeViewItemFolder.DataContext = System.IO.Path.Combine(LStrParentFolder, LStrSingleFolder) + @"\";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemFolder, LStrImagesPath + @"\S0001013.ico");
                    if (ITreeViewItemExpand == null) { TreeViewServerFolder.Items.Add(LTreeViewItemFolder); }
                    else { ITreeViewItemExpand.Items.Add(LTreeViewItemFolder); }
                    LTreeViewItemFolder.Expanded += LTreeViewItemFolder_Expanded;
                }
            }
            catch { }
            finally
            {
                if (ITreeViewItemExpand != null) { ITreeViewItemExpand.IsExpanded = true; }
            }
        }

        private void LTreeViewItemFolder_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem LTreeViewItemExpanded = e.Source as TreeViewItem;
                if (LTreeViewItemExpanded == null) { return; }
                if (LTreeViewItemExpanded.Tag.ToString() == "1") { return; }
                LTreeViewItemExpanded.Tag = "1";

                ITreeViewItemExpand = LTreeViewItemExpanded;

                if (IOperationEvent == null) { return; }
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "C001";
                LEventArgs.ObjectSource0 = IStrCurrentFolderType;
                LEventArgs.ObjectSource1 = LTreeViewItemExpanded.DataContext.ToString();
                IOperationEvent(this, LEventArgs);
            }
            catch { }
        }

        public List<string> GetSettedData(ref string AStrCallReturn)
        {
            List<string> LListStrReturn = new List<string>();

            try
            {
                TreeViewItem LTreeViewItemSelected = TreeViewServerFolder.SelectedItem as TreeViewItem;
                if (LTreeViewItemSelected != null)
                {
                    LListStrReturn.Add(LTreeViewItemSelected.DataContext.ToString());
                }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        #region 对象初始化执行的代码
        private void UCLocateServerFolder_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            if (IStrCurrentFolderType == "D")
            {
                TabItemLocateServerFolder.Header = " " + App.GetDisplayCharater("M02084") + " ";
            }
            else
            {
                TabItemLocateServerFolder.Header = " " + App.GetDisplayCharater("M02085") + " ";
            }
        }
        #endregion

        
    }
}
