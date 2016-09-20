using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using UMPScoreSheetDesigner.Commands;
using UMPScoreSheetDesigner.Converters;
using UMPScoreSheetDesigner.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.PropertyGrids;
using VoiceCyber.Wpf.PropertyGrids.Definitions;
using VoiceCyber.Wpf.PropertyGrids.Editors;
using MenuItem = VoiceCyber.Ribbon.MenuItem;

namespace UMPScoreSheetDesigner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region Members

        private bool mIsChanged;
        private ObjectItem mRootObject;
        private long mObjectID;
        private ObservableCollection<ScoreItem> mListControlSourceItems;
        private ObservableCollection<ScoreItem> mListControlTargetItems;
        private ObservableCollection<StandardItem> mListItemStandardItems;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private ObservableCollection<LanguageTypeItem> mListLanguageTypes;
        private ScoreDocument mScoreDocument;
        private ScoreSheet mCurrentScoreSheet;
        private int mLanguageID;
        private double mViewerScale;
        private int mDragType;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            mListControlSourceItems = new ObservableCollection<ScoreItem>();
            mListControlTargetItems = new ObservableCollection<ScoreItem>();
            mListItemStandardItems = new ObservableCollection<StandardItem>();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListLanguageTypes = new ObservableCollection<LanguageTypeItem>();
            mRootObject = new ObjectItem();
            mObjectID = 0;
            mLanguageID = 2052;
            mViewerScale = 1;
            mDragType = 0;
            mIsChanged = false;

            Loaded += MainWindow_OnLoaded;
            Closing += MainWindow_OnClosing;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            BindCommands();
            BindEventHandlers();
            LoadLanguageInfos();
            InitScoreSettings();
            InitLanuageTypes();
            TvObjects.ItemsSource = mRootObject.Children;
            GalleryLanguages.ItemsSource = mListLanguageTypes;
            SliderScale.Tag = mViewerScale;
            CreateNewButtons();
            SetDragDropState();
            ShowLanguages();
            LanguageTypeItem languageType = mListLanguageTypes.FirstOrDefault(l => l.LangID == mLanguageID);
            if (languageType != null)
            {
                GalleryLanguages.SelectedItem = languageType;
            }
            LoadLayout();
            SetViewStatus();
            WindowState = WindowState.Maximized;
        }

        private void MainWindow_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mIsChanged)
            {
                var result = MessageBox.Show(string.Format("{0}", GetLanguage("Designer", "N013", "ScoreSheet has changed. Do you want to save?")),
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


        #region Init And Load

        private void BindCommands()
        {
            CommandBinding binding = new CommandBinding(ApplicationCommands.Close);
            binding.Executed += (s, e) => Close();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.ShowAboutCommand);
            binding.Executed += (s, e) => { };
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.New);
            binding.Executed += (s, e) => { };
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.NewObjectCommand);
            binding.Executed += NewObjectButton_Click;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Open);
            binding.Executed += (s, e) => OpenScoreDocument();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Save);
            binding.Executed += BtnSave_Click;
            binding.CanExecute += (s, e) => e.CanExecute = mIsChanged;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.SaveAs);
            binding.Executed += BtnSaveTo_Click;
            binding.CanExecute += (s, e) =>
            {
                if (mRootObject.Children.Count > 0)
                {
                    e.CanExecute = true;
                }
            };
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Delete);
            binding.Executed += BtnDelete_Click;
            binding.CanExecute += (s, e) =>
            {
                if (TvObjects.SelectedItem != null)
                {
                    e.CanExecute = true;
                }
            };
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.SetViewCommand);
            binding.Executed += (s, e) => SetPanelVisibility();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Print);
            binding.Executed += BtnPrint_Click;
            binding.CanExecute += (s, e) =>
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item != null)
                {
                    var scoreSheet = item.Data as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        e.CanExecute = true;
                    }
                }
            };
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.CaculateCommand);
            binding.Executed += BtnCaculate_Click;
            binding.CanExecute += (s, e) =>
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item != null)
                {
                    var scoreItem = item.Data as ScoreGroup;
                    if (scoreItem != null)
                    {
                        e.CanExecute = true;
                    }
                }
            };
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.SetDragDropCommand);
            binding.Executed += (s, e) =>
            {
                var type = e.Parameter.ToString();
                if (type == "0")
                {
                    mDragType = 0;
                }
                else if (type == "1")
                {
                    mDragType = 1;
                }
                else if (type == "2")
                {
                    mDragType = 2;
                }
            };
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.SaveLayoutCommand);
            binding.Executed += SaveLayout;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.ResetLayoutCommand);
            binding.Executed += ResetLayout;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(MainWindowCommands.ChildListCommand);
            binding.Executed += ExecuteChildListCommand;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);
        }

        private void BindEventHandlers()
        {
            TvObjects.SelectedItemChanged += TvObjects_OnSelectedItemChanged;
            GalleryLanguages.SelectionChanged += GalleryLanguages_SelectionChanged;
            SliderScale.ValueChanged += SliderScale_ValueChanged;
            ScoreItemPropertyGrid.SelectedPropertyItemChanged += ScoreItemPropertyGrid_SelectedPropertyItemChanged;
            ScoreItemPropertyGrid.PropertyValueChanged += ScoreItemPropertyGrid_PropertyValueChanged;
            PropertyChildList.SelectedItemChanged += PropertyChildList_SelectedItemChanged;
            ChildPropertyGrid.PropertyValueChanged += ChildPropertyGrid_PropertyValueChanged;
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
            //Tip的宽度
            mListScoreSettings.Add(new ScoreSetting
            {
                Code = "T_WIDTH",
                Category = "S",
                Value = "50"
            });
            #endregion
        }

        private void LoadLanguageInfos()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.xml");
            if (!File.Exists(path))
            {
                ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N007", "Language file not exist."), path));
                return;
            }
            try
            {
                ScoreLanguageManager manager;
                OperationReturn optReturn = XMLHelper.DeserializeFile<ScoreLanguageManager>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N008", "Load Langugae info fail."), optReturn.Code,
                        optReturn.Message));
                    return;
                }
                manager = optReturn.Data as ScoreLanguageManager;
                if (manager == null)
                {
                    ShowErrorMessage(string.Format("{0}\tScoreLanguageManager is null", GetLanguage("Designer", "N008", "Load Langugae info fail.")));
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

        private void InitLanuageTypes()
        {
            mListLanguageTypes.Clear();
            mListLanguageTypes.Add(new LanguageTypeItem
            {
                LangID = 1033,
                Display = GetLanguage("Basic", "L1033", "English")
            });
            mListLanguageTypes.Add(new LanguageTypeItem
            {
                LangID = 2052,
                Display = GetLanguage("Basic", "L2052", "简体中文")
            });
        }

        private void LoadLayout()
        {
            try
            {
                var serializer = new XmlLayoutSerializer(DockingManagerMain);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layout.xml");
                if (File.Exists(path))
                {
                    using (var stream = new StreamReader(path))
                    {
                        serializer.Deserialize(stream);
                    }
                }
                //绑定事件及标题
                var document =
                    DockingManagerMain.Layout.Descendents()
                        .OfType<LayoutDocument>()
                        .FirstOrDefault(p => p.ContentId == "PanelScoreViewer");
                if (document != null)
                {
                    document.Title = GetLanguage("Designer", "P001", "Score Viewer");
                }
                var panel = GetPanleByContentID("PanelObject");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Title = GetLanguage("Designer", "P002", "Score Object Explorer");
                }
                panel = GetPanleByContentID("PanelProperty");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Title = GetLanguage("Designer", "P003", "Score Property Editor");
                }
                panel = GetPanleByContentID("PanelChildList");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Title = GetLanguage("Designer", "P004", "Child Lister");
                }
                panel = GetPanleByContentID("PanelChildProperty");
                if (panel != null)
                {
                    panel.IsVisibleChanged += (s, e) => SetViewStatus();
                    panel.Title = GetLanguage("Designer", "P005", "Child Property");
                }

                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "view.ini");
                if (File.Exists(path))
                {
                    string[] infos = File.ReadAllLines(path, Encoding.UTF8);
                    string strInfo, strValue;
                    bool bValue;
                    for (int i = 0; i < infos.Length; i++)
                    {
                        strInfo = infos[i];
                        if (string.IsNullOrEmpty(strInfo)) { continue; }
                        if (strInfo.StartsWith("[ViewHead]="))
                        {
                            strValue = strInfo.Substring(11);
                            if (bool.TryParse(strValue, out bValue))
                            {
                                ViewHead.Visibility = bValue ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                        if (strInfo.StartsWith("[Status]="))
                        {
                            strValue = strInfo.Substring(9);
                            if (bool.TryParse(strValue, out bValue))
                            {
                                StatusBar.Visibility = bValue ? Visibility.Visible : Visibility.Collapsed;
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

        #endregion


        #region EventHandlers

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
                //初始化可用的子项
                var itemStandard = scoreObject as ItemStandard;
                if (itemStandard != null)
                {
                    mListItemStandardItems.Clear();
                    for (int i = 0; i < itemStandard.ValueItems.Count; i++)
                    {
                        mListItemStandardItems.Add(itemStandard.ValueItems[i]);
                    }
                }
                ShowScoreViewer(scoreObject);

                BindPropertyGrid(scoreObject, ScoreItemPropertyGrid);
            }
            if (ViewHead != null && ViewHead.Visibility != Visibility.Visible)
            {
                ViewHead.Visibility = Visibility.Visible;
            }
            var panel = GetPanleByContentID("PanelProperty");

            if (!panel.IsVisible)
            {
                panel.Show();
            }
            SetViewStatus();
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

        private void NewObjectButton_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var newBtnItem = e.Parameter as ButtonItem;
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
                case "TextComment":
                    NewTextComment();
                    break;
                case "ItemComment":
                    NewItemComment();
                    break;
            }
            BtnNew.IsDropDownOpen = false;
            CreateNewButtons();
            mIsChanged = true;
            var panel = GetPanleByContentID("PanelObject");
            if (!panel.IsVisible)
            {
                panel.Show();
            }
        }

        private void BtnSave_Click(object sender, ExecutedRoutedEventArgs e)
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
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    item.InvalidMessage = invalidMessage;
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N_003", "Confirm to save ScoreSheet?")),
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

        private void BtnSaveTo_Click(object sender, ExecutedRoutedEventArgs e)
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
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    item.InvalidMessage = invalidMessage;
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N_003", "Confirm to save ScoreSheet?")),
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

        private void BtnDelete_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = TvObjects.SelectedItem;
            if (selectedItem == null) { return; }
            var selectedObj = selectedItem as ObjectItem;
            if (selectedObj == null) { return; }
            var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", GetLanguage("Designer", "N006", "Confirm Delete?"), selectedObj.Display), "UMP Score Designer", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    }
                    scoreItem = parent.Data as ScoreItem;
                    Comment comment = selectedObj.Data as Comment;
                    if (scoreItem != null && comment != null)
                    {
                        scoreItem.Comments.Remove(comment);
                    }
                    ObjectItem_PropertyChanged(selectedObj, scoreItem);
                    mIsChanged = true;
                    parent.RemoveChild(selectedObj);
                    if (selectedObj.ObjType == (int)ScoreObjectType.ScoreSheet)
                    {
                        mScoreDocument = null;
                        mIsChanged = false;
                    }
                    BorderViewer.Child = null;
                    LbTitle.DataContext = null;
                    ImageObject.DataContext = null;
                    BindPropertyGrid(null, ScoreItemPropertyGrid);
                }
            }
        }

        void BtnCaculate_Click(object sender, ExecutedRoutedEventArgs e)
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
                            scoreSheet.SetFlag(0);
                            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
                            selectObj.InvalidMessage = invalidMessage;
                            ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        scoreSheet.ResetFlag(0);
                        double score = scoreSheet.CaculateScore();
                        var viewer = BorderViewer.Child as StatisticalScoreSheetViewer;
                        if (viewer != null)
                        {
                            viewer.CaculateScore();
                        }
                        ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N002", "Score:"), score.ToString("0.00")));
                        return;
                    }
                    ScoreGroup scoreGroup = selectObj.Data as ScoreGroup;
                    if (scoreGroup != null)
                    {
                        result = scoreGroup.CheckValid();
                        selectObj.InvalidCode = result.Code;
                        if (result.Code != 0)
                        {
                            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
                            selectObj.InvalidMessage = invalidMessage;
                            ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        double score = scoreGroup.CaculateScore();
                        ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N002", "Score:"), score.ToString("0.00")));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnPrint_Click(object sender, ExecutedRoutedEventArgs e)
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
                var comment = scoreObject as Comment;
                if (comment != null)
                {
                    objectItem.Display = comment.Title;
                    objectItem.ToolTip = comment.Title;
                }
            }
        }

        void ScoreItemPropertyGrid_SelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            var property = e.NewValue as PropertyItem;
            var scoreObject = ScoreItemPropertyGrid.SelectedObject as ScoreObject;
            if (property != null && scoreObject != null)
            {
                if (property.PropertyDescriptor.Name == "ValueItems")
                {
                    var itemStandard = scoreObject as ItemStandard;
                    if (itemStandard != null)
                    {
                        BindChildList(property.PropertyDescriptor.Name, itemStandard);
                        var panel = GetPanleByContentID("PanelChildList");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                        return;
                    }
                    var itemComment = scoreObject as ItemComment;
                    if (itemComment != null)
                    {
                        BindChildList(property.PropertyDescriptor.Name, itemComment);
                        var panel = GetPanleByContentID("PanelChildList");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                        return;
                    }
                }
                if (property.PropertyDescriptor.Name == "ControlItems")
                {
                    var scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        BindChildList(property.PropertyDescriptor.Name, scoreSheet);
                        var panel = GetPanleByContentID("PanelChildList");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                        return;
                    }
                }
                if (property.PropertyDescriptor.Name == "TitleStyle")
                {
                    var scoreItem = scoreObject as ScoreItem;
                    if (scoreItem != null)
                    {
                        if (scoreItem.TitleStyle == null)
                        {
                            scoreItem.TitleStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = GetID(),
                                ScoreObject = scoreItem,
                                ScoreSheet = scoreItem.ScoreSheet,
                                ForeColor = Colors.Black,
                            };
                        }
                        BindPropertyGrid(scoreItem.TitleStyle, ChildPropertyGrid);
                        var panel = GetPanleByContentID("PanelChildProperty");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                    }
                    var comment = scoreObject as Comment;
                    if (comment != null)
                    {
                        if (comment.TitleStyle == null)
                        {
                            comment.TitleStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = GetID(),
                                ScoreObject = comment,
                                ScoreSheet = comment.ScoreSheet,
                                ForeColor = Colors.Black,
                            };
                        }
                        BindPropertyGrid(comment.TitleStyle, ChildPropertyGrid);
                        var panel = GetPanleByContentID("PanelChildProperty");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                    }
                    return;
                }
                if (property.PropertyDescriptor.Name == "PanelStyle")
                {
                    var scoreItem = scoreObject as ScoreItem;
                    if (scoreItem != null)
                    {
                        if (scoreItem.PanelStyle == null)
                        {
                            scoreItem.PanelStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = GetID(),
                                ScoreObject = scoreItem,
                                ScoreSheet = scoreItem.ScoreSheet,
                                ForeColor = Colors.Black,
                            };
                        }
                        BindPropertyGrid(scoreItem.PanelStyle, ChildPropertyGrid);
                        var panel = GetPanleByContentID("PanelChildProperty");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                    }
                    var comment = scoreObject as Comment;
                    if (comment != null)
                    {
                        if (comment.PanelStyle == null)
                        {
                            comment.PanelStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = GetID(),
                                ScoreObject = comment,
                                ScoreSheet = comment.ScoreSheet,
                                ForeColor = Colors.Black,
                            };
                        }
                        BindPropertyGrid(comment.PanelStyle, ChildPropertyGrid);
                        var panel = GetPanleByContentID("PanelChildProperty");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                    }
                    return;
                }
            }
            BindChildList(string.Empty, null);
            BindPropertyGrid(null, ChildPropertyGrid);
        }

        void PropertyChildList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<ScoreObject> e)
        {
            if (e.NewValue != null)
            {
                //初始化可用的评分项源和目标
                ControlItem controlItem = e.NewValue as ControlItem;
                if (controlItem != null)
                {
                    ScoreSheet scoreSheet = controlItem.ScoreSheet;
                    if (scoreSheet != null)
                    {
                        List<ScoreItem> listScoreItems = new List<ScoreItem>();
                        scoreSheet.GetAllScoreItem(ref listScoreItems);
                        mListControlSourceItems.Clear();
                        for (int i = 0; i < listScoreItems.Count; i++)
                        {
                            mListControlSourceItems.Add(listScoreItems[i]);
                        }
                        listScoreItems = new List<ScoreItem>();
                        scoreSheet.GetAllScoreItem(ref listScoreItems);
                        mListControlTargetItems.Clear();
                        for (int i = 0; i < listScoreItems.Count; i++)
                        {
                            mListControlTargetItems.Add(listScoreItems[i]);
                        }
                    }
                }

                BindPropertyGrid(e.NewValue, ChildPropertyGrid);
                var panel = GetPanleByContentID("PanelChildProperty");
                if (!panel.IsVisible)
                {
                    panel.Show();
                }
            }
        }

        void ChildPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ScoreObject scoreObject = ScoreItemPropertyGrid.SelectedObject as ScoreObject;
            if (scoreObject != null)
            {
                PropertyItem item = e.OriginalSource as PropertyItem;
                if (item != null)
                {
                    string propertyName = item.PropertyDescriptor.Name;
                    PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                    args.ScoreObject = scoreObject;
                    args.NewValue = e.NewValue;
                    args.OldValue = e.OldValue;
                    args.PropertyName = propertyName;
                    scoreObject.PropertyChanged(this, args);
                    mIsChanged = true;
                }
            }
        }

        void ScoreItemPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ScoreObject scoreObject = ScoreItemPropertyGrid.SelectedObject as ScoreObject;
            if (scoreObject != null)
            {
                PropertyItem item = e.OriginalSource as PropertyItem;
                if (item != null)
                {
                    string propertyName = item.PropertyDescriptor.Name;
                    if (propertyName == "ViewClassic")
                    {
                        TvObjects_OnSelectedItemChanged(this, null);
                        return;
                    }
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

        private void ExecuteChildListCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter as ChildListCommandArgs;
            if (param == null) { return; }
            string name = param.Name;
            var propertyChildList = param.Data as PropertyChildList;
            if (propertyChildList == null) { return; }
            var scoreObject = PropertyChildList.ScoreObject;
            if (scoreObject == null) { return; }
            PropertyChangedEventArgs args = new PropertyChangedEventArgs();
            args.ScoreObject = scoreObject;
            switch (name)
            {
                case "Add":
                    var itemStandard = scoreObject as ItemStandard;
                    if (itemStandard != null)
                    {
                        args.PropertyName = "ValueItems";
                        NewStandardItem(itemStandard);
                        propertyChildList.InitChildItems();
                        mIsChanged = true;
                    }
                    var itemComment = scoreObject as ItemComment;
                    if (itemComment != null)
                    {
                        args.PropertyName = "ValueItems";
                        NewCommentItem(itemComment);
                        propertyChildList.InitChildItems();
                        mIsChanged = true;
                    }
                    var scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        args.PropertyName = "ControlItems";
                        NewControlItem(scoreSheet);
                        propertyChildList.InitChildItems();
                        mIsChanged = true;
                    }
                    break;
                case "Remove":
                    itemStandard = scoreObject as ItemStandard;
                    if (itemStandard != null)
                    {
                        args.PropertyName = "ValueItems";
                        var standardItem = PropertyChildList.GetSelectedItem() as StandardItem;
                        if (standardItem != null)
                        {
                            itemStandard.ValueItems.Remove(standardItem);
                            mListItemStandardItems.Remove(standardItem);
                        }
                        propertyChildList.InitChildItems();
                        BindPropertyGrid(null, ChildPropertyGrid);
                        mIsChanged = true;
                    }
                    itemComment = scoreObject as ItemComment;
                    if (itemComment != null)
                    {
                        args.PropertyName = "ValueItems";
                        var commentItem = PropertyChildList.GetSelectedItem() as CommentItem;
                        if (commentItem != null)
                        {
                            itemComment.ValueItems.Remove(commentItem);
                        }
                        propertyChildList.InitChildItems();
                        BindPropertyGrid(null, ChildPropertyGrid);
                        mIsChanged = true;
                    }
                    scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        args.PropertyName = "ControlItems";
                        var controlItem = PropertyChildList.GetSelectedItem() as ControlItem;
                        if (controlItem != null)
                        {
                            scoreSheet.ControlItems.Remove(controlItem);
                        }
                        propertyChildList.InitChildItems();
                        BindPropertyGrid(null, ChildPropertyGrid);
                        mIsChanged = true;
                    }
                    break;
                case "Up":
                case "Down":
                    itemStandard = scoreObject as ItemStandard;
                    if (itemStandard != null)
                    {
                        args.PropertyName = "ValueItems";
                    }
                    itemComment = scoreObject as ItemComment;
                    if (itemComment != null)
                    {
                        args.PropertyName = "ValueItems";
                    }
                    scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        args.PropertyName = "ControlItems";
                    }
                    PropertyChildList.MoveScoreObject(name);
                    mIsChanged = true;
                    break;
            }
            scoreObject.PropertyChanged(this, args);
        }

        #endregion


        #region Others

        private void CreateNewButtons()
        {
            try
            {
                ButtonItem btnItem;
                ButtonItem childBtn;
                MenuItem menuItem;
                MenuItem childItem;
                Image icon;
                BtnNew.Items.Clear();
                if (TvObjects.Items == null || TvObjects.Items.Count <= 0)
                {
                    btnItem = new ButtonItem
                    {
                        Name = "ScoreSheet",
                        Header = GetObjectLanguage((int)ScoreObjectType.ScoreSheet, "ScoreSheet"),
                        Icon = "Images/template.ico",
                        ToolTip = "ScoreSheet"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = MainWindowCommands.NewObjectCommand;
                    menuItem.CommandParameter = btnItem;
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
                    btnItem = new ButtonItem
                    {
                        Name = "ScoreGroup",
                        Header = GetObjectLanguage((int)ScoreObjectType.ScoreGroup, "ScoreGroup"),
                        Icon = "Images/templateitem.ico",
                        ToolTip = "ScoreGroup"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = MainWindowCommands.NewObjectCommand;
                    menuItem.CommandParameter = btnItem;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);

                    btnItem = new ButtonItem
                    {
                        Name = "Standard",
                        Header = GetObjectLanguage((int)ScoreObjectType.Standard, "Standard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "Standard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.IsSplited = true;

                    childBtn = new ButtonItem
                    {
                        Name = "NumericStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.NumericStandard, "NumericStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "NumericStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "YesNoStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.YesNoStandard, "YesNoStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "YesNoStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "SliderStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.SliderStandard, "SliderStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "SliderStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "ItemStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.ItemStandard, "ItemStandard"),
                        Icon = "Images/standard.ico",
                        ToolTip = "ItemStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    BtnNew.Items.Add(menuItem);
                }
                //Comment
                if (
                    scoreObj.Type == ScoreObjectType.ScoreSheet
                    || scoreObj.Type == ScoreObjectType.ScoreGroup
                    || scoreObj.Type == ScoreObjectType.NumericStandard
                    || scoreObj.Type == ScoreObjectType.YesNoStandard
                    || scoreObj.Type == ScoreObjectType.SliderStandard
                    || scoreObj.Type == ScoreObjectType.ItemStandard
                    )
                {
                    btnItem = new ButtonItem
                    {
                        Name = "Comment",
                        Header = GetObjectLanguage((int)ScoreObjectType.Comment, "Comment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "Comment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.IsSplited = true;

                    childBtn = new ButtonItem
                    {
                        Name = "TextComment",
                        Header = GetObjectLanguage((int)ScoreObjectType.TextComment, "TextComment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "TextComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    childBtn = new ButtonItem
                    {
                        Name = "ItemComment",
                        Header = GetObjectLanguage((int)ScoreObjectType.ItemComment, "ItemComment"),
                        Icon = "Images/comment.ico",
                        ToolTip = "ItemComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = MainWindowCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    BtnNew.Items.Add(menuItem);
                }
                if (scoreObj.Type == ScoreObjectType.ScoreSheet)
                {
                    btnItem = new ButtonItem
                    {
                        Name = "ControlItem",
                        Header = GetObjectLanguage((int)ScoreObjectType.ControlItem, "ControlItem"),
                        Icon = "Images/controlitem.png",
                        ToolTip = "ControlItem"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = MainWindowCommands.NewObjectCommand;
                    menuItem.CommandParameter = btnItem;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                }

                #region Other

                #endregion

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveLayout(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(DockingManagerMain);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layout.xml");
                using (var stream = new StreamWriter(path))
                {
                    serializer.Serialize(stream);
                }
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "view.ini");
                string[] infos = new string[2];
                infos[0] = string.Format("[ViewHead]={0}", ViewHead.Visibility == Visibility.Visible);
                infos[1] = string.Format("[Status]={0}", StatusBar.Visibility == Visibility.Visible);
                File.WriteAllLines(path, infos, Encoding.UTF8);
                ShowInfoMessage(string.Format("SaveLayout end.\t{0}", path));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ResetLayout(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LoadLayout();
                SetViewStatus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void OpenScoreDocument()
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
                    ShowInfoMessage(string.Format("{0}", GetLanguage("Designer", "N005", "ScoreSheet already opened.")));
                    return;
                }
                if (mRootObject != null && mRootObject.Children.Count > 0)
                {
                    var openResult = MessageBox.Show(string.Format("{0}", GetLanguage("Designer", "N004", "Save the exist ScoreSheet?")), "UMPScoreSheetDesigner",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (openResult == MessageBoxResult.Yes)
                    {
                        SaveScoreObject(mScoreDocument);
                    }
                    mRootObject.Children.Clear();
                    mScoreDocument = null;
                }
                LoadScoreDocument(dialog.FileName);
                mIsChanged = false;
            }
        }

        private void SaveScoreObject(ScoreDocument scoreDocument)
        {
            if (scoreDocument == null) { return; }
            try
            {
                OperationReturn optReturn = XMLHelper.SerializeFile(scoreDocument, scoreDocument.FullPath);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N009", "Serialize fail."), optReturn.Code, optReturn.Message));
                    return;
                }
                ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N010", "Save end."), scoreDocument.FullPath));
                mIsChanged = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadScoreDocument(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    ShowErrorMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N011", "File not exist."), path));
                    return;
                }
                ScoreDocument scoreDocument = null;
                ScoreSheet scoreSheet;
                OperationReturn optReturn = XMLHelper.DeserializeFile<ScoreSheet>(path);
                if (!optReturn.Result)
                {
                    optReturn = XMLHelper.DeserializeFile<ScoreDocument>(path);
                    if (!optReturn.Result)
                    {
                        ShowErrorMessage(string.Format("{0}\t{1}\t{2}", GetLanguage("Designer", "N012", "Load ScoreSheet fail."), optReturn.Code, optReturn.Message));
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
                var panel = GetPanleByContentID("PanelObject");

                if (!panel.IsVisible)
                {
                    panel.Show();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
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
                                    scoreSheet.SetFlag(0);
                                    item.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
                                }
                                else
                                {
                                    scoreSheet.ResetFlag(0);
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
                    item.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
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
                            parent.InvalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
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
                        target.ObjType == (int)ScoreObjectType.ScoreGroup ||
                        target.ObjType == (int)ScoreObjectType.NumericStandard ||
                        target.ObjType == (int)ScoreObjectType.YesNoStandard ||
                        target.ObjType == (int)ScoreObjectType.SliderStandard ||
                        target.ObjType == (int)ScoreObjectType.ItemStandard)
                     )
                    {
                        ScoreItem sourceScoreItem = source.Data as ScoreItem;
                        ScoreItem targetScoreItem = target.Data as ScoreItem;
                        ScoreGroup targetGroup = target.Data as ScoreGroup;
                        //不允许移动到非ScoreGroup的下级
                        if (mDragType == 2 && targetGroup == null) { return; }
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
                mIsChanged = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ChangeStandardType(Standard standard, StandardType newType)
        {
            List<ScoreProperty> listProperties = new List<ScoreProperty>();
            var baseStandard = new Standard();
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
                    if ((listProperties[i].Flag & ScorePropertyFlag.Copy) != 0)
                    {
                        newStandard.CopyProperty(standard, newStandard, listProperties[i]);
                    }
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

        private void PrepareProperties(ScoreObject scoreObject, PropertyGrid propertyGrid)
        {
            List<ScoreProperty> listScoreProperties = new List<ScoreProperty>();
            scoreObject.GetPropertyList(ref listScoreProperties);
            listScoreProperties = listScoreProperties.OrderBy(s => s.OrderID).ToList();
            propertyGrid.PropertyDefinitions.Clear();
            propertyGrid.EditorDefinitions.Clear();
            for (int i = 0; i < listScoreProperties.Count; i++)
            {
                ScoreProperty scoreProperty = listScoreProperties[i];
                if ((scoreProperty.Flag & ScorePropertyFlag.Visible) != 0)
                {
                    PropertyDefinition pd = new PropertyDefinition();
                    pd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                    pd.DisplayName = GetPropertyLanguage(scoreProperty.ID, scoreProperty.Name);
                    pd.Description = GetDescriptionLanugage(scoreProperty.ID, scoreProperty.Description);
                    pd.Category = GetCategoryLanguage(scoreProperty.Category, scoreProperty.Category.ToString());
                    pd.DisplayOrder = scoreProperty.OrderID;
                    propertyGrid.PropertyDefinitions.Add(pd);
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
                            propertyGrid.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.Enum:
                            string[] enums = Enum.GetNames(scoreProperty.ValueType);
                            List<EnumItem> listEnum = new List<EnumItem>();
                            for (int k = 0; k < enums.Length; k++)
                            {
                                var temp = Enum.Parse(scoreProperty.ValueType, enums[k]);
                                EnumItem item = new EnumItem();
                                item.Name = enums[k];
                                item.Type = scoreProperty.ValueType;
                                item.Display = GetEnumValueLanguage(scoreProperty.ValueType, (int)temp, enums[k]);
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
                            fef.SetValue(ItemsControl.ItemsSourceProperty, listEnum);
                            fef.SetBinding(Selector.SelectedItemProperty, binding);
                            fef.SetBinding(ContentProperty, binding);
                            fef.SetResourceReference(ItemsControl.ItemTemplateProperty, "EnumEditorItem");
                            fef.SetResourceReference(ItemsControl.ItemContainerStyleProperty, "EnumEditorItemStyle");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            propertyGrid.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.Item:
                        case ScorePropertyDataType.Style:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorTextBlock));
                            fef.SetValue(TextBlock.TextProperty, "(......)");
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            propertyGrid.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.ScoreItem:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorComboBox));
                            if (scoreProperty.Name == "Source")
                            {
                                fef.SetValue(ItemsControl.ItemsSourceProperty, mListControlSourceItems);
                            }
                            else if (scoreProperty.Name == "Target")
                            {
                                fef.SetValue(ItemsControl.ItemsSourceProperty, mListControlTargetItems);
                            }
                            else { continue; }
                            fef.SetBinding(Selector.SelectedItemProperty, new Binding("Value"));
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            propertyGrid.EditorDefinitions.Add(etd);
                            break;
                        case ScorePropertyDataType.StandardItem:
                            etd = new EditorTemplateDefinition();
                            etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                            dt = new DataTemplate();
                            fef = new FrameworkElementFactory(typeof(PropertyGridEditorComboBox));
                            if (scoreProperty.Name == "DefaultValue")
                            {
                                fef.SetValue(ItemsControl.ItemsSourceProperty, mListItemStandardItems);
                            }
                            else { continue; }
                            fef.SetBinding(Selector.SelectedItemProperty, new Binding("Value"));
                            dt.VisualTree = fef;
                            dt.Seal();
                            etd.EditingTemplate = dt;
                            propertyGrid.EditorDefinitions.Add(etd);
                            break;
                    }
                }
            }
        }

        private void BindChildList(string property, ScoreObject scoreObject)
        {
            if (string.IsNullOrEmpty(property) || scoreObject == null)
            {
                PropertyChildList.ScoreObject = null;
                PropertyChildList.ChildName = string.Empty;
                PropertyChildList.InitChildItems();
                return;
            }
            PropertyChildList.ScoreObject = scoreObject;
            PropertyChildList.ChildName = GetLanguage("PropertyViewer", "P_Display_" + property, string.Empty);
            PropertyChildList.InitChildItems();
        }

        private void BindPropertyGrid(ScoreObject scoreObject, PropertyGrid propertyGrid)
        {
            if (scoreObject == null)
            {
                propertyGrid.SelectedObject = null;
                propertyGrid.SelectedObjectTypeName = string.Empty;
                propertyGrid.SelectedObjectName = string.Empty;
                return;
            }
            PrepareProperties(scoreObject, propertyGrid);
            propertyGrid.SelectedObject = null;
            propertyGrid.SelectedObject = scoreObject;
            propertyGrid.SelectedObjectTypeName = GetObjectLanguage((int)scoreObject.Type, scoreObject.Type.ToString());
            propertyGrid.SelectedObjectName = scoreObject.ToString();
        }

        private void ShowScoreViewer(ScoreObject scoreObject)
        {
            ScoreSheet scoreSheet;
            ScoreItemClassic viewClassic = ScoreItemClassic.Tree;
            switch (scoreObject.Type)
            {
                case ScoreObjectType.ScoreSheet:
                    scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        //ScoreSheetViewer viewer = new ScoreSheetViewer();
                        ////评分模式
                        //viewer.ViewMode = 0;
                        //viewer.ScoreSheet = scoreSheet;
                        //viewer.Settings = mListScoreSettings;
                        //viewer.Languages = mListLanguageInfos;
                        //viewer.LangID = mLanguageID;
                        //viewer.ViewClassic = scoreSheet.ViewClassic;
                        //BorderViewer.Child = viewer;

                        StatisticalScoreSheetViewer viewer = new StatisticalScoreSheetViewer();
                        viewer.ViewMode = 0;
                        viewer.ScoreSheet = scoreSheet;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = mLanguageID;
                        viewer.ViewClassic = scoreSheet.ViewClassic;
                        BorderViewer.Child = viewer;
                    }
                    break;
                case ScoreObjectType.ScoreGroup:
                    ScoreGroup scoreGroup = scoreObject as ScoreGroup;
                    if (scoreGroup != null)
                    {
                        scoreSheet = scoreGroup.ScoreSheet;
                        if (scoreSheet != null)
                        {
                            viewClassic = scoreSheet.ViewClassic;
                        }
                        ScoreGroupViewer viewer = new ScoreGroupViewer();
                        //评分模式
                        viewer.ViewMode = 0;
                        viewer.ScoreGroup = scoreGroup;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = mLanguageID;
                        viewer.ViewClassic = viewClassic;
                        BorderViewer.Child = viewer;
                    }
                    break;
                case ScoreObjectType.NumericStandard:
                case ScoreObjectType.YesNoStandard:
                case ScoreObjectType.SliderStandard:
                case ScoreObjectType.ItemStandard:
                    var standard = scoreObject as Standard;
                    if (standard != null)
                    {
                        scoreSheet = standard.ScoreSheet;
                        if (scoreSheet != null)
                        {
                            viewClassic = scoreSheet.ViewClassic;
                        }
                        StandardViewer viewer = new StandardViewer();
                        //评分模式
                        viewer.ViewMode = 0;
                        viewer.Standard = standard;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = mLanguageID;
                        viewer.ViewClassic = viewClassic;
                        BorderViewer.Child = viewer;
                    }
                    break;
                case ScoreObjectType.TextComment:
                case ScoreObjectType.ItemComment:
                    var comment = scoreObject as Comment;
                    if (comment != null)
                    {
                        CommentViewer viewer = new CommentViewer();
                        //评分模式
                        viewer.ViewMode = 0;
                        viewer.Comment = comment;
                        viewer.Settings = mListScoreSettings;
                        viewer.Languages = mListLanguageInfos;
                        viewer.LangID = mLanguageID;
                        viewer.ViewClassic = viewClassic;
                        BorderViewer.Child = viewer;
                    }
                    break;
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
                scoreSheet.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ScoreSheet, "ScoreSheet"));
                scoreSheet.ViewClassic = ScoreItemClassic.Tree;
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.TotalScore = 100.00;
                scoreSheet.Creator = GetID();
                scoreSheet.CreateTime = DateTime.Now;
                scoreSheet.Status = "Y";
                scoreSheet.DateFrom = DateTime.Now;
                scoreSheet.DateTo = DateTime.Parse("2199-12-31");
                scoreSheet.UseTag = 0;
                scoreSheet.QualifiedLine = 0;
                scoreSheet.ScoreType = ScoreType.Numeric;
                scoreSheet.Flag = 0;

                mRootObject.IsExpanded = true;
                CreateScoreSheetItem(mRootObject, scoreSheet, false);

                mCurrentScoreSheet = scoreSheet;
                Title = string.Format("{0} - {1}", mCurrentScoreSheet.Title, GetLanguage("Designer", "002", "Score Sheet Designer"));

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
                if (scoreSheet == null) { return; }
                ScoreGroup newGroup = new ScoreGroup();
                newGroup.ID = GetID();
                newGroup.ItemID = scoreSheet.GetNewItemID();
                newGroup.Type = ScoreObjectType.ScoreGroup;
                newGroup.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ScoreGroup, "ScoreGroup"));
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
                if (scoreSheet == null) { return; }
                NumericStandard newStandard = new NumericStandard();
                newStandard.ID = GetID();
                newStandard.ItemID = scoreSheet.GetNewItemID();
                newStandard.Type = ScoreObjectType.NumericStandard;
                newStandard.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.NumericStandard, "NumericStandard"));
                newStandard.ViewClassic = scoreSheet.ViewClassic;
                newStandard.ScoreType = scoreSheet.ScoreType;
                newStandard.Parent = scoreGroup;
                newStandard.ScoreSheet = scoreSheet;
                newStandard.UsePointSystem = scoreSheet.UsePointSystem;
                newStandard.StandardType = StandardType.Numeric;
                newStandard.ScoreClassic = StandardClassic.TextBox;
                newStandard.DefaultValue = 0;
                newStandard.MaxValue = 0;
                newStandard.MinValue = 0;
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
                if (scoreSheet == null) { return; }
                YesNoStandard yesNoStandard = new YesNoStandard();
                yesNoStandard.ID = GetID();
                yesNoStandard.ItemID = scoreSheet.GetNewItemID();
                yesNoStandard.Type = ScoreObjectType.YesNoStandard;
                yesNoStandard.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.YesNoStandard, "YesNoStandard"));
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
                if (scoreSheet == null) { return; }
                ItemStandard itemStandard = new ItemStandard();
                itemStandard.ID = GetID();
                itemStandard.ItemID = scoreSheet.GetNewItemID();
                itemStandard.Type = ScoreObjectType.ItemStandard;
                itemStandard.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ItemStandard, "ItemStandard"));
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
                if (scoreSheet == null) { return; }
                SliderStandard sliderStandard = new SliderStandard();
                sliderStandard.ID = GetID();
                sliderStandard.ItemID = scoreSheet.GetNewItemID();
                sliderStandard.Type = ScoreObjectType.SliderStandard;
                sliderStandard.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.SliderStandard, "SliderStandard"));
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

        private void NewTextComment()
        {
            try
            {
                ObjectItem selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                ScoreItem scoreItem = selectedItem.Data as ScoreItem;
                if (scoreItem == null) { return; }
                ScoreSheet scoreSheet = scoreItem.ScoreSheet;
                if (scoreSheet == null) { return; }
                TextComment textComment = new TextComment();
                textComment.Type = ScoreObjectType.TextComment;
                textComment.ID = GetID();
                textComment.ScoreItem = scoreItem;
                textComment.ScoreSheet = scoreSheet;
                textComment.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.TextComment, "TextComment"));
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
                ScoreSheet scoreSheet = scoreItem.ScoreSheet;
                if (scoreSheet == null) { return; }
                ItemComment itemComment = new ItemComment();
                itemComment.Type = ScoreObjectType.ItemComment;
                itemComment.ID = GetID();
                itemComment.ScoreItem = scoreItem;
                itemComment.ScoreSheet = scoreSheet;
                itemComment.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ItemComment, "ItemComment"));
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

        private void NewControlItem(ScoreSheet scoreSheet)
        {
            try
            {
                ControlItem controlItem = new ControlItem();
                controlItem.ID = GetID();
                controlItem.Type = ScoreObjectType.ControlItem;
                controlItem.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ControlItem, "ControlItem"));
                controlItem.ScoreSheet = scoreSheet;
                controlItem.OrderID = scoreSheet.ControlItems.Count;
                scoreSheet.ControlItems.Add(controlItem);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewStandardItem(ItemStandard itemStandard)
        {
            try
            {
                StandardItem standardItem = new StandardItem();
                standardItem.ID = GetID();
                standardItem.Type = ScoreObjectType.StandardItem;
                standardItem.Display = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.StandardItem, "StandardItem"));
                standardItem.ScoreItem = itemStandard;
                itemStandard.ValueItems.Add(standardItem);
                mListItemStandardItems.Add(standardItem);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void NewCommentItem(ItemComment itemComment)
        {
            try
            {
                CommentItem commentItem = new CommentItem();
                commentItem.Type = ScoreObjectType.CommentItem;
                commentItem.ID = GetID();
                commentItem.Text = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.CommentItem, "CommentItem"));
                commentItem.Comment = itemComment;
                itemComment.ValueItems.Add(commentItem);
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
                //CreateChildControlItem(item, scoreSheet);

                parent.AddChild(item);
                item.IsSelected = true;
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
                item.IsSelected = true;
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
                item.IsSelected = true;
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
                item.IsSelected = true;
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

                //CreateChildStandardItem(item, itemStandard);
                CreateChildComment(item, itemStandard);

                parent.AddChild(item);
                item.IsSelected = true;
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
                item.IsSelected = true;
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
                item.IsSelected = true;
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

                //CreateChildCommentItem(item, itemComment);

                parent.AddChild(item);
                item.IsSelected = true;
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

        #endregion


        #region Basics

        private void SetPanelVisibility()
        {
            var visible = CbObjects.IsChecked == true;
            var panel = GetPanleByContentID("PanelObject");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            visible = CbProperty.IsChecked == true;
            panel = GetPanleByContentID("PanelProperty");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            visible = CbChildList.IsChecked == true;
            panel = GetPanleByContentID("PanelChildList");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            visible = CbChildProperty.IsChecked == true;
            panel = GetPanleByContentID("PanelChildProperty");
            if (panel != null)
            {
                if (visible) { panel.Show(); }
                else { panel.Hide(); }
            }
            ViewHead.Visibility = CbViewHead.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            StatusBar.Visibility = CbStatue.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetViewStatus()
        {
            var panel = GetPanleByContentID("PanelObject");
            if (panel != null)
            {
                CbObjects.IsChecked = panel.IsVisible;
            }
            panel = GetPanleByContentID("PanelProperty");
            if (panel != null)
            {
                CbProperty.IsChecked = panel.IsVisible;
            }
            panel = GetPanleByContentID("PanelChildList");
            if (panel != null)
            {
                CbChildList.IsChecked = panel.IsVisible;
            }
            panel = GetPanleByContentID("PanelChildProperty");
            if (panel != null)
            {
                CbChildProperty.IsChecked = panel.IsVisible;
            }
            CbViewHead.IsChecked = ViewHead.Visibility == Visibility.Visible;
            CbStatue.IsChecked = StatusBar.Visibility == Visibility.Visible;
        }

        private long GetID()
        {
            mObjectID++;
            //评分对象的ID规则是：
            //301+日期+序号(4位，不足前面加0）
            //共19位，如：
            //3011503101323120001
            return Convert.ToInt64(string.Format("301{0}{1}", DateTime.Now.ToString("yyMMddHHmmss"), mObjectID.ToString("0000")));
        }

        private void ShowErrorMessage(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, "UMP ScoreSheet Designer", MessageBoxButton.OK, MessageBoxImage.Error));
            
        }

        private void ShowInfoMessage(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, "UMP ScoreSheet Designer", MessageBoxButton.OK, MessageBoxImage.Information));
           
        }

        private void SetDragDropState()
        {
            RadioDown.IsChecked = mDragType == 0;
            RadioUp.IsChecked = mDragType == 1;
            RadioChild.IsChecked = mDragType == 2;
        }

        private void ShowLanguages()
        {
            BtnAbout.Header = GetLanguage("ToolBar", "B001", "About");
            BtnClose.Header = GetLanguage("ToolBar", "B002", "Close");
            BtnNew.Header = GetLanguage("ToolBar", "B003", "New");
            BtnOpen.Header = GetLanguage("ToolBar", "B004", "Open");
            BtnSave.Header = GetLanguage("ToolBar", "B005", "Save");
            BtnSaveTo.Header = GetLanguage("ToolBar", "B006", "Save");
            BtnDelete.Header = GetLanguage("ToolBar", "B007", "Delete");
            BtnPrint.Header = GetLanguage("ToolBar", "B008", "Print");
            BtnCaculate.Header = GetLanguage("ToolBar", "B009", "Caculate");
            BtnSaveLayout.Header = GetLanguage("ToolBar", "B010", "Save Layout");
            BtnResetLayout.Header = GetLanguage("ToolBar", "B011", "Reset Layout");

            BtnAbout.ToolTip = GetLanguage("ToolBar", "BT001", "About");
            BtnClose.ToolTip = GetLanguage("ToolBar", "BT002", "Close");
            BtnNew.ToolTip = GetLanguage("ToolBar", "BT003", "New");
            BtnOpen.ToolTip = GetLanguage("ToolBar", "BT004", "Open");
            BtnSave.ToolTip = GetLanguage("ToolBar", "BT005", "Save");
            BtnSaveTo.ToolTip = GetLanguage("ToolBar", "BT006", "Save");
            BtnDelete.ToolTip = GetLanguage("ToolBar", "BT007", "Delete");
            BtnPrint.ToolTip = GetLanguage("ToolBar", "BT008", "Print");
            BtnCaculate.ToolTip = GetLanguage("ToolBar", "BT009", "Caculate");
            BtnSaveLayout.ToolTip = GetLanguage("ToolBar", "BT010", "Save Layout");
            BtnResetLayout.ToolTip = GetLanguage("ToolBar", "BT011", "Reset Layout");

            RadioDown.Header = GetLanguage("ToolBar", "R001", "As next object");
            RadioUp.Header = GetLanguage("ToolBar", "R002", "As pre object");
            RadioChild.Header = GetLanguage("ToolBar", "R003", "As child object");

            CbObjects.Header = GetLanguage("ToolBar", "C001", "Objects");
            CbProperty.Header = GetLanguage("ToolBar", "C002", "Property");
            CbViewHead.Header = GetLanguage("ToolBar", "C003", "Viewer Head");
            CbStatue.Header = GetLanguage("ToolBar", "C004", "Statue");
            CbChildList.Header = GetLanguage("ToolBar", "C005", "Statue");
            CbChildProperty.Header = GetLanguage("ToolBar", "C006", "Statue");

            CbObjects.ToolTip = GetLanguage("ToolBar", "C001", "Objects");
            CbProperty.ToolTip = GetLanguage("ToolBar", "C002", "Property");
            CbViewHead.ToolTip = GetLanguage("ToolBar", "C003", "Viewer Head");
            CbStatue.ToolTip = GetLanguage("ToolBar", "C004", "Statue");
            CbChildList.ToolTip = GetLanguage("ToolBar", "C005", "Statue");
            CbChildProperty.ToolTip = GetLanguage("ToolBar", "C006", "Statue");

            TabHome.Header = GetLanguage("ToolBar", "T001", "Home");

            GroupBasic.Header = GetLanguage("ToolBar", "G001", "Basic");
            GroupView.Header = GetLanguage("ToolBar", "G002", "View");
            GroupTool.Header = GetLanguage("ToolBar", "G003", "Tools");
            GroupLanguage.Header = GetLanguage("ToolBar", "G004", "Languages");
            GroupDragDrop.Header = GetLanguage("ToolBar", "G005", "Drag and Drop");
            GroupLayout.Header = GetLanguage("ToolBar", "G006", "Layout");

            var panel = DockingManagerMain.Layout.Descendents()
                .OfType<LayoutContent>()
                .FirstOrDefault(p => p.ContentId == "PanelScoreViewer");
            if (panel != null)
            {
                panel.Title = GetLanguage("Designer", "P001", "Score Viewer");
            }
            panel = GetPanleByContentID("PanelObject");
            if (panel != null)
            {
                panel.Title = GetLanguage("Designer", "P002", "Score Object Explorer");
            }
            panel = GetPanleByContentID("PanelProperty");
            if (panel != null)
            {
                panel.Title = GetLanguage("Designer", "P003", "Score Property Editor");
            }
            panel = GetPanleByContentID("PanelChildList");
            if (panel != null)
            {
                panel.Title = GetLanguage("Designer", "P004", "Child Lister");
            }
            panel = GetPanleByContentID("PanelChildProperty");
            if (panel != null)
            {
                panel.Title = GetLanguage("Designer", "P005", "Child Property");
            }
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

        private string GetObjectLanguage(int type, string display)
        {
            string strCode = string.Format("OBJ301{0}", type.ToString("000"));
            ScoreLangauge language =
                mListLanguageInfos.FirstOrDefault(
                    l => l.LangID == mLanguageID && l.Category == "ObjectViewer" && l.Code == strCode);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        private string GetPropertyLanguage(int id, string display)
        {
            string strCode = string.Format("PRO301{0}", id.ToString("000"));
            ScoreLangauge language =
                mListLanguageInfos.FirstOrDefault(
                    l => l.LangID == mLanguageID && l.Category == "PropertyViewer" && l.Code == strCode);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        private string GetDescriptionLanugage(int id, string display)
        {
            string strCode = string.Format("PROD301{0}", id.ToString("000"));
            ScoreLangauge language =
                mListLanguageInfos.FirstOrDefault(
                    l => l.LangID == mLanguageID && l.Category == "PropertyViewer" && l.Code == strCode);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        private string GetCategoryLanguage(int id, string display)
        {
            string strCode = string.Format("GRP301{0}", id.ToString("000"));
            ScoreLangauge language =
               mListLanguageInfos.FirstOrDefault(
                   l => l.LangID == mLanguageID && l.Category == "PropertyViewer" && l.Code == strCode);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        private string GetEnumValueLanguage(Type enumType, int enumValue, string display)
        {
            string strCode = string.Format("ENUM{0}{1}", enumType.Name, enumValue.ToString("000"));
            ScoreLangauge language =
               mListLanguageInfos.FirstOrDefault(
                   l => l.LangID == mLanguageID && l.Category == "PropertyViewer" && l.Code == strCode);
            if (language != null)
            {
                display = language.Display;
            }
            return display;
        }

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                DockingManagerMain.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
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
