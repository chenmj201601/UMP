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
using UMP.MAMT.CertificateSetting;
using UMP.MAMT.DatabaseSetting;
using UMP.MAMT.IISBindingSetting;
using UMP.MAMT.LicenseServer;
using UMP.MAMT.LogicPartitionSetting;
using UMP.MAMT.PublicClasses;
using YoungWPFTabControl;

namespace UMP.MAMT.BasicControls
{
    public partial class UCObjectDetails : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        #region WPF TabCotrol
        //TabCotrol控件
        private YoungWPFTabControl.TabControl ITabControlMainPanel = null;

        //增加TabItem的参数
        private MamtOperationEventArgs IOperationEventArgsAddItem = new MamtOperationEventArgs();

        //对象资源管理详细信息TabItem
        YoungWPFTabControl.TabItem ITabItemServerObjectDetail = null;
        #endregion

        //当前传递进来显示的对象参数
        private MamtOperationEventArgs IOperationEventArgs = null;

        public UCObjectDetails()
        {
            InitializeComponent();
            this.Loaded += UCObjectDetails_Loaded;
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            try
            {
                if (IOperationEventArgs == null)
                {
                    LabelCurrentObjectPath.Content = App.GetDisplayCharater("M01011");
                }
                else
                {
                    ShowObjectDetailsInTabItemServerObject(IOperationEventArgs, ABoolLanguageChange);
                }
                try
                {
                    TextBlock LTextBlockHeader = (TextBlock)ITabItemServerObjectDetail.Header;
                    LTextBlockHeader.Text = App.GetDisplayCharater("M01012");
                    //LabelCurrentObjectPath.Content = App.GetDisplayCharater("M01011");
                }
                catch { }
            }
            catch { }
            
        }

        public void OpenServerObjectSingleInformation(object AObjectSender, MamtOperationEventArgs AOperationEventArgs)
        {
            try
            {
                IOperationEventArgsAddItem = AOperationEventArgs;

                //这里需要判断要打开的对象是否已经打开，如果打开，则跳转到已经打开的页面，考虑是否可以放到MainWindows中处理该判断逻辑

                ITabControlMainPanel.AddTabItem();
            }
            catch { }
        }

        public void ShowObjectDetailsInTabItemServerObject(MamtOperationEventArgs AObjectArgs, bool ABoollanguageChanage)
        {
            try
            {
                IOperationEventArgs = AObjectArgs;
                if (AObjectArgs.StrElementTag == "TSCH")            //UCObject TreeViewItem SelectedItemChanged
                {
                    ShowObjectTabItemHeaderFromBasicTreeViewItem(AObjectArgs.ObjSource);
                    ShowObjectInformationInTabItemFromBasicTreeViewItem(AObjectArgs.ObjSource, ABoollanguageChanage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SetTabItemServerObjectDetailToolTip");
            }
        }

        private void UCObjectDetails_Loaded(object sender, RoutedEventArgs e)
        {
            ImageServerDetailTip.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000012.ico"), UriKind.RelativeOrAbsolute));
            DisplayElementCharacters(false);
            InitWPFTabControl();
        }

        private void InitWPFTabControl()
        {
            try
            {
                if (ITabControlMainPanel != null) { return; }

                ITabControlMainPanel = new YoungWPFTabControl.TabControl();

                ITabControlMainPanel.Name = "TabControlServerDetail";
                ITabControlMainPanel.TabStripPlacement = Dock.Top;
                ITabControlMainPanel.TabItemMinWidth = 150.00;
                ITabControlMainPanel.TabItemMinHeight = 26;
                ITabControlMainPanel.TabItemMaxHeight = 26;
                ITabControlMainPanel.VerticalContentAlignment = VerticalAlignment.Center;
                ITabControlMainPanel.AllowAddNew = false;
                ITabControlMainPanel.Background = Brushes.Transparent;
                ITabControlMainPanel.AddNewTabToEnd = true;
                ITabControlMainPanel.Opacity = 0.8;

                LinearGradientBrush LLinearBrushNormalBackground = new LinearGradientBrush();
                LLinearBrushNormalBackground.StartPoint = new Point(0, 0);
                LLinearBrushNormalBackground.EndPoint = new Point(0, 1);
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(252, 253, 253), 0.0));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(229, 234, 245), 0.3));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(207, 215, 235), 0.3));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(224, 229, 245), 0.7));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(236, 238, 252), 1.0));
                ITabControlMainPanel.TabItemNormalBackground = LLinearBrushNormalBackground;

                LinearGradientBrush LLinearBrushSelectedBackground = new LinearGradientBrush();
                LLinearBrushSelectedBackground.StartPoint = new Point(0, 0);
                LLinearBrushSelectedBackground.EndPoint = new Point(0, 1);
                LLinearBrushSelectedBackground.GradientStops.Add(new GradientStop(Color.FromRgb(251, 253, 254), 0.0));
                LLinearBrushSelectedBackground.GradientStops.Add(new GradientStop(Color.FromRgb(234, 246, 251), 0.3));
                LLinearBrushSelectedBackground.GradientStops.Add(new GradientStop(Color.FromRgb(206, 231, 250), 0.3));
                LLinearBrushSelectedBackground.GradientStops.Add(new GradientStop(Color.FromRgb(185, 209, 250), 1.0));
                ITabControlMainPanel.TabItemSelectedBackground = LLinearBrushSelectedBackground;

                LinearGradientBrush LLinearBrushMouseOverBackground = new LinearGradientBrush();
                LLinearBrushNormalBackground.StartPoint = new Point(0, 0);
                LLinearBrushNormalBackground.EndPoint = new Point(0, 1);
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(252, 253, 253), 0.0));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(229, 234, 245), 0.3));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(207, 215, 235), 0.3));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(224, 229, 245), 0.7));
                LLinearBrushNormalBackground.GradientStops.Add(new GradientStop(Color.FromRgb(236, 238, 252), 1.0));
                ITabControlMainPanel.TabItemMouseOverBackground = LLinearBrushMouseOverBackground;

                ITabControlMainPanel.TabItemAdded += LTabControlMainPanel_TabItemAdded;
                ITabControlMainPanel.SelectionChanged += LTabControlMainPanel_SelectionChanged;
                ITabControlMainPanel.TabItemClosed += LTabControlMainPanel_TabItemClosed;

                GridTabControlObjectDetail.Children.Add(ITabControlMainPanel);

            }
            catch { }
        }

        private void LTabControlMainPanel_TabItemClosed(object sender, TabItemEventArgs e)
        {

        }

        private void LTabControlMainPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                YoungWPFTabControl.TabControl LTabControl = (YoungWPFTabControl.TabControl)e.Source;
                YoungWPFTabControl.TabItem LTabItem = (YoungWPFTabControl.TabItem)LTabControl.SelectedItem;
                LabelCurrentObjectPath.Content = LTabItem.Tag.ToString();
            }
            catch { }
        }

        private void LTabControlMainPanel_TabItemAdded(object sender, TabItemEventArgs e)
        {
            try
            {
                if (IOperationEventArgsAddItem.StrElementTag == "0000000000") { OpenServerObjectItemDetailsUserControl(e); }
            }
            catch { }
        }

        private void OpenServerObjectItemDetailsUserControl(TabItemEventArgs AEventArgs)
        {
            try
            {
                BitmapImage LBitmapImageShow = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000013.ico"), UriKind.RelativeOrAbsolute));
                Image LImageTabItem = new Image();
                LImageTabItem.Width = 16; LImageTabItem.Height = 16;
                LImageTabItem.Source = LBitmapImageShow;
                LImageTabItem.Margin = new Thickness(2, 0, 2, 0);
                AEventArgs.TabItem.Icon = LImageTabItem;

                TextBlock LTextBlockHeader = new TextBlock();
                LTextBlockHeader.Style = (Style)App.Current.Resources["TextBlockNormalStyle"];
                LTextBlockHeader.TextTrimming = TextTrimming.CharacterEllipsis;
                LTextBlockHeader.TextWrapping = TextWrapping.NoWrap;
                LTextBlockHeader.Text = App.GetDisplayCharater("M01012");
                AEventArgs.TabItem.Header = LTextBlockHeader;
                AEventArgs.TabItem.Tag = App.GetDisplayCharater("M01011");

                AEventArgs.TabItem.AllowDelete = false;

                ITabItemServerObjectDetail = AEventArgs.TabItem;
                LabelCurrentObjectPath.Content = App.GetDisplayCharater("M01011");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowObjectTabItemHeaderFromBasicTreeViewItem(object AObjectSource)
        {
            string LStrObjectPath = string.Empty;
            TreeViewItem LTreeViewItemCurrentSelected = null;
            TreeViewItem LTreeViewItemParent = null;

            if (AObjectSource == null)
            {
                ITabItemServerObjectDetail.Tag = App.GetDisplayCharater("M01011");
                if (ITabControlMainPanel.SelectedItem == ITabItemServerObjectDetail)
                {
                    LabelCurrentObjectPath.Content = ITabItemServerObjectDetail.Tag.ToString();
                }
                return;
            }
            LTreeViewItemCurrentSelected = AObjectSource as TreeViewItem;
            LStrObjectPath = LTreeViewItemCurrentSelected.Header.ToString();
            LTreeViewItemParent = LTreeViewItemCurrentSelected;
            while (LTreeViewItemParent.Parent.GetType() == typeof(TreeViewItem))
            {
                LTreeViewItemParent = (TreeViewItem)LTreeViewItemParent.Parent;
                LStrObjectPath = LTreeViewItemParent.Header.ToString() + @" \ " + LStrObjectPath;
            }
            ITabItemServerObjectDetail.Tag = LStrObjectPath;
            if (ITabControlMainPanel.SelectedItem == ITabItemServerObjectDetail) { LabelCurrentObjectPath.Content = ITabItemServerObjectDetail.Tag.ToString(); }
        }

        //获取TreeViewItem的ID
        private string GetObjectID000(object AObjectSource)
        {
            string LStrObjectID = "000";
            string LStrObjectName = string.Empty;

            try
            {
                if (AObjectSource == null) { LStrObjectID = "000"; }
                else
                {
                    Type LTypeObject = AObjectSource.GetType();
                    if (LTypeObject == typeof(TreeViewItem))
                    {
                        TreeViewItem LTreeViewItem = AObjectSource as TreeViewItem;
                        LStrObjectName = LTreeViewItem.Name;
                        LStrObjectID = LStrObjectName.Substring(3);
                        if (LStrObjectID == "OTB") { LStrObjectID = "004-1"; }
                        if (LStrObjectID == "OVW" || LStrObjectID == "OFT" || LStrObjectID == "OPD" || LStrObjectID == "OTP") { LStrObjectID = "004-2"; }
                    }
                }
            }
            catch { LStrObjectID = "000"; }

            return LStrObjectID;
        }

        private void ShowObjectInformationInTabItemFromBasicTreeViewItem(object AObjectSource, bool ABoollanguageChanage)
        {
            string LStrTVIObjectID = string.Empty;

            try
            {
                ITabItemServerObjectDetail.Content = null;
                LStrTVIObjectID = GetObjectID000(AObjectSource);

                switch (LStrTVIObjectID)
                {
                    case "002":
                        ShowTVI002UserObject(AObjectSource, ABoollanguageChanage);
                        break;
                    case "003":
                        ShowTVI003UserObject(AObjectSource, ABoollanguageChanage);
                        break;
                    case "004":
                        ShowTVI004UserObject(AObjectSource, ABoollanguageChanage);
                        break;
                    case "007":
                        ShowTVI007UserObject(AObjectSource, ABoollanguageChanage);
                        break;
                    case "006":
                        ShowTVI006UserObject(AObjectSource, ABoollanguageChanage);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        private void ShowTVI003UserObject(object AObjectSource, bool ABoollanguageChanage)
        {
            try
            {
                UCIISBindingListView LUCIISBindingListView = new UCIISBindingListView();
                LUCIISBindingListView.IOperationEvent += LUCObjectDetailOperationsEvent;
                ITabItemServerObjectDetail.Content = LUCIISBindingListView;
                LUCIISBindingListView.DisplayElementCharacters(false);
                LUCIISBindingListView.ShowObjectContainChildObject(AObjectSource, false);
            }
            catch { }
        }

        private void ShowTVI002UserObject(object AObjectSource, bool ABoollanguageChanage)
        {
            try
            {
                UCCertificateListView LUCCertificateListView = new UCCertificateListView();
                LUCCertificateListView.IOperationEvent += LUCObjectDetailOperationsEvent;
                ITabItemServerObjectDetail.Content = LUCCertificateListView;
                LUCCertificateListView.DisplayElementCharacters(false);
                LUCCertificateListView.ShowObjectContainChildObject(AObjectSource, false);
            }
            catch { }
        }

        private void ShowTVI004UserObject(object AObjectSource, bool ABoollanguageChanage)
        {
            try
            {
                UCDatabaseProfileListView LUCDatabaseProfileListView = new UCDatabaseProfileListView();
                LUCDatabaseProfileListView.IOperationEvent += LUCObjectDetailOperationsEvent;
                ITabItemServerObjectDetail.Content = LUCDatabaseProfileListView;
                LUCDatabaseProfileListView.DisplayElementCharacters(false);
                LUCDatabaseProfileListView.ShowObjectContainChildObject(AObjectSource, false);
            }
            catch { }
        }

        private void ShowTVI007UserObject(object AObjectSource, bool ABoollanguageChanage)
        {
            try
            {
                UCLicenseServerView LUCLicenseServerView = new UCLicenseServerView();
                LUCLicenseServerView.IOperationEvent += LUCObjectDetailOperationsEvent;
                ITabItemServerObjectDetail.Content = LUCLicenseServerView;
                LUCLicenseServerView.DisplayElementCharacters(false);
                LUCLicenseServerView.ShowObjectContainChildObject(AObjectSource, false);
            }
            catch { }
        }

        private void ShowTVI006UserObject(object AObjectSource, bool ABoollanguageChanage)
        {
            try
            {
                UCLogicPartitionView LUCLogicPartitionView = new UCLogicPartitionView();
                LUCLogicPartitionView.IOperationEvent += LUCObjectDetailOperationsEvent;
                ITabItemServerObjectDetail.Content = LUCLogicPartitionView;
                LUCLogicPartitionView.DisplayElementCharacters(false);
                LUCLogicPartitionView.ShowObjectContainChildObject(AObjectSource, false);
            }
            catch { }
        }

        private void LUCObjectDetailOperationsEvent(object sender, MamtOperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }
    }
}
