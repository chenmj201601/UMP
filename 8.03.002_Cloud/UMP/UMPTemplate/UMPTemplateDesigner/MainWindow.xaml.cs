using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VoiceCyber.Common;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls.Design;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.PropertyGrids;
using VoiceCyber.Wpf.PropertyGrids.Definitions;
using VoiceCyber.Wpf.PropertyGrids.Editors;
using MenuItem = VoiceCyber.Ribbon.MenuItem;
using MessageBox = System.Windows.MessageBox;
using PropertyChangedEventArgs = VoiceCyber.UMP.ScoreSheets.PropertyChangedEventArgs;
using WindowState = System.Windows.WindowState;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region Members

        private ObjectItem mRootObject;
        private long mObjectID;
        public ObservableCollection<ScoreItem> ListControlSourceItems { get; set; }
        public ObservableCollection<ScoreItem> ListControlTargetItems { get; set; }
        public static ObservableCollection<ScoreSetting> ListScoreIcons { get; set; }
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private ObservableCollection<LanguageTypeItem> mListLanguageTypes;
        private ScoreDocument mScoreDocument;
        private int mLanguageID;
        private double mViewerScale;
        private int mDragType;
        private bool mIsChanged;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            ListControlSourceItems = new ObservableCollection<ScoreItem>();
            ListControlTargetItems = new ObservableCollection<ScoreItem>();
            ListScoreIcons = new ObservableCollection<ScoreSetting>();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListLanguageTypes = new ObservableCollection<LanguageTypeItem>();
            mObjectID = 0;
            mLanguageID = 2052;
            mViewerScale = 1;
            mDragType = 0;
            mIsChanged = false;
            mRootObject = new ObjectItem();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            CreateNewButtons();
            BindEventHandler();
            InitScoreSettings();
            LoadScoreIcons();
            LoadLanguageInfos();
            InitLanuageTypes();
            TvObjects.ItemsSource = mRootObject.Children;
            GalleryLanguages.ItemsSource = mListLanguageTypes;
            SliderScale.Tag = mViewerScale;
            CbObjects.IsChecked = false;
            CbProperty.IsChecked = false;
            CbViewHead.IsChecked = false;
            SetPanelView();
            SetDragDropState();
            ShowLanguages();
            LanguageTypeItem languageType = mListLanguageTypes.FirstOrDefault(l => l.LangID == mLanguageID);
            if (languageType != null)
            {
                GalleryLanguages.SelectedItem = languageType;
            }
            WindowState = WindowState.Maximized;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (mIsChanged)
            {
                var result = MessageBox.Show(string.Format("{0}", GetLanguage("Designer", "N_013", "ScoreSheet has changed. Do you want to save?")),
                    "UMPScoreSheetDesigner", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    BtnSave_Click(this, null);
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #region EventHandlers

        private void BindEventHandler()
        {
            TvObjects.SelectedItemChanged += TvObjects_OnSelectedItemChanged;
            BtnClose.Click += BtnClose_Click;
            BtnAbout.Click += BtnAbout_Click;
            BtnOpen.Click += BtnOpen_Click;
            BtnSave.Click += BtnSave_Click;
            BtnSaveTo.Click += BtnSaveTo_Click;
            BtnDelete.Click += BtnDelete_Click;
            CbObjects.Click += CbView_Click;
            CbProperty.Click += CbView_Click;
            CbViewHead.Click += CbView_Click;
            BtnPrint.Click += BtnPrint_Click;
            BtnCaculate.Click += BtnCaculate_Click;
            SliderScale.ValueChanged += SliderScale_ValueChanged;
            GalleryLanguages.SelectionChanged += GalleryLanguages_SelectionChanged;
            RadioDown.Click += RadioDragType_Click;
            RadioUp.Click += RadioDragType_Click;
            RadioChild.Click += RadioDragType_Click;
        }

        void RadioDragType_Click(object sender, RoutedEventArgs e)
        {
            var radio = e.Source as RadioButton;
            if (radio != null)
            {
                var type = radio.Tag.ToString();
                if (type == "0")
                {
                    mDragType = 0;
                }
                else if (type == "1")
                {
                    mDragType = 1;
                }
                else
                {
                    mDragType = 2;
                }
            }
            SetDragDropState();
        }

        void GalleryLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                LanguageTypeItem item = e.AddedItems[0] as LanguageTypeItem;
                if (item != null)
                {
                    mLanguageID = item.LangID;
                    ShowLanguages();

                    TvObjects_OnSelectedItemChanged(this, null);
                }
            }
        }

        void SliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int value;
                if (int.TryParse(SliderScale.Value.ToString(), out value))
                {
                    switch (value)
                    {
                        case 10:
                            mViewerScale = 0.2;
                            break;
                        case 15:
                            mViewerScale = 0.3;
                            break;
                        case 20:
                            mViewerScale = 0.4;
                            break;
                        case 25:
                            mViewerScale = 0.5;
                            break;
                        case 30:
                            mViewerScale = 0.6;
                            break;
                        case 35:
                            mViewerScale = 0.7;
                            break;
                        case 40:
                            mViewerScale = 0.8;
                            break;
                        case 45:
                            mViewerScale = 0.9;
                            break;
                        case 50:
                            mViewerScale = 1.0;
                            break;
                        case 55:
                            mViewerScale = 1.5;
                            break;
                        case 60:
                            mViewerScale = 2.0;
                            break;
                        case 65:
                            mViewerScale = 2.5;
                            break;
                        case 70:
                            mViewerScale = 3.0;
                            break;
                        case 75:
                            mViewerScale = 3.5;
                            break;
                        case 80:
                            mViewerScale = 4.0;
                            break;
                        case 85:
                            mViewerScale = 4.5;
                            break;
                        case 90:
                            mViewerScale = 5.0;
                            break;
                    }
                }
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = mViewerScale;
                tran.ScaleY = mViewerScale;
                BorderViewer.LayoutTransform = tran;
                SliderScale.Tag = mViewerScale;
                BindingExpression be = SliderScale.GetBindingExpression(ToolTipProperty);
                if (be != null)
                {
                    be.UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnCaculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectObj = TvObjects.SelectedItem as ObjectItem;
                if (selectObj != null)
                {
                    string invalidMessage;
                    CheckValidResult result;
                    ScoreSheet scoreSheet = selectObj.Data as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        result = scoreSheet.CheckValid();
                        selectObj.InvalidCode = result.Code;
                        if (result.Code != 0)
                        {
                            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", result.Code), result.Message), result.ScoreObject);
                            selectObj.InvalidMessage = invalidMessage;
                            ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        double score = scoreSheet.CaculateScore();
                        ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_002", "Score:"), score.ToString("0.00")));
                        return;
                    }
                    ScoreGroup scoreGroup = selectObj.Data as ScoreGroup;
                    if (scoreGroup != null)
                    {
                        result = scoreGroup.CheckValid();
                        selectObj.InvalidCode = result.Code;
                        if (result.Code != 0)
                        {
                            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", result.Code), result.Message), result.ScoreObject);
                            selectObj.InvalidMessage = invalidMessage;
                            ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        double score = scoreGroup.CaculateScore();
                        ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_002", "Score:"), score.ToString("0.00")));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(BorderViewer.Child, string.Empty);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void CbView_Click(object sender, RoutedEventArgs e)
        {
            SetPanelView();
        }

        private void TvObjects_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CreateNewButtons();
            var obj = TvObjects.SelectedItem;
            ObjectItem item = obj as ObjectItem;
            if (item == null) { return; }
            ImageObject.DataContext = item;
            LbTitle.DataContext = item;
            ScoreObject scoreObject = item.Data as ScoreObject;
            if (scoreObject != null)
            {
                if (scoreObject.Type == ScoreObjectType.ControlItem)
                {
                    //初始化可用的评分项源和目标
                    ControlItem controlItem = scoreObject as ControlItem;
                    if (controlItem != null)
                    {
                        ScoreSheet scoreSheet = controlItem.ScoreSheet;
                        if (scoreSheet != null)
                        {
                            List<ScoreItem> listScoreItems = new List<ScoreItem>();
                            scoreSheet.GetAllScoreItem(ref listScoreItems);
                            ListControlSourceItems.Clear();
                            for (int i = 0; i < listScoreItems.Count; i++)
                            {
                                ListControlSourceItems.Add(listScoreItems[i]);
                            }
                            listScoreItems = new List<ScoreItem>();
                            scoreSheet.GetAllScoreItem(ref listScoreItems);
                            ListControlTargetItems.Clear();
                            for (int i = 0; i < listScoreItems.Count; i++)
                            {
                                ListControlTargetItems.Add(listScoreItems[i]);
                            }
                        }
                    }
                }
                ScoreObjectViewer scoreObjectViewer = new ScoreObjectViewer();
                scoreObjectViewer.ScoreObject = scoreObject;
                scoreObjectViewer.Settings = mListScoreSettings;
                scoreObjectViewer.Languages = mListLanguageInfos;
                scoreObjectViewer.LangID = mLanguageID;
                BorderViewer.Child = scoreObjectViewer;
                PrepareProperties(scoreObject);
                ObjectProperty.SelectedObject = null;
                ObjectProperty.SelectedObject = scoreObject;
                ObjectProperty.SelectedObjectTypeName = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", scoreObject.GetType().Name), scoreObject.GetType().Name);
                ObjectProperty.SelectedObjectName = scoreObject.ToString();
            }
            if (CbViewHead.IsChecked == null || CbViewHead.IsChecked != true)
            {
                CbViewHead.IsChecked = true;
            }
            if (CbProperty.IsChecked == null || CbProperty.IsChecked != true)
            {
                CbProperty.IsChecked = true;
            }
            SetPanelView();
        }

        private void NewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as MenuItem;
            if (btn == null) { return; }
            var newBtnItem = btn.DataContext as NewButtonItem;
            if (newBtnItem == null) { return; }
            switch (newBtnItem.Name)
            {
                case "ScoreSheet":
                    NewScoreSheet();
                    break;
                case "ScoreGroup":
                    NewScoreGroup();
                    break;
                case "NumericStandard":
                    NewNumericStandard();
                    break;
                case "YesNoStandard":
                    NewYesNoStandard();
                    break;
                case "SliderStandard":
                    NewSliderStandard();
                    break;
                case "ItemStandard":
                    NewItemStandard();
                    break;
                case "StandardItem":
                    NewStandardItem();
                    break;
                case "TextComment":
                    NewTextComment();
                    break;
                case "ItemComment":
                    NewItemComment();
                    break;
                case "CommentItem":
                    NewCommentItem();
                    break;
                case "ControlItem":
                    NewControlItem();
                    break;
            }
            BtnNew.IsDropDownOpen = false;
            CreateNewButtons();
            mIsChanged = true;
            if (CbObjects.IsChecked != true)
            {
                CbObjects.IsChecked = true;
                SetPanelView();
            }
        }

        void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mRootObject.Children.Count <= 0) { return; }
                ObjectItem item = mRootObject.Children[0];
                var scoreSheet = item.Data as ScoreSheet;
                if (scoreSheet == null) { return; }
                string invalidMessage;
                CheckValidResult checkResult = scoreSheet.CheckValid();
                item.InvalidCode = checkResult.Code;
                if (checkResult.Code != 0)
                {
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    item.InvalidMessage = invalidMessage;
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N_001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N_003", "Confirm to save ScoreSheet?")),
                          "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmResult != MessageBoxResult.Yes) { return; }
                }
                if (mScoreDocument == null || mScoreDocument.ScoreObject != scoreSheet)
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    dialog.Filter = "XML File(*.xml)|*.xml|All Files(*.*)|*.*";
                    var result = dialog.ShowDialog();
                    if (result != true) { return; }
                    mScoreDocument = new ScoreDocument();
                    string fullName = dialog.FileName;
                    string folderPath = fullName.Substring(0, fullName.LastIndexOf('\\'));
                    string fileName = fullName.Substring(fullName.LastIndexOf('\\') + 1);
                    fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
                    mScoreDocument.Name = fileName;
                    mScoreDocument.FolderPath = folderPath;
                    mScoreDocument.FullPath = dialog.FileName;
                    mScoreDocument.ScoreObject = scoreSheet;
                    SaveScoreObject(mScoreDocument);
                }
                else
                {
                    SaveScoreObject(mScoreDocument);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnSaveTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mRootObject.Children.Count <= 0) { return; }
                ObjectItem item = mRootObject.Children[0];
                var scoreSheet = item.Data as ScoreSheet;
                if (scoreSheet == null) { return; }
                string invalidMessage;
                CheckValidResult checkResult = scoreSheet.CheckValid();
                item.InvalidCode = checkResult.Code;
                if (checkResult.Code != 0)
                {
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    item.InvalidMessage = invalidMessage;
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N_001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N_003", "Confirm to save ScoreSheet?")),
                           "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmResult != MessageBoxResult.Yes) { return; }
                }
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Filter = "XML File(*.xml)|*.xml|All Files(*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result != true) { return; }
                mScoreDocument = new ScoreDocument();
                string fullName = dialog.FileName;
                string fileName = fullName.Substring(fullName.LastIndexOf('\\') + 1);
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
                mScoreDocument.Name = fileName;
                mScoreDocument.FullPath = dialog.FileName;
                mScoreDocument.ScoreObject = scoreSheet;
                SaveScoreObject(mScoreDocument);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "XML File(*.xml)|*.xml|All Files(*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == true)
            {
                //已经打开
                if (mScoreDocument != null && mScoreDocument.FullPath == dialog.FileName)
                {
                    ShowInfoMessage(string.Format("{0}", GetLanguage("Designer", "N_005", "ScoreSheet already opened.")));
                    return;
                }
                if (mRootObject != null && mRootObject.Children.Count > 0)
                {
                    var openResult = MessageBox.Show(string.Format("{0}", GetLanguage("Designer", "N_004", "Save the exist ScoreSheet?")), "UMPScoreSheetDesigner",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (openResult == MessageBoxResult.Yes)
                    {
                        BtnSave_Click(this, null);
                    }
                    mRootObject.Children.Clear();
                    mScoreDocument = null;
                }
                LoadScoreSheet(dialog.FileName);
                mIsChanged = false;
            }
        }

        void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = TvObjects.SelectedItem;
            if (selectedItem == null) { return; }
            var selectedObj = selectedItem as ObjectItem;
            if (selectedObj == null) { return; }
            var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", GetLanguage("Designer", "N_006", "Confirm Delete?"), selectedObj.Display), "UMP Score Designer", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ObjectItem parent = selectedObj.Parent;
                if (parent != null)
                {
                    ScoreGroup scoreGroup = parent.Data as ScoreGroup;
                    ScoreItem scoreItem = selectedObj.Data as ScoreItem;
                    if (scoreGroup != null && scoreItem != null)
                    {
                        scoreGroup.Items.Remove(scoreItem);
                        ObjectItem_PropertyChanged(selectedObj, scoreItem);
                    }
                    mIsChanged = true;
                    ItemStandard itemStandard = parent.Data as ItemStandard;
                    StandardItem standardItem = selectedObj.Data as StandardItem;
                    if (itemStandard != null && standardItem != null)
                    {
                        itemStandard.ValueItems.Remove(standardItem);
                    }
                    parent.RemoveChild(selectedObj);
                    if (selectedObj.ObjType == (int)ScoreObjectType.ScoreSheet)
                    {
                        mScoreDocument = null;
                    }
                    BorderViewer.Child = null;
                    ObjectProperty.SelectedObject = null;
                    LbTitle.DataContext = null;
                    ImageObject.DataContext = null;
                }
            }
        }

        void BtnAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ObjectProperty_OnSelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            var property = ObjectProperty.SelectedPropertyItem as PropertyItem;
            if (property != null && property.PropertyDescriptor.Name == "TitleStyle")
            {
                ScoreItem scoreItem = ObjectProperty.SelectedObject as ScoreItem;
                if (scoreItem != null && scoreItem.TitleStyle == null)
                {
                    scoreItem.TitleStyle = new VisualStyle
                    {
                        Type = ScoreObjectType.VisualStyle,
                        ID = GetID(),
                        ScoreObject = scoreItem,
                        ForeColor = Colors.Black,
                    };
                    ObjectProperty.SelectedObject = null;
                    ObjectProperty.SelectedObject = scoreItem;
                }
                Comment comment = ObjectProperty.SelectedObject as Comment;
                if (comment != null && comment.TitleStyle == null)
                {
                    comment.TitleStyle = new VisualStyle
                    {
                        Type = ScoreObjectType.VisualStyle,
                        ID = GetID(),
                        ScoreObject = comment,
                        ForeColor = Colors.Black,
                    };
                    ObjectProperty.SelectedObject = null;
                    ObjectProperty.SelectedObject = comment;
                }
            }
            if (property != null && property.PropertyDescriptor.Name == "PanelStyle")
            {
                ScoreItem scoreItem = ObjectProperty.SelectedObject as ScoreItem;
                if (scoreItem != null && scoreItem.PanelStyle == null)
                {
                    scoreItem.PanelStyle = new VisualStyle
                    {
                        Type = ScoreObjectType.VisualStyle,
                        ID = GetID(),
                        ScoreObject = scoreItem,
                        BackColor = Colors.Transparent
                    };
                    ObjectProperty.SelectedObject = null;
                    ObjectProperty.SelectedObject = scoreItem;
                }
                Comment comment = ObjectProperty.SelectedObject as Comment;
                if (comment != null && comment.PanelStyle == null)
                {
                    comment.PanelStyle = new VisualStyle
                    {
                        Type = ScoreObjectType.VisualStyle,
                        ID = GetID(),
                        ScoreObject = comment,
                        ForeColor = Colors.Black,
                    };
                    ObjectProperty.SelectedObject = null;
                    ObjectProperty.SelectedObject = comment;
                }
            }
        }

        private void ObjectProperty_OnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            var propertyItem = e.PropertyItem as PropertyItem;
            if (propertyItem == null) { return; }
            if (propertyItem.PropertyDescriptor.Name == "FontSize")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_FontSize", "FontSize");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_FontSize", "FontSize");
            }
            if (propertyItem.PropertyDescriptor.Name == "FontWeight")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_FontWeight", "FontWeight");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_FontWeight", "FontWeight");
            }
            if (propertyItem.PropertyDescriptor.Name == "FontFamily")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_FontFamily", "FontFamily");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_FontFamily", "FontFamily");
            }
            if (propertyItem.PropertyDescriptor.Name == "ForeColor")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_ForeColor", "ForeColor");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_ForeColor", "ForeColor");
            }
            if (propertyItem.PropertyDescriptor.Name == "BackColor")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_BackColor", "BackColor");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_BackColor", "BackColor");
            }
            if (propertyItem.PropertyDescriptor.Name == "Width")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_Width", "Width");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_Width", "Width");
            }
            if (propertyItem.PropertyDescriptor.Name == "Height")
            {
                propertyItem.DisplayName = GetLanguage("PropertyViewer", "P_Display_Height", "Height");
                propertyItem.Description = GetLanguage("PropertyViewer", "P_Description_Height", "Height");
            }
        }

        private void ObjectProperty_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ScoreObject scoreObject = ObjectProperty.SelectedObject as ScoreObject;
            if (scoreObject != null)
            {
                PropertyItem item = e.OriginalSource as PropertyItem;
                if (item != null)
                {
                    string propertyName = item.PropertyDescriptor.Name;
                    if (propertyName == "StandardType")
                    {
                        Standard standard = scoreObject as Standard;
                        //此处有缺陷，暂且注释
                        ////多值型评分标准切换成其他类型评分标准，提示会将所有子项清除
                        //ItemStandard itemStandard = standard as ItemStandard;
                        //if (itemStandard != null)
                        //{
                        //    var result =
                        //        MessageBox.Show(
                        //            string.Format(
                        //                "{0}", GetLanguage("Designer", "N_014", "ItemStandard change to other Standard will clear all StandardItem. Confirm to changed standard type?")),
                        //            "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        //    if (result != MessageBoxResult.Yes)
                        //    {
                        //        standard.StandardType = (StandardType)e.OldValue;
                        //        return;
                        //    }
                        //}
                        StandardType newType = (StandardType)e.NewValue;
                        if (standard != null)
                        {
                            ChangeStandardType(standard, newType);
                            mIsChanged = true;
                            return;
                        }
                    }
                    PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                    args.ScoreObject = scoreObject;
                    args.NewValue = e.NewValue;
                    args.OldValue = e.OldValue;
                    args.PropertyName = propertyName;
                    scoreObject.PropertyChanged(this, args);
                    mIsChanged = true;
                    var obj = TvObjects.SelectedItem as ObjectItem;
                    if (obj != null)
                    {
                        ObjectItem_PropertyChanged(obj, scoreObject);
                    }
                }
            }
        }

        private void ObjectItem_PropertyChanged(object sender, ScoreObject scoreObject)
        {
            var objectItem = sender as ObjectItem;
            if (objectItem != null)
            {
                var scoreItem = scoreObject as ScoreItem;
                if (scoreItem != null)
                {
                    objectItem.Display = scoreItem.Title;
                    objectItem.ToolTip = scoreItem.Title;
                    CheckObjectItemValid(objectItem);
                }
                var standardItem = scoreObject as StandardItem;
                if (standardItem != null)
                {
                    objectItem.Display = standardItem.Display;
                    objectItem.ToolTip = standardItem.Display;
                }
                var comment = scoreObject as Comment;
                if (comment != null)
                {
                    objectItem.Display = comment.Title;
                    objectItem.ToolTip = comment.Title;
                }
                var commentItem = scoreObject as CommentItem;
                if (commentItem != null)
                {
                    objectItem.Display = commentItem.Text;
                    objectItem.ToolTip = commentItem.Text;
                }
                var controlItem = scoreObject as ControlItem;
                if (controlItem != null)
                {
                    objectItem.Display = controlItem.Title;
                    objectItem.ToolTip = controlItem.Title;
                }
            }
        }

        #endregion

        #region NewScoreObject

        private void NewScoreSheet()
        {
            try
            {
                ScoreSheet scoreSheet = new ScoreSheet();
                scoreSheet.ID = GetID();
                scoreSheet.Type = ScoreObjectType.ScoreSheet;
                scoreSheet.Title = "New ScoreSheet";
                scoreSheet.ViewClassic = ScoreItemClassic.Table;
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.TotalScore = 100.00;
                scoreSheet.Creator = GetID();
                scoreSheet.CreateTime = DateTime.Now;
                scoreSheet.Status = "Y";
                scoreSheet.DateFrom = DateTime.Now;
                scoreSheet.DateTo = DateTime.MaxValue;
                scoreSheet.UseTag = 0;
                scoreSheet.QualifiedLine = 0;
                scoreSheet.ScoreType = ScoreType.Numeric;
                scoreSheet.Flag = 0;

                mRootObject.IsExpanded = true;
                CreateScoreSheetItem(mRootObject, scoreSheet, false);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewScoreGroup()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreGroup scoreGroup = selectedItem.Data as ScoreGroup;
                if (scoreGroup == null) { return; }
                ScoreSheet scoreSheet = scoreGroup.ScoreSheet;
                ScoreGroup newGroup = new ScoreGroup();
                newGroup.ID = GetID();
                newGroup.Type = ScoreObjectType.ScoreGroup;
                newGroup.Title = "New Group";
                newGroup.ViewClassic = scoreSheet.ViewClassic;
                newGroup.ScoreType = scoreSheet.ScoreType;
                newGroup.Parent = scoreGroup;
                newGroup.ScoreSheet = scoreSheet;
                newGroup.UsePointSystem = scoreSheet.UsePointSystem;
                scoreGroup.Items.Add(newGroup);

                selectedItem.IsExpanded = true;
                CreateScoreGroupItem(selectedItem, newGroup, false);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewNumericStandard()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreGroup scoreGroup = selectedItem.Data as ScoreGroup;
                if (scoreGroup == null) { return; }
                ScoreSheet scoreSheet = scoreGroup.ScoreSheet;
                NumericStandard newStandard = new NumericStandard();
                newStandard.ID = GetID();
                newStandard.Type = ScoreObjectType.NumericStandard;
                newStandard.Title = "New Numeric Standard";
                newStandard.ViewClassic = scoreSheet.ViewClassic;
                newStandard.ScoreType = scoreSheet.ScoreType;
                newStandard.Parent = scoreGroup;
                newStandard.ScoreSheet = scoreSheet;
                newStandard.UsePointSystem = scoreSheet.UsePointSystem;
                newStandard.StandardType = StandardType.Numeric;
                newStandard.ScoreClassic = StandardClassic.TextBox;
                newStandard.DefaultValue = 0;
                newStandard.MaxValue = int.MaxValue;
                newStandard.MinValue = int.MinValue;
                scoreGroup.Items.Add(newStandard);

                selectedItem.IsExpanded = true;
                CreateNumericStandardItem(selectedItem, newStandard, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewYesNoStandard()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreGroup scoreGroup = selectedItem.Data as ScoreGroup;
                if (scoreGroup == null) { return; }
                ScoreSheet scoreSheet = scoreGroup.ScoreSheet;
                YesNoStandard yesNoStandard = new YesNoStandard();
                yesNoStandard.ID = GetID();
                yesNoStandard.Type = ScoreObjectType.YesNoStandard;
                yesNoStandard.Title = "New YesNo Standard";
                yesNoStandard.ViewClassic = scoreSheet.ViewClassic;
                yesNoStandard.ScoreType = scoreSheet.ScoreType;
                yesNoStandard.Parent = scoreGroup;
                yesNoStandard.ScoreSheet = scoreSheet;
                yesNoStandard.UsePointSystem = scoreSheet.UsePointSystem;
                yesNoStandard.StandardType = StandardType.YesNo;
                yesNoStandard.ScoreClassic = StandardClassic.YesNo;
                yesNoStandard.DefaultValue = false;
                scoreGroup.Items.Add(yesNoStandard);

                selectedItem.IsExpanded = true;
                CreateYesNoStandardItem(selectedItem, yesNoStandard, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewItemStandard()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreGroup scoreGroup = selectedItem.Data as ScoreGroup;
                if (scoreGroup == null) { return; }
                ScoreSheet scoreSheet = scoreGroup.ScoreSheet;
                ItemStandard itemStandard = new ItemStandard();
                itemStandard.ID = GetID();
                itemStandard.Type = ScoreObjectType.ItemStandard;
                itemStandard.Title = "New Item Standard";
                itemStandard.ViewClassic = scoreSheet.ViewClassic;
                itemStandard.ScoreType = scoreSheet.ScoreType;
                itemStandard.Parent = scoreGroup;
                itemStandard.ScoreSheet = scoreSheet;
                itemStandard.UsePointSystem = scoreSheet.UsePointSystem;
                itemStandard.StandardType = StandardType.Item;
                itemStandard.ScoreClassic = StandardClassic.DropDownList;
                scoreGroup.Items.Add(itemStandard);

                selectedItem.IsExpanded = true;
                CreateItemStandardItem(selectedItem, itemStandard, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewSliderStandard()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreGroup scoreGroup = selectedItem.Data as ScoreGroup;
                if (scoreGroup == null) { return; }
                ScoreSheet scoreSheet = scoreGroup.ScoreSheet;
                SliderStandard sliderStandard = new SliderStandard();
                sliderStandard.ID = GetID();
                sliderStandard.Type = ScoreObjectType.SliderStandard;
                sliderStandard.Title = "New Slider Standard";
                sliderStandard.ViewClassic = scoreSheet.ViewClassic;
                sliderStandard.ScoreType = scoreSheet.ScoreType;
                sliderStandard.Parent = scoreGroup;
                sliderStandard.ScoreSheet = scoreSheet;
                sliderStandard.UsePointSystem = scoreSheet.UsePointSystem;
                sliderStandard.StandardType = StandardType.Slider;
                sliderStandard.ScoreClassic = StandardClassic.Slider;
                scoreGroup.Items.Add(sliderStandard);

                selectedItem.IsExpanded = true;
                CreateSliderStandardItem(selectedItem, sliderStandard, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewStandardItem()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ItemStandard itemStandard = selectedItem.Data as ItemStandard;
                if (itemStandard == null) { return; }
                StandardItem standardItem = new StandardItem();
                standardItem.ID = GetID();
                standardItem.Type = ScoreObjectType.StandardItem;
                standardItem.Display = "New StandardItem";
                standardItem.ScoreItem = itemStandard;
                itemStandard.ValueItems.Add(standardItem);

                selectedItem.IsExpanded = true;
                CreateStandardItemItem(selectedItem, standardItem);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewTextComment()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreItem scoreItem = selectedItem.Data as ScoreItem;
                if (scoreItem == null) { return; }
                TextComment textComment = new TextComment();
                textComment.Type = ScoreObjectType.TextComment;
                textComment.ID = GetID();
                textComment.ScoreItem = scoreItem;
                textComment.Title = "New Text Comment";
                textComment.Style = CommentStyle.Text;
                scoreItem.Comments.Add(textComment);

                selectedItem.IsExpanded = true;
                CreateTextCommentItem(selectedItem, textComment);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewItemComment()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreItem scoreItem = selectedItem.Data as ScoreItem;
                if (scoreItem == null) { return; }
                ItemComment itemComment = new ItemComment();
                itemComment.Type = ScoreObjectType.ItemComment;
                itemComment.ID = GetID();
                itemComment.ScoreItem = scoreItem;
                itemComment.Title = "New Item Comment";
                itemComment.Style = CommentStyle.Item;
                scoreItem.Comments.Add(itemComment);

                selectedItem.IsExpanded = true;
                CreateItemCommentItem(selectedItem, itemComment, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewCommentItem()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ItemComment itemComment = selectedItem.Data as ItemComment;
                if (itemComment == null) { return; }

                CommentItem commentItem = new CommentItem();
                commentItem.Type = ScoreObjectType.CommentItem;
                commentItem.ID = GetID();
                commentItem.Text = "New Comment Item";
                commentItem.Comment = itemComment;
                itemComment.ValueItems.Add(commentItem);

                selectedItem.IsExpanded = true;
                CreateCommentItemItem(selectedItem, commentItem);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewControlItem()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreSheet scoreSheet = selectedItem.Data as ScoreSheet;
                if (scoreSheet == null) { return; }

                ControlItem controlItem = new ControlItem();
                controlItem.ID = GetID();
                controlItem.Type = ScoreObjectType.ControlItem;
                controlItem.Title = "New Control Item";
                controlItem.ScoreSheet = scoreSheet;
                controlItem.OrderID = scoreSheet.ControlItems.Count;
                scoreSheet.ControlItems.Add(controlItem);

                selectedItem.IsExpanded = true;
                CreateControlItemItem(selectedItem, controlItem);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion

        #region CreateObjectItem

        private void CreateScoreSheetItem(ObjectItem parent, ScoreSheet scoreSheet, bool isExpanded)
        {
            try
            {
                if (scoreSheet == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.ScoreSheet;
                item.Display = scoreSheet.Title;
                item.ToolTip = scoreSheet.Title;
                item.IsExpanded = isExpanded;
                item.Icon = "Images/template.ico";
                item.Data = scoreSheet;

                CreateChildItem(item, scoreSheet);
                CreateChildComment(item, scoreSheet);
                CreateChildControlItem(item, scoreSheet);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateScoreGroupItem(ObjectItem parent, ScoreGroup scoreGroup, bool isExpanded)
        {
            try
            {
                if (scoreGroup == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.ScoreGroup;
                item.Display = scoreGroup.Title;
                item.ToolTip = scoreGroup.Title;
                item.IsExpanded = isExpanded;
                item.Icon = "Images/TemplateItem.ico";
                item.Data = scoreGroup;

                CreateChildItem(item, scoreGroup);
                CreateChildComment(item, scoreGroup);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateNumericStandardItem(ObjectItem parent, NumericStandard numericStandard, bool isExpanded)
        {
            try
            {
                if (numericStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.NumericStandard;
                item.Display = numericStandard.Title;
                item.ToolTip = numericStandard.Title;
                item.Icon = "Images/standard.ico";
                item.Data = numericStandard;
                item.IsExpanded = isExpanded;

                CreateChildComment(item, numericStandard);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateYesNoStandardItem(ObjectItem parent, YesNoStandard yesNoStandard, bool isExpanded)
        {
            try
            {
                if (yesNoStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.YesNoStandard;
                item.Display = yesNoStandard.Title;
                item.ToolTip = yesNoStandard.Title;
                item.Icon = "Images/standard.ico";
                item.Data = yesNoStandard;
                item.IsExpanded = isExpanded;

                CreateChildComment(item, yesNoStandard);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateItemStandardItem(ObjectItem parent, ItemStandard itemStandard, bool isExpanded)
        {
            try
            {
                if (itemStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.ItemStandard;
                item.Display = itemStandard.Title;
                item.ToolTip = itemStandard.Title;
                item.Icon = "Images/standard.ico";
                item.Data = itemStandard;
                item.IsExpanded = isExpanded;

                CreateChildStandardItem(item, itemStandard);
                CreateChildComment(item, itemStandard);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateSliderStandardItem(ObjectItem parent, SliderStandard sliderStandard, bool isExpanded)
        {
            try
            {
                if (sliderStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.SliderStandard;
                item.Display = sliderStandard.Title;
                item.ToolTip = sliderStandard.Title;
                item.Icon = "Images/standard.ico";
                item.Data = sliderStandard;
                item.IsExpanded = isExpanded;

                CreateChildComment(item, sliderStandard);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateStandardItemItem(ObjectItem parent, StandardItem standardItem)
        {
            try
            {
                if (standardItem == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.StandardItem;
                item.Display = standardItem.Display;
                item.ToolTip = standardItem.Display;
                item.Icon = "Images/standarditem.ico";
                item.Data = standardItem;

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateTextCommentItem(ObjectItem parent, TextComment textComment)
        {
            try
            {
                if (textComment == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.TextComment;
                item.Display = textComment.Title;
                item.ToolTip = textComment.Description;
                item.Icon = "Images/comment.ico";
                item.Data = textComment;

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateItemCommentItem(ObjectItem parent, ItemComment itemComment, bool isExpanded)
        {
            try
            {
                if (itemComment == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.ItemComment;
                item.Display = itemComment.Title;
                item.ToolTip = itemComment.Description;
                item.Icon = "Images/comment.ico";
                item.Data = itemComment;
                item.IsExpanded = isExpanded;

                CreateChildCommentItem(item, itemComment);

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateCommentItemItem(ObjectItem parent, CommentItem commentItem)
        {
            try
            {
                if (commentItem == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.CommentItem;
                item.Display = commentItem.Text;
                item.ToolTip = commentItem.Text;
                item.Icon = "Images/commentitem.ico";
                item.Data = commentItem;

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateControlItemItem(ObjectItem parent, ControlItem controlItem)
        {
            try
            {
                if (controlItem == null) { return; }
                ObjectItem item = new ObjectItem();
                item.ObjType = (int)ScoreObjectType.ControlItem;
                item.Display = controlItem.Title;
                item.ToolTip = controlItem.Title;
                item.Icon = "Images/controlitem.png";
                item.Data = controlItem;

                parent.AddChild(item);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreateChildItem(ObjectItem parent, ScoreGroup scoreGroup)
        {
            if (parent == null || scoreGroup == null) { return; }
            if (scoreGroup.Items.Count > 0)
            {
                for (int i = 0; i < scoreGroup.Items.Count; i++)
                {
                    ScoreItem scoreItem = scoreGroup.Items[i];
                    switch (scoreItem.Type)
                    {
                        case ScoreObjectType.ScoreGroup:
                            CreateScoreGroupItem(parent, scoreItem as ScoreGroup, false);
                            break;
                        case ScoreObjectType.NumericStandard:
                            CreateNumericStandardItem(parent, scoreItem as NumericStandard, false);
                            break;
                        case ScoreObjectType.YesNoStandard:
                            CreateYesNoStandardItem(parent, scoreItem as YesNoStandard, false);
                            break;
                        case ScoreObjectType.ItemStandard:
                            CreateItemStandardItem(parent, scoreItem as ItemStandard, false);
                            break;
                        case ScoreObjectType.SliderStandard:
                            CreateSliderStandardItem(parent, scoreItem as SliderStandard, false);
                            break;
                    }
                }
            }
        }

        private void CreateChildComment(ObjectItem parent, ScoreItem scoreItem)
        {
            if (parent == null || scoreItem == null) { return; }
            if (scoreItem.Comments.Count > 0)
            {
                for (int i = 0; i < scoreItem.Comments.Count; i++)
                {
                    Comment comment = scoreItem.Comments[i];
                    switch (comment.Style)
                    {
                        case CommentStyle.Text:
                            CreateTextCommentItem(parent, comment as TextComment);
                            break;
                        case CommentStyle.Item:
                            CreateItemCommentItem(parent, comment as ItemComment, false);
                            break;
                    }
                }
            }
        }

        private void CreateChildControlItem(ObjectItem parent, ScoreSheet scoreSheet)
        {
            if (parent == null || scoreSheet == null) { return; }
            if (scoreSheet.ControlItems.Count > 0)
            {
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    ControlItem controlItem = scoreSheet.ControlItems[i];
                    CreateControlItemItem(parent, controlItem);
                }
            }
        }

        private void CreateChildStandardItem(ObjectItem parent, ItemStandard itemStandard)
        {
            if (parent == null || itemStandard == null) { return; }
            if (itemStandard.ValueItems.Count > 0)
            {
                for (int i = 0; i < itemStandard.ValueItems.Count; i++)
                {
                    StandardItem standardItem = itemStandard.ValueItems[i];
                    CreateStandardItemItem(parent, standardItem);
                }
            }
        }

        private void CreateChildCommentItem(ObjectItem parent, ItemComment itemComment)
        {
            if (parent == null || itemComment == null) { return; }
            if (itemComment.ValueItems.Count > 0)
            {
                for (int i = 0; i < itemComment.ValueItems.Count; i++)
                {
                    CommentItem commentItem = itemComment.ValueItems[i];
                    CreateCommentItemItem(parent, commentItem);
                }
            }
        }

        #endregion

        #region Load and Init

        private void LoadScoreIcons()
        {
            ListScoreIcons.Clear();
            for (int i = 0; i < mListScoreSettings.Count; i++)
            {
                ScoreSetting setting = mListScoreSettings[i];
                if (setting.Category == "I")
                {
                    ListScoreIcons.Add(setting);
                }
            }
        }

        private void InitScoreSettings()
        {
            mListScoreSettings.Clear();

            #region 基本设定信息
            //默认显示风格
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "V_CLASSIC",
                Category = "S",
                Value = "Tree"
            });
            //标题图标是否显示
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "T_ICON_VIS",
                Category = "S",
                Value = "F"
            });
            //打分值的宽度
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "V_WIDTH",
                Category = "S",
                Value = "150"
            });
            #endregion

            #region 图标信息
            //ScoreGroup默认图标
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_SCOREG",
                Category = "I",
                Value = "Images/scoregroup.png"
            });
            //Standard默认图标
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_STANDARD",
                Category = "I",
                Value = "Images/standard.png"
            });
            //描述开关
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_DESCRIPTION",
                Category = "I",
                Value = "Images/info.png"
            });
            //备注开关
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_COMMENT_ITEM",
                Category = "I",
                Value = "Images/showcomment.png"
            });
            //关键项
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_KEY_ITEM",
                Category = "I",
                Value = "Images/keyitem.png"
            });
            //附加项
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_ADD_ITEM",
                Category = "I",
                Value = "Images/additionalitem.png"
            });
            //控制源
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_CTL_SRC",
                Category = "I",
                Value = "Images/controlitem.png"
            });
            //控制目标
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "I_CTL_TGT",
                Category = "I",
                Value = "Images/controltarget.png"
            });
            #endregion
        }

        private void InitLanuageTypes()
        {
            mListLanguageTypes.Clear();
            mListLanguageTypes.Add(new LanguageTypeItem
            {
                LangID = 1033,
                Display = GetLanguage("Basic", "L_1033", "English")
            });
            mListLanguageTypes.Add(new LanguageTypeItem
            {
                LangID = 2052,
                Display = GetLanguage("Basic", "L_2052", "简体中文")
            });
        }

        private void LoadLanguageInfos()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.xml");
            if (!File.Exists(path))
            {
                ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_007", "Language file not exist."), path));
                return;
            }
            try
            {
                ScoreLanguageManager manager;
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreLanguageManager>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N_008", "Load Langugae info fail."), optReturn.Code,
                        optReturn.Message));
                    return;
                }
                manager = optReturn.Data as ScoreLanguageManager;
                if (manager == null)
                {
                    ShowErrorMessage(string.Format("{0}\tScoreLanguageManager is null", GetLanguage("Designer", "N_008", "Load Langugae info fail.")));
                    return;
                }
                mListLanguageInfos.Clear();
                for (int i = 0; i < manager.Languages.Count; i++)
                {
                    mListLanguageInfos.Add(manager.Languages[i]);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

        }

        #endregion

        #region Other

        private void CreateNewButtons()
        {
            try
            {
                NewButtonItem btnItem;
                NewButtonItem childBtn;
                MenuItem menuItem;
                MenuItem childItem;
                Image icon;
                BtnNew.Items.Clear();
                if (TvObjects.Items == null || TvObjects.Items.Count <= 0)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "ScoreSheet",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "ScoreSheet"), "ScoreSheet"),
                        Icon = "Images/template.ico",
                        ToolTip = "ScoreSheet"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);
                    return;
                }
                var selectObj = TvObjects.SelectedItem as ObjectItem;
                if (selectObj == null) { return; }
                var scoreObj = selectObj.Data as ScoreObject;
                if (scoreObj == null) { return; }
                if (scoreObj.Type == ScoreObjectType.ScoreSheet
                    || scoreObj.Type == ScoreObjectType.ScoreGroup)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "ScoreGroup",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "ScoreGroup"), "ScoreGroup"),
                        Icon = "Images/templateitem.ico",
                        ToolTip = "ScoreGroup"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);

                    btnItem = new NewButtonItem
                    {
                        Name = "Standard",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "Standard"), "Standard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "Standard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    //menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.IsSplited = true;

                    childBtn = new NewButtonItem
                    {
                        Name = "NumericStandard",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "NumericStandard"), "NumericStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "NumericStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new NewButtonItem
                    {
                        Name = "YesNoStandard",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "YesNoStandard"), "YesNoStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "YesNoStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new NewButtonItem
                    {
                        Name = "SliderStandard",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "SliderStandard"), "SliderStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "SliderStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new NewButtonItem
                    {
                        Name = "ItemStandard",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "ItemStandard"), "ItemStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "ItemStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    BtnNew.Items.Add(menuItem);
                }
                if (scoreObj.Type == ScoreObjectType.ScoreSheet
                   || scoreObj.Type == ScoreObjectType.ScoreGroup
                   || scoreObj.Type == ScoreObjectType.NumericStandard
                    || scoreObj.Type == ScoreObjectType.YesNoStandard
                    || scoreObj.Type == ScoreObjectType.SliderStandard
                    || scoreObj.Type == ScoreObjectType.ItemStandard)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "Comment",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "Comment"), "Comment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "Comment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    //menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.IsSplited = true;

                    childBtn = new NewButtonItem
                    {
                        Name = "TextComment",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "TextComment"), "TextComment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "TextComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    childBtn = new NewButtonItem
                    {
                        Name = "ItemComment",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "ItemComment"), "ItemComment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "ItemComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Click += NewObjectButton_Click;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    BtnNew.Items.Add(menuItem);
                }
                if (scoreObj.Type == ScoreObjectType.ItemStandard)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "StandardItem",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "StandardItem"), "StandardItem"),
                        Icon = "Images/standarditem.ico",
                        ToolTip = "StandardItem"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);
                }
                if (scoreObj.Type == ScoreObjectType.ItemComment)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "CommentItem",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "CommentItem"), "CommentItem"),
                        Icon = "Images/commentitem.ico",
                        ToolTip = "CommentItem"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);
                }
                if (scoreObj.Type == ScoreObjectType.ScoreSheet)
                {
                    btnItem = new NewButtonItem
                    {
                        Name = "ControlItem",
                        Header = GetLanguage("ObjectViewer", string.Format("O_ScoreObjectType_{0}", "ControlItem"), "ControlItem"),
                        Icon = "Images/controlitem.png",
                        ToolTip = "ControlItem"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Click += NewObjectButton_Click;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveScoreObject(ScoreDocument scoreDocument)
        {
            if (scoreDocument == null) { return; }
            try
            {
                OperationReturn optReturn = XMLHelper.SerializeObject(scoreDocument, scoreDocument.FullPath);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N_009", "Serialize fail."), optReturn.Code, optReturn.Message));
                    return;
                }
                ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_010", "Save end."), scoreDocument.FullPath));
                mIsChanged = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadScoreSheet(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_011", "File not exist."), path));
                    return;
                }
                ScoreDocument scoreDocument = null;
                ScoreSheet scoreSheet;
                OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(path);
                if (!optReturn.Result)
                {
                    optReturn = XMLHelper.DeserializeObject<ScoreDocument>(path);
                    if (!optReturn.Result)
                    {
                        ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N_012", "Load ScoreSheet fail."), optReturn.Code, optReturn.Message));
                        return;
                    }
                    scoreDocument = optReturn.Data as ScoreDocument;
                    if (scoreDocument == null)
                    {
                        ShowErrorMessage(string.Format("ScoreDocument is null"));
                        return;
                    }
                    scoreSheet = scoreDocument.ScoreObject as ScoreSheet;
                }
                else
                {
                    scoreSheet = optReturn.Data as ScoreSheet;
                }
                if (scoreSheet == null)
                {
                    ShowErrorMessage(string.Format("ScoreSheet is null"));
                    return;
                }
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.Init();
                if (scoreDocument == null)
                {
                    scoreDocument = new ScoreDocument();
                }
                string fullName = path;
                string folderPath = fullName.Substring(0, fullName.LastIndexOf('\\'));
                string fileName = fullName.Substring(fullName.LastIndexOf('\\') + 1);
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
                scoreDocument.Name = fileName;
                scoreDocument.FolderPath = folderPath;
                scoreDocument.FullPath = fullName;
                scoreDocument.ScoreObject = scoreSheet;
                mScoreDocument = scoreDocument;
                CreateScoreSheetItem(mRootObject, scoreSheet, true);
                CheckScoreSheetValid();
                CreateNewButtons();
                if (CbObjects.IsChecked != true)
                {
                    CbObjects.IsChecked = true;
                    SetPanelView();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void PrepareProperties(ScoreObject scoreObject)
        {
            List<ScoreProperty> listScoreProperties = new List<ScoreProperty>();
            scoreObject.GetPropertyList(ref listScoreProperties);
            listScoreProperties = listScoreProperties.OrderBy(s => s.OrderID).ToList();
            ObjectProperty.PropertyDefinitions.Clear();
            ObjectProperty.EditorDefinitions.Clear();
            for (int i = 0; i < listScoreProperties.Count; i++)
            {
                ScoreProperty scoreProperty = listScoreProperties[i];
                if ((scoreProperty.Flag & ScorePropertyFlag.Visible) != 0)
                {
                    PropertyDefinition pd = new PropertyDefinition();
                    pd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                    pd.DisplayName = GetLanguage("PropertyViewer", string.Format("P_Display_{0}", scoreProperty.Name), scoreProperty.Display);
                    pd.Category = GetLanguage("PropertyViewer", string.Format("P_Category_{0}", scoreProperty.Category), scoreProperty.Category);
                    pd.Description = GetLanguage("PropertyViewer", string.Format("P_Description_{0}", scoreProperty.Name), scoreProperty.Category);
                    pd.DisplayOrder = scoreProperty.OrderID;
                    ObjectProperty.PropertyDefinitions.Add(pd);
                    EditorTemplateDefinition etd;
                    DataTemplate dt;
                    FrameworkElementFactory fef;
                    Binding binding;
                    switch (scoreProperty.DataType)
                    {
                        case ScorePropertyDataType.MString:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(MultiLineTextEditor));
                            MultLineTextConverter converter = new MultLineTextConverter();
                            binding = new Binding("Value");
                            binding.Converter = converter;
                            fef.SetBinding(ContentProperty, binding);
                            fef.SetBinding(MultiLineTextEditor.TextProperty, new Binding("Value"));
                            fef.SetValue(BackgroundProperty, Brushes.Transparent);
                            fef.SetResourceReference(StyleProperty, "MultiLineEditorStyle");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            ObjectProperty.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.Icon:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorComboBox));
                            IconNameToScoreSettingConverter iconConverter = new IconNameToScoreSettingConverter();
                            binding = new Binding("Value");
                            binding.Converter = iconConverter;
                            binding.ConverterParameter = ListScoreIcons;
                            fef.SetValue(PropertyGridEditorComboBox.ItemsSourceProperty, ListScoreIcons);
                            fef.SetBinding(PropertyGridEditorComboBox.SelectedItemProperty, binding);
                            fef.SetBinding(ContentProperty, binding);
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemTemplateProperty, "IconEditorItem");
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemContainerStyleProperty, "IconEditorItemStyle");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            ObjectProperty.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.Enum:
                            string[] enums = Enum.GetNames(scoreProperty.ValueType);
                            List<EnumItem> listEnum = new List<EnumItem>();
                            for (int k = 0; k < enums.Length; k++)
                            {
                                EnumItem item = new EnumItem();
                                item.Name = enums[k];
                                item.Type = scoreProperty.ValueType;
                                item.Display = GetLanguage("PropertyViewer", string.Format("P_Enum_{0}_{1}", scoreProperty.ValueType.Name, enums[k]), enums[k]);
                                listEnum.Add(item);
                            }
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorComboBox));
                            EnumConverter enumConverter = new EnumConverter();
                            binding = new Binding("Value");
                            binding.Converter = enumConverter;
                            binding.ConverterParameter = listEnum;
                            fef.SetValue(PropertyGridEditorComboBox.ItemsSourceProperty, listEnum);
                            fef.SetBinding(PropertyGridEditorComboBox.SelectedItemProperty, binding);
                            fef.SetBinding(ContentProperty, binding);
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemTemplateProperty, "EnumEditorItem");
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemContainerStyleProperty, "EnumEditorItemStyle");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            ObjectProperty.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.CustomObject:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorComboBox));
                            if (scoreProperty.Name == "Source")
                            {
                                fef.SetValue(PropertyGridEditorComboBox.ItemsSourceProperty, ListControlSourceItems);
                            }
                            else if (scoreProperty.Name == "Target")
                            {
                                fef.SetValue(PropertyGridEditorComboBox.ItemsSourceProperty, ListControlTargetItems);
                            }
                            else { continue; }
                            fef.SetBinding(PropertyGridEditorComboBox.SelectedItemProperty, new Binding("Value"));
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemTemplateProperty, "CustomObjectEditorItem");
                            fef.SetResourceReference(PropertyGridEditorComboBox.ItemContainerStyleProperty, "CustomObjectEditorItemStyle");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            ObjectProperty.EditorDefinitions.Add(etd);
                            break;
                    }

                }
            }
        }

        private void CheckScoreSheetValid()
        {
            try
            {
                if (mRootObject != null && mRootObject.Children.Count > 0)
                {
                    for (int i = 0; i < mRootObject.Children.Count; i++)
                    {
                        ObjectItem item = mRootObject.Children[i];
                        if (item.ObjType == (int)ScoreObjectType.ScoreSheet)
                        {
                            ScoreSheet scoreSheet = item.Data as ScoreSheet;
                            if (scoreSheet != null)
                            {
                                CheckValidResult result = scoreSheet.CheckValid();
                                item.InvalidCode = result.Code;
                                if (result.Code != 0)
                                {
                                    item.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", result.Code), result.Message), result.ScoreObject);
                                }
                                else
                                {
                                    scoreSheet.Flag = scoreSheet.Flag | 1;
                                    item.InvalidMessage = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CheckObjectItemValid(ObjectItem item)
        {
            //检查评分项
            ScoreItem scoreItem = item.Data as ScoreItem;
            if (scoreItem != null)
            {
                //检查本项
                CheckValidResult result = scoreItem.CheckValid();
                item.InvalidCode = result.Code;
                if (result.Code != 0)
                {
                    item.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", result.Code), result.Message), result.ScoreObject);
                }
                else
                {
                    item.InvalidMessage = string.Empty;
                }
                ObjectItem parent = item.Parent;
                if (parent != null)
                {
                    //检查父项
                    scoreItem = parent.Data as ScoreItem;
                    if (scoreItem != null)
                    {
                        result = scoreItem.CheckValid();
                        parent.InvalidCode = result.Code;
                        if (result.Code != 0)
                        {
                            parent.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("N_Invalid_{0}", result.Code), result.Message), result.ScoreObject);
                        }
                        else
                        {
                            parent.InvalidMessage = string.Empty;
                        }
                    }
                }
                //检查评分表
                CheckScoreSheetValid();
            }
        }

        private void ChangeStandardType(Standard standard, StandardType newType)
        {
            List<ScoreProperty> listProperties = new List<ScoreProperty>();
            Standard baseStandard = new Standard();
            baseStandard.GetPropertyList(ref listProperties);
            Standard newStandard = null;
            switch (newType)
            {
                case StandardType.Numeric:
                    newStandard = new NumericStandard();
                    newStandard.Type = ScoreObjectType.NumericStandard;
                    break;
                case StandardType.YesNo:
                    newStandard = new YesNoStandard();
                    newStandard.Type = ScoreObjectType.YesNoStandard;
                    break;
                case StandardType.Slider:
                    newStandard = new SliderStandard();
                    newStandard.Type = ScoreObjectType.SliderStandard;
                    break;
                case StandardType.Item:
                    newStandard = new ItemStandard();
                    newStandard.Type = ScoreObjectType.ItemStandard;
                    break;
            }
            if (newStandard != null)
            {
                newStandard.ID = standard.ID;
                newStandard.StandardType = newType;
                newStandard.Parent = standard.Parent;
                newStandard.ScoreSheet = standard.ScoreSheet;
                //复制属性
                for (int i = 0; i < listProperties.Count; i++)
                {
                    newStandard.CopyProperty(standard, newStandard, listProperties[i]);
                }
                ScoreGroup parent = newStandard.Parent as ScoreGroup;
                if (parent != null)
                {
                    int index = parent.Items.IndexOf(standard);
                    parent.Items.Insert(index, newStandard);
                    parent.Items.Remove(standard);
                }
                ObjectItem item = TvObjects.SelectedItem as ObjectItem;
                if (item != null)
                {
                    item.ObjType = (int)newStandard.Type;
                    item.Data = newStandard;
                    item.IsSelected = true;
                    //如果有子项，清除
                    item.Children.Clear();
                    TvObjects_OnSelectedItemChanged(this, null);
                }
            }
        }

        private void MoveScoreObject(ObjectItem source, ObjectItem target)
        {
            try
            {
                if (source == null || target == null) { return; }
                ObjectItem sourceParent = source.Parent;
                ObjectItem targetParent = target.Parent;
                if (sourceParent == null || targetParent == null) { return; }
                if (sourceParent == targetParent)
                {
                    //源和目标在同一父级节点下，此时处理对象次序（OrderID）

                    //移动评分项的次序
                    if ((source.ObjType == (int)ScoreObjectType.ScoreGroup ||
                       source.ObjType == (int)ScoreObjectType.NumericStandard ||
                       source.ObjType == (int)ScoreObjectType.YesNoStandard ||
                       source.ObjType == (int)ScoreObjectType.SliderStandard ||
                       source.ObjType == (int)ScoreObjectType.ItemStandard)
                       &&
                       (target.ObjType == (int)ScoreObjectType.ScoreSheet ||
                        target.ObjType == (int)ScoreObjectType.ScoreGroup) ||
                        target.ObjType == (int)ScoreObjectType.NumericStandard ||
                        target.ObjType == (int)ScoreObjectType.YesNoStandard ||
                        target.ObjType == (int)ScoreObjectType.SliderStandard ||
                        target.ObjType == (int)ScoreObjectType.ItemStandard)
                    {
                        ScoreItem sourceScoreItem = source.Data as ScoreItem;
                        ScoreItem targetScoreItem = target.Data as ScoreItem;
                        if (sourceScoreItem != null && targetScoreItem != null)
                        {
                            RemoveScoreItem(source);
                            InsertScoreItem(source, target);
                        }
                    }
                }
                else
                {
                    //源和目标不在同一父级节点下，此时仅仅是将源对象移动到目标对象的下级

                    //将评分标准或组移动到另一评分组或评分表里
                    if ((source.ObjType == (int)ScoreObjectType.ScoreGroup ||
                        source.ObjType == (int)ScoreObjectType.NumericStandard ||
                        source.ObjType == (int)ScoreObjectType.YesNoStandard ||
                        source.ObjType == (int)ScoreObjectType.SliderStandard ||
                        source.ObjType == (int)ScoreObjectType.ItemStandard)
                        &&
                        (target.ObjType == (int)ScoreObjectType.ScoreSheet
                        || target.ObjType == (int)ScoreObjectType.ScoreGroup))
                    {
                        ScoreItem sourceScoreItem = source.Data as ScoreItem;
                        ScoreGroup targetScoreGroup = target.Data as ScoreGroup;
                        if (sourceScoreItem != null && targetScoreGroup != null)
                        {
                            RemoveScoreItem(source);
                            AddScoreItem(source, target);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void RemoveScoreItem(ObjectItem item)
        {
            ScoreItem scoreItem = item.Data as ScoreItem;
            ScoreGroup scoreGroup = item.Parent.Data as ScoreGroup;
            if (scoreItem != null && scoreGroup != null)
            {
                scoreGroup.Items.Remove(scoreItem);
                item.Parent.RemoveChild(item);
            }
        }

        private void AddScoreItem(ObjectItem item, ObjectItem parent)
        {
            ScoreItem scoreItem = item.Data as ScoreItem;
            ScoreGroup scoreGroup = parent.Data as ScoreGroup;
            if (scoreItem != null && scoreGroup != null)
            {
                scoreItem.Parent = scoreGroup;
                scoreGroup.Items.Add(scoreItem);
                parent.AddChild(item);
            }
        }

        private void InsertScoreItem(ObjectItem source, ObjectItem target)
        {
            ObjectItem parent = source.Parent;
            ScoreItem scoreItem = source.Data as ScoreItem;
            ScoreGroup scoreGroup = target.Parent.Data as ScoreGroup;
            if (scoreItem != null && scoreGroup != null)
            {
                int index = parent.Children.IndexOf(target);
                switch (mDragType)
                {
                    case 0:
                        scoreItem.Parent = scoreGroup;
                        scoreGroup.Items.Insert(index + 1, scoreItem);
                        parent.Children.Insert(index + 1, source);
                        break;
                    case 1:
                        scoreItem.Parent = scoreGroup;
                        scoreGroup.Items.Insert(index, scoreItem);
                        parent.Children.Insert(index, source);
                        break;
                    case 2:
                        scoreGroup = target.Data as ScoreGroup;
                        if (scoreGroup != null)
                        {
                            scoreItem.Parent = scoreGroup;
                            scoreGroup.Items.Add(scoreItem);
                            target.Children.Add(source);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Basic

        private long GetID()
        {
            mObjectID++;
            return Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss") + mObjectID.ToString("0000"));
        }

        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg, "UMP Template Designer", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfoMessage(string msg)
        {
            MessageBox.Show(msg, "UMP Template Designer", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetPanelView()
        {
            if (CbObjects.IsChecked == true)
            {
                GridObjects.Width = new GridLength(200);
            }
            else
            {
                GridObjects.Width = new GridLength(0);
            }
            if (CbProperty.IsChecked == true)
            {
                GridProperty.Width = new GridLength(250);
            }
            else
            {
                GridProperty.Width = new GridLength(0);
            }
            if (CbViewHead.IsChecked == true)
            {
                GridViewHead.Height = new GridLength(30);
            }
            else
            {
                GridViewHead.Height = new GridLength(0);
            }
        }

        private void SetDragDropState()
        {
            RadioDown.IsChecked = mDragType == 0;
            RadioUp.IsChecked = mDragType == 1;
            RadioChild.IsChecked = mDragType == 2;
        }

        private void ShowLanguages()
        {
            BtnAbout.Header = GetLanguage("ToolBar", "B_About", "About");
            BtnClose.Header = GetLanguage("ToolBar", "B_Close", "Close");
            BtnNew.Header = GetLanguage("ToolBar", "B_New", "New");
            BtnOpen.Header = GetLanguage("ToolBar", "B_Open", "Open");
            BtnSave.Header = GetLanguage("ToolBar", "B_Save", "Save");
            BtnSaveTo.Header = GetLanguage("ToolBar", "B_SaveTo", "Save");
            BtnDelete.Header = GetLanguage("ToolBar", "B_Delete", "Delete");
            BtnPrint.Header = GetLanguage("ToolBar", "B_Print", "Print");
            BtnCaculate.Header = GetLanguage("ToolBar", "B_Caculate", "Caculate");

            BtnAbout.ToolTip = GetLanguage("ToolBar", "I_About", "About");
            BtnClose.ToolTip = GetLanguage("ToolBar", "I_Close", "Close");
            BtnNew.ToolTip = GetLanguage("ToolBar", "I_New", "New");
            BtnOpen.ToolTip = GetLanguage("ToolBar", "I_Open", "Open");
            BtnSave.ToolTip = GetLanguage("ToolBar", "I_Save", "Save");
            BtnSaveTo.ToolTip = GetLanguage("ToolBar", "I_SaveTo", "Save");
            BtnDelete.ToolTip = GetLanguage("ToolBar", "I_Delete", "Delete");
            BtnPrint.ToolTip = GetLanguage("ToolBar", "I_Print", "Print");
            BtnCaculate.ToolTip = GetLanguage("ToolBar", "I_Caculate", "Caculate");

            RadioDown.Header = GetLanguage("ToolBar", "R_MoveDown", "As next object");
            RadioUp.Header = GetLanguage("ToolBar", "R_MoveUp", "As pre object");
            RadioChild.Header = GetLanguage("ToolBar", "R_MoveChild", "As child object");

            CbObjects.Header = GetLanguage("ToolBar", "C_Objects", "Objects");
            CbProperty.Header = GetLanguage("ToolBar", "C_Property", "Property");
            CbViewHead.Header = GetLanguage("ToolBar", "C_ViewHead", "Viewer Head");

            CbObjects.ToolTip = GetLanguage("ToolBar", "I_Objects", "Objects");
            CbProperty.ToolTip = GetLanguage("ToolBar", "I_Property", "Property");
            CbViewHead.ToolTip = GetLanguage("ToolBar", "I_ViewHead", "Viewer Head");

            TabHome.Header = GetLanguage("ToolBar", "T_Home", "Home");

            GroupBasic.Header = GetLanguage("ToolBar", "G_Basic", "Basic");
            GroupView.Header = GetLanguage("ToolBar", "G_View", "View");
            GroupTool.Header = GetLanguage("ToolBar", "G_Tools", "Tools");
            GroupLanguage.Header = GetLanguage("ToolBar", "G_Languages", "Languages");
            GroupDragDrop.Header = GetLanguage("ToolBar", "G_DragDrop", "Drag and Drop");

        }

        private string GetLanguage(string category, string code, string display)
        {
            ScoreLangauge language =
                mListLanguageInfos.FirstOrDefault(
                    l => l.LangID == mLanguageID && l.Category == category && l.Code == code);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        #endregion

        #region Drag and Drop

        private void PanelObject_OnDragOver(object sender, DragEventArgs e)
        {

        }

        private void PanelObject_OnDragEnter(object sender, DragEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                panel.Background = Brushes.LightBlue;
            }
        }

        private void PanelObject_OnDragLeave(object sender, DragEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                panel.Background = Brushes.Transparent;
            }
        }

        private void PanelObject_OnDrop(object sender, DragEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                panel.Background = Brushes.Transparent;
                var targetItem = panel.Tag as ObjectItem;
                var sourceItem = e.Data.GetData(typeof(ObjectItem)) as ObjectItem;
                if (sourceItem == null || targetItem == null || sourceItem == targetItem)
                {
                    return;
                }
                MoveScoreObject(sourceItem, targetItem);
            }
        }

        private void PanelObject_OnMouseMove(object sender, MouseEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                var item = panel.Tag as ObjectItem;
                if (item != null)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragDrop.DoDragDrop(panel, item, DragDropEffects.Move);
                    }
                }
            }
        }

        #endregion

    }
}
