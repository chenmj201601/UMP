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
using UMP.Tools.BasicModule;
using UMP.Tools.LanguageMaintenance;
using UMP.Tools.OnlineUserManagement;
using UMP.Tools.PublicClasses;
using UMP.Tools.ThirdPartyApplications;
using YoungWPFTabControl;

namespace UMP.Tools.BasicControls
{
    public partial class UCFeatureDetails : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        #region WPF TabCotrol
        //TabCotrol控件
        private YoungWPFTabControl.TabControl ITabControlMainPanel = null;

        //增加TabItem的参数
        private OperationEventArgs IOperationEventArgsAddItem = new OperationEventArgs();

        //对象资源管理详细信息TabItem
        YoungWPFTabControl.TabItem ITabItemServerObjectDetail = null;
        #endregion

        //当前传递进来显示的对象参数
        private OperationEventArgs IOperationEventArgs = null;

        public UCFeatureDetails()
        {
            InitializeComponent();

            this.Loaded += UCFeatureDetails_Loaded;
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
                    if (!ABoolLanguageChange)
                    {
                        ShowObjectDetailsInTabItemServerObject(IOperationEventArgs, ABoolLanguageChange);
                    }
                }
                try
                {
                    TextBlock LTextBlockHeader = (TextBlock)ITabItemServerObjectDetail.Header;
                    LTextBlockHeader.Text = App.GetDisplayCharater("M01012");
                }
                catch { }
            }
            catch { }

        }

        public void OpenServerObjectSingleInformation(object AObjectSender, OperationEventArgs AOperationEventArgs)
        {
            try
            {
                IOperationEventArgsAddItem = AOperationEventArgs;

                //这里需要判断要打开的对象是否已经打开，如果打开，则跳转到已经打开的页面，考虑是否可以放到MainWindows中处理该判断逻辑

                ITabControlMainPanel.AddTabItem();
            }
            catch { }
        }

        public void ShowObjectDetailsInTabItemServerObject(OperationEventArgs AObjectArgs, bool ABoollanguageChanage)
        {
            try
            {
                IOperationEventArgs = AObjectArgs;
                if (AObjectArgs.StrElementTag == "TSCH")            //UCObject TreeViewItem SelectedItemChanged
                {
                    ShowObjectTabItemHeaderFromBasicTreeViewItem(AObjectArgs.ObjSource);
                    ShowObjectInformationInTabItemFromBasicTreeViewItem(AObjectArgs.ObjSource, ABoollanguageChanage);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SetTabItemServerObjectDetailToolTip");
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

        private void ShowObjectInformationInTabItemFromBasicTreeViewItem(object AObjectSource, bool ABoollanguageChanage)
        {
            string LStrTVIObjectIDFull = string.Empty;
            string LStrTVIObjectIDLeft3 = string.Empty;
            string LStrTVIObjectIDAfter3 = string.Empty;

            try
            {
                ITabItemServerObjectDetail.Content = null;
                LStrTVIObjectIDFull = GetObjectID000(AObjectSource);
                LStrTVIObjectIDLeft3 = LStrTVIObjectIDFull.Substring(0, 3);
                LStrTVIObjectIDAfter3 = LStrTVIObjectIDFull.Substring(3);

                switch (LStrTVIObjectIDLeft3)
                {
                    case "202":
                        ShowTVI202Object(AObjectSource, LStrTVIObjectIDAfter3, ABoollanguageChanage);
                        break;
                    case "401":
                        ShowTVI401Object(AObjectSource, LStrTVIObjectIDAfter3, ABoollanguageChanage);
                        break;
                    case "502":
                        ShowTVI502Object(AObjectSource, LStrTVIObjectIDAfter3, ABoollanguageChanage);
                        break;
                    case "602":
                        ShowTVI602Object(AObjectSource, LStrTVIObjectIDAfter3, ABoollanguageChanage);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        #region 获取TreeViewItem的ID
        /// <summary>
        /// 获取TreeViewItem的ID
        /// </summary>
        /// <param name="AObjectSource"></param>
        /// <returns></returns>
        private string GetObjectID000(object AObjectSource)
        {
            string LStrObjectID = "000";
            string LStrObjectName = string.Empty;

            try
            {
                if (AObjectSource == null) { return LStrObjectID; }
                Type LTypeObject = AObjectSource.GetType();
                if (LTypeObject == typeof(TreeViewItem))
                {
                    TreeViewItem LTreeViewItem = AObjectSource as TreeViewItem;
                    LStrObjectName = LTreeViewItem.Name;
                    LStrObjectID = LStrObjectName.Substring(3);
                }
            }
            catch { LStrObjectID = "000"; }

            return LStrObjectID;
        }
        #endregion

        #region 显示语言包主窗口
        private void ShowTVI202Object(object AObjectSource, string AStrTVIObjectIDAfter3, bool ABoollanguageChanage)
        {
            try
            {
                if (ABoollanguageChanage) { return; }
                if (AStrTVIObjectIDAfter3.Substring(1, 1) == "0") { return; }
                LanguageTranslatorMain LLanguageTranslatorMain = new LanguageTranslatorMain();
                LLanguageTranslatorMain.IOperationEvent += ObjectDetailContentIOperationEvent;
                ITabItemServerObjectDetail.Content = LLanguageTranslatorMain;
                if(AStrTVIObjectIDAfter3.Substring(0, 1) == "1")
                {
                    TreeViewItem LTreeViewItemLanguage = AObjectSource as TreeViewItem;
                    DataTable LDataTableLanguage = LTreeViewItemLanguage.DataContext as DataTable;
                    LLanguageTranslatorMain.WriteDataTable2ListView(LDataTableLanguage);
                }
            }
            catch { }
        }
        #endregion

        #region 显示在线用户列表
        private void ShowTVI502Object(object AObjectSource, string AStrTVIObjectIDAfter3, bool ABoollanguageChanage)
        {
            try
            {
                if (ABoollanguageChanage) { return; }
                OnlineUserListViews LOnlineUserListViews = new OnlineUserListViews();
                LOnlineUserListViews.IOperationEvent += ObjectDetailContentIOperationEvent;
                ITabItemServerObjectDetail.Content = LOnlineUserListViews;
                TreeViewItem LTreeViewItemRent = AObjectSource as TreeViewItem;
                DataTable LDataTableOnlineUser = LTreeViewItemRent.DataContext as DataTable;
                LOnlineUserListViews.WriteDataTable2ListView(LDataTableOnlineUser);
            }
            catch { }
        }
        #endregion

        #region 显示离线语言包
        private void ShowTVI401Object(object AObjectSource, string AStrTVIObjectIDAfter3, bool ABoollanguageChanage)
        {
            string LStrOfflineFileType = string.Empty;
            try
            {
                if (ABoollanguageChanage) { return; }
                TreeViewItem LTreeViewItemOfflineFile = AObjectSource as TreeViewItem;
                if (LTreeViewItemOfflineFile.DataContext == null) { return; }
                OperationEventArgs LOperationEventArgs = (OperationEventArgs)LTreeViewItemOfflineFile.DataContext;
                LStrOfflineFileType = LOperationEventArgs.AppenObjeSource3.ToString();
                if (LStrOfflineFileType == "01")
                {
                    OfflineLanguageFileType01 LOfflineLanguageFileType01 = new OfflineLanguageFileType01();
                    LOfflineLanguageFileType01.IOperationEvent += ObjectDetailContentIOperationEvent;
                    ITabItemServerObjectDetail.Content = LOfflineLanguageFileType01;
                    LOfflineLanguageFileType01.WriteDataTable2ListView(LOperationEventArgs);
                }
            }
            catch { }
        }
        #endregion

        #region 显示第三方应用信息
        private void ShowTVI602Object(object AObjectSource, string AStrTVIObjectIDAfter3, bool ABoollanguageChanage)
        {
            string LStrThirdPartyAppName = string.Empty;

            try
            {
                if (ABoollanguageChanage) { return; }
                TreeViewItem LTreeViewItemThirdParty = AObjectSource as TreeViewItem;
                DataRow LDataRowThirdPartyInfo = LTreeViewItemThirdParty.DataContext as DataRow;
                LStrThirdPartyAppName = LDataRowThirdPartyInfo["Attribute00"].ToString();

                if (LStrThirdPartyAppName == "ASM")
                {
                    UCThirdPartyASM LUCThirdPartyASM = new UCThirdPartyASM();
                    LUCThirdPartyASM.IOperationEvent += ObjectDetailContentIOperationEvent;
                    ITabItemServerObjectDetail.Content = LUCThirdPartyASM;
                    LUCThirdPartyASM.WriteData2View(LDataRowThirdPartyInfo);
                }
            }
            catch { }
        }
        #endregion

        private void ObjectDetailContentIOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }

        #region 初始化事件
        private void UCFeatureDetails_Loaded(object sender, RoutedEventArgs e)
        {
            ImageFeatureDetailTip.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000012.ico"), UriKind.RelativeOrAbsolute));
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
        #endregion
    }
}
