using PFShareClassesC;
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

namespace UMPS1110
{
    public partial class UCObjectOperationGroup : UserControl, S1110Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private string IStrImageFolder = string.Empty;
        private string IStrVerificationCode104 = string.Empty;
        private OperationParameters IOperationParameters = null;

        public UCObjectOperationGroup()
        {
            InitializeComponent();
            IStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            IStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1110");
            this.Loaded += UCObjectOperationGroup_Loaded;
            GridObjectOperationGroupName.PreviewMouseLeftButtonDown += GridObjectOperationGroupName_PreviewMouseLeftButtonDown;
        }

        private void GridObjectOperationGroupName_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (StackPanelObjectOperationsList.Visibility == System.Windows.Visibility.Collapsed)
            {
                StackPanelObjectOperationsList.Visibility = System.Windows.Visibility.Visible;
                ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(IStrImageFolder, "S1110009.png"), UriKind.RelativeOrAbsolute));
            }
            else
            {
                StackPanelObjectOperationsList.Visibility = System.Windows.Visibility.Collapsed;
                ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(IStrImageFolder, "S1110007.png"), UriKind.RelativeOrAbsolute));
            }
        }

        private void UCObjectOperationGroup_Loaded(object sender, RoutedEventArgs e)
        {
            ImageUpDownArrow.Source = new BitmapImage(new Uri(System.IO.Path.Combine(IStrImageFolder, "S1110009.png"), UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AParameters">
        /// OperationParameters.StrObjectTag Object Type ID
        /// OperationParameters.ObjectSource0 Object Information DataRow等
        /// OperationParameters.ObjectSource1 权限列表 - DataTable
        /// OperationParameters.ObjectSource2 显示的权限ID List<string>
        /// OperationParameters.ObjectSource3 Current Object
        /// </param>
        public void ShowObjectAllOperations(OperationParameters AParameters)
        {
            IOperationParameters = AParameters;

            TextBlockOperationGroupName.Text = GetObjectContentForExpanderHeader(AParameters);

            List<string> LListStrOperations = AParameters.ObjectSource2 as List<string>;

            StackPanelObjectOperationsList.Children.Clear();

            foreach (string LStrOperationID in LListStrOperations)
            {
                if (!string.IsNullOrEmpty(LStrOperationID))
                {
                    UCObjectOperationSingle LUCObjectOperationSingle = new UCObjectOperationSingle(LStrOperationID, IOperationParameters.ObjectSource3);
                    LUCObjectOperationSingle.IOperationEvent += LUCObjectOperationSingle_IOperationEvent;
                    LUCObjectOperationSingle.ShowOperationDetails(IOperationParameters);
                    LUCObjectOperationSingle.Margin = new Thickness(0, 1, 0, 1);
                    StackPanelObjectOperationsList.Children.Add(LUCObjectOperationSingle);
                }
                else
                {
                    if (StackPanelObjectOperationsList.Children.Count == 0) { continue; }
                    Label LLableSpliter = new Label();
                    LLableSpliter.Margin = new Thickness(10, 1, 5, 1);
                    LLableSpliter.Height = 1;
                    LLableSpliter.Background = Brushes.LightGray;
                    StackPanelObjectOperationsList.Children.Add(LLableSpliter);
                }
            }
        }

        private string GetObjectContentForExpanderHeader(OperationParameters AParameters)
        {
            string LStrReturn = string.Empty;
            string LStrTypeIDLeft3 = string.Empty;

            try
            {
                LStrTypeIDLeft3 = AParameters.StrObjectTag.Substring(0, 3);
                Type LTypeObject = AParameters.ObjectSource3.GetType();
                if (LTypeObject == typeof(ListViewItem))
                {
                    ListViewItem LListViewItemObject = AParameters.ObjectSource3 as ListViewItem;
                    DataRow LDataRowInfo = LListViewItemObject.Tag as DataRow;
                    if (LStrTypeIDLeft3 == "212")
                    {
                        LStrReturn = LDataRowInfo["C017"].ToString();
                        LStrReturn = EncryptionAndDecryption.EncryptDecryptString(LStrReturn, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    }
                    return LStrReturn;
                }
                if (LTypeObject == typeof(TreeViewItem))
                {
                    TreeViewItem LTreeViewItemObject = AParameters.ObjectSource3 as TreeViewItem;
                    LStrReturn = LTreeViewItemObject.Header.ToString();
                    return LStrReturn;
                }
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        private void LUCObjectOperationSingle_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }
    }
}
