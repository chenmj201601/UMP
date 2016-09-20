//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    64b9b9b5-9665-4f03-95a1-61a366e4cd6b
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetDesignPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                ScoreSheetDesignPage
//
//        created by Charley at 2014/10/14 11:42:36
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UMPS3101.Commands;
using UMPS3101.Models;
using UMPS3101.Wcf11012;
using UMPS3101.Wcf31011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.PropertyGrids;
using VoiceCyber.Wpf.PropertyGrids.Definitions;
using VoiceCyber.Wpf.PropertyGrids.Editors;
using EnumConverter = UMPS3101.Converters.EnumConverter;
using MenuItem = VoiceCyber.Ribbon.MenuItem;
using PropertyChangedEventArgs = VoiceCyber.UMP.ScoreSheets.PropertyChangedEventArgs;

namespace UMPS3101
{
    /// <summary>
    /// ScoreSheetDesignPage.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreSheetDesignPage
    {
        #region Members
        private int mIdleCheckCount;
        private int mIdleCheckInterval;
        private Timer mIdleCheckTimer;
        private bool mIsChanged;
        private ObjectItem mRootObject;
        private ObservableCollection<ScoreItem> mListControlSourceItems;
        private ObservableCollection<ScoreItem> mListControlTargetItems;
        private ObservableCollection<StandardItem> mListItemStandardItems;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private ObservableCollection<LanguageTypeItem> mListLanguageTypes;
        private ScoreSheet mCurrentScoreSheet;
        private int mLanguageID;
        private double mViewerScale;
        private int mDragType;
        private BackgroundWorker mWorker;
        private string mLayoutInfo;

        public ScoreSheet mScoresheet;

        #endregion

        public ScoreSheetDesignPage()
        {
            InitializeComponent();

            mListControlSourceItems = new ObservableCollection<ScoreItem>();
            mListControlTargetItems = new ObservableCollection<ScoreItem>();
            mListItemStandardItems = new ObservableCollection<StandardItem>();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListLanguageTypes = new ObservableCollection<LanguageTypeItem>();
            mRootObject = new ObjectItem();
            mLanguageID = 2052;
            mViewerScale = 1;
            mDragType = 0;
            mIsChanged = false;

            GridMain.MouseMove += GridMain_MouseMove;
            GridMain.KeyDown += GridMain_KeyDown;
            Loaded += ScoreSheetDesignPage_Loaded;
            App.NetPipeEvent += App_NetPipeEvent;
        }

        void ScoreSheetDesignPage_Loaded(object sender, RoutedEventArgs e)
        {
            TvObjects.ItemsSource = mRootObject.Children;
            GalleryLanguages.ItemsSource = mListLanguageTypes;
            SliderScale.Tag = mViewerScale;

            Init();
            BindCommands();
            BindEventHandlers();
            InitScoreSettings();
            InitLanuageTypes();
            SetDragDropState();

            MyWaiter.Visibility = Visibility.Visible;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                LoadLanguageInfos();
                TryLoadScoreSheet();
                LoadLayout();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                MyWaiter.Visibility = Visibility.Collapsed;
                CreateNewButtons();
                ShowLanguages();
                LanguageTypeItem languageType = mListLanguageTypes.FirstOrDefault(l => l.LangID == mLanguageID);
                if (languageType != null)
                {
                    GalleryLanguages.SelectedItem = languageType;
                }
                SetViewStatus();
                InitLayout();
                InitButtonEnable();
                SendLoadedMessage();
            };
            mWorker.RunWorkerAsync();
        }


        #region Init And Load


        private void InitButtonEnable()
        {
            //评过分的评分表不允许被修改
            if (App.IsModifyScoreSheet == true && App.CurrentScoreSheetInfo != null)
            {
                var basicScoreSheetInfo = App.CurrentScoreSheetInfo;
                if (basicScoreSheetInfo.UseFlag > 0)
                {
                    BtnNew.IsEnabled = false;
                    BtnSave.IsEnabled = false;
                    BtnDelete.IsEnabled = false;
                    PropertyChildList.PanelAddRemoveButton.IsEnabled = false;
                }
            }

        }

        private void Init()
        {
            try
            {
                mIdleCheckCount = 0;
                mIdleCheckInterval = 1000;
                mIdleCheckTimer = new Timer(mIdleCheckInterval);
                mIdleCheckTimer.Elapsed += mIdleCheckTimer_Elapsed;
                mIdleCheckTimer.Start();

                PageName = "SSDMainPage";
                StylePath = "UMPS3101/SSDMainPage.xaml";
                mLanguageID = App.Session.LangTypeInfo.LangID;
                ThemeInfo = App.Session.ThemeInfo;
                LangTypeInfo = App.Session.LangTypeInfo;
                AppServerInfo = App.Session.AppServerInfo;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBinding binding = new CommandBinding(SSDPageCommands.NavigateCommand);
            binding.Executed += NavigateCommand_Executed;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.ShowAboutCommand);
            binding.Executed += (s, e) => { };
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.New);
            binding.Executed += (s, e) => { };
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.NewObjectCommand);
            binding.Executed += NewObjectButton_Click;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Open);
            //binding.Executed += (s, e) => OpenScoreDocument();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.Save);
            binding.Executed += BtnSave_Click;
            binding.CanExecute += (s, e) => e.CanExecute = mIsChanged;
            CommandBindings.Add(binding);

            binding = new CommandBinding(ApplicationCommands.SaveAs);
            //binding.Executed += BtnSaveTo_Click;
            binding.CanExecute += (s, e) =>
            {
                if (mRootObject != null && mRootObject.Children.Count > 0)
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

            binding = new CommandBinding(SSDPageCommands.SetViewCommand);
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

            binding = new CommandBinding(SSDPageCommands.CaculateCommand);
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

            binding = new CommandBinding(SSDPageCommands.SetDragDropCommand);
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

            binding = new CommandBinding(SSDPageCommands.SaveLayoutCommand);
            binding.Executed += SaveLayout;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.ResetLayoutCommand);
            binding.Executed += ResetLayout;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.ChildListCommand);
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
            try
            {
                if (App.Session == null || App.Session.LangTypeInfo == null) { return; }
                mListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("31{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("3101{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session)
                    , WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        App.ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    ScoreLangauge scoreLanguage = new ScoreLangauge();
                    scoreLanguage.LangID = langInfo.LangID;
                    scoreLanguage.Display = langInfo.Display;

                    string name = langInfo.Name;
                    if (name.StartsWith("OBJ301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("PRO301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("PROD301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101GRP301"))
                    {
                        scoreLanguage.Code = name;
                        scoreLanguage.Category = langInfo.Page;
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101Designer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(12);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ToolBar"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(11);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ScoreViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(15);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101PropertyViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(18);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                    if (name.StartsWith("3101ObjectViewer"))
                    {
                        scoreLanguage.Category = langInfo.Page;
                        scoreLanguage.Code = name.Substring(16);
                        mListLanguageInfos.Add(scoreLanguage);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Load Language"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitLanuageTypes()
        {
            mListLanguageTypes.Clear();
            if (App.Session != null && App.Session.SupportLangTypes != null)
            {
                for (int i = 0; i < App.Session.SupportLangTypes.Count; i++)
                {
                    LangTypeInfo langTypeInfo = App.Session.SupportLangTypes[i];
                    LanguageTypeItem item = new LanguageTypeItem();
                    item.LangID = langTypeInfo.LangID;
                    item.Display = langTypeInfo.Display;
                    mListLanguageTypes.Add(item);
                }
            }
        }

        private void LoadLayout()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetLayoutInfo;
                webRequest.ListData.Add("310101");
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.WriteLog("LoadLayout", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strLayoutInfo = webReturn.Data;
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    App.WriteLog("LoadLayout", string.Format("Fail.\tLayoutInfo is empty"));
                    return;
                }
                mLayoutInfo = strLayoutInfo;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitLayout()
        {
            try
            {
                if (string.IsNullOrEmpty(mLayoutInfo)) { return; }
                byte[] byteLayoutInfo = Encoding.UTF8.GetBytes(mLayoutInfo);
                MemoryStream ms = new MemoryStream();
                ms.Write(byteLayoutInfo, 0, byteLayoutInfo.Length);
                ms.Position = 0;
                var serializer = new XmlLayoutSerializer(DockingManagerMain);
                using (var stream = new StreamReader(ms))
                {
                    serializer.Deserialize(stream);
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
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
            TxtTitle.DataContext = item;
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
            if (ViewHead.Visibility != Visibility.Visible)
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
                try
                {
                    LanguageTypeItem item = e.AddedItems[0] as LanguageTypeItem;
                    if (item != null)
                    {
                        mLanguageID = item.LangID;
                        ShowLanguages();
                        TvObjects_OnSelectedItemChanged(this, null);
                        LangTypeInfo langTypeInfo = App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID == mLanguageID);
                        if (langTypeInfo != null)
                        {
                            App.Session.LangTypeInfo = langTypeInfo;
                            App.Session.LangTypeID = langTypeInfo.LangID;
                            App.InitLanguageInfos();
                            SendLanguageChangeMessage();
                        }
                    }
                }
                catch
                {
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
                App.ShowExceptionMessage(ex.Message);
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
                CheckObjectItemValid(item);
                var scoreSheet = item.Data as ScoreSheet;
                if (scoreSheet == null) { return; }
                string invalidMessage;
                CheckValidResult checkResult = scoreSheet.CheckValid();
                item.InvalidCode = checkResult.Code;
                if (checkResult.Code != 0)
                {
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    item.InvalidMessage = invalidMessage;
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N003", "Confirm to save ScoreSheet?")),
                          "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmResult != MessageBoxResult.Yes) { return; }
                }
                SaveScoreSheetData(scoreSheet);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                        mIsChanged = false;
                    }
                    BorderViewer.Child = null;
                    TxtTitle.DataContext = null;
                    ImageObject.DataContext = null;
                    BindPropertyGrid(null, ScoreItemPropertyGrid);
                }
            }
        }

        private void NavigateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                var nav = e.Parameter.ToString();
                if (nav == "SSM")
                {
                    if (NavigationService != null)
                        NavigationService.Navigate(new Uri("SSMMainPage.xaml", UriKind.Relative));
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
                            App.ShowExceptionMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        scoreSheet.ResetFlag(0);
                        double score = scoreSheet.CaculateScore();
                        App.ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N_002", "Score:"), score.ToString("0.00")));
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
                            App.ShowExceptionMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        double score = scoreGroup.CaculateScore();
                        App.ShowInfoMessage(string.Format("{0}\t{1}", GetLanguage("Designer", "N002", "Score:"), score.ToString("0.00")));
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                App.ShowExceptionMessage(ex.Message);
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
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
                            long id = GetID();
                            if (id < 0) { return; }
                            scoreItem.TitleStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = id,
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
                        }
                    }
                    var comment = scoreObject as Comment;
                    if (comment != null)
                    {
                        if (comment.TitleStyle == null)
                        {
                            long id = GetID();
                            if (id < 0) { return; }
                            comment.TitleStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = id,
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
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
                            long id = GetID();
                            if (id < 0) { return; }
                            scoreItem.PanelStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = id,
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
                        }
                    }
                    var comment = scoreObject as Comment;
                    if (comment != null)
                    {
                        if (comment.PanelStyle == null)
                        {
                            long id = GetID();
                            if (id < 0) { return; }
                            comment.PanelStyle = new VisualStyle
                            {
                                Type = ScoreObjectType.VisualStyle,
                                ID = id,
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
                        if (!panel.IsActive)
                        {
                            panel.IsActive = true;
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
                        mListControlTargetItems.Add(scoreSheet);
                    }
                }

                BindPropertyGrid(e.NewValue, ChildPropertyGrid);
                //var panel = GetPanleByContentID("PanelChildProperty");
                //if (!panel.IsVisible)
                //{
                //    panel.Show();
                //}
                //if (!panel.IsActive)
                //{
                //    panel.IsActive = true;
                //}
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
                    //检查评分表
                    CheckScoreSheetValid();
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
                        mIsChanged = true;
                        return;
                    }
                    if (propertyName == "StandardType")
                    {
                        var standard = scoreObject as Standard;
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
                        Icon = "/Themes/Default/UMPS3101/Images/template.ico",
                        ToolTip = "ScoreSheet"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = SSDPageCommands.NewObjectCommand;
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
                        Icon = "/Themes/Default/UMPS3101/Images/templateitem.ico",
                        ToolTip = "ScoreGroup"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = SSDPageCommands.NewObjectCommand;
                    menuItem.CommandParameter = btnItem;
                    menuItem.DataContext = btnItem;
                    menuItem.Icon = icon;
                    menuItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    BtnNew.Items.Add(menuItem);

                    btnItem = new ButtonItem
                    {
                        Name = "Standard",
                        Header = GetObjectLanguage((int)ScoreObjectType.Standard, "Standard"),
                        Icon = "/Themes/Default/UMPS3101/Images/standard.ico",
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
                        Icon = "/Themes/Default/UMPS3101/Images/standard.ico",
                        ToolTip = "NumericStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "YesNoStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.YesNoStandard, "YesNoStandard"),
                        Icon = "/Themes/Default/UMPS3101/Images/standard.ico",
                        ToolTip = "YesNoStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "SliderStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.SliderStandard, "SliderStandard"),
                        Icon = "/Themes/Default/UMPS3101/Images/standard.ico",
                        ToolTip = "SliderStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);
                    childBtn = new ButtonItem
                    {
                        Name = "ItemStandard",
                        Header = GetObjectLanguage((int)ScoreObjectType.ItemStandard, "ItemStandard"),
                        Icon = "/Themes/Default/UMPS3101/Images/standard.ico",
                        ToolTip = "ItemStandard"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
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
                        Icon = "/Themes/Default/UMPS3101/Images/comment.ico",
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
                        Icon = "/Themes/Default/UMPS3101/Images/comment.ico",
                        ToolTip = "TextComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
                    childItem.CommandParameter = childBtn;
                    childItem.DataContext = childBtn;
                    childItem.Icon = icon;
                    childItem.SetResourceReference(StyleProperty, "MenuItemNewStyle");
                    menuItem.Items.Add(childItem);

                    childBtn = new ButtonItem
                    {
                        Name = "ItemComment",
                        Header = GetObjectLanguage((int)ScoreObjectType.ItemComment, "ItemComment"),
                        Icon = "/Themes/Default/UMPS3101/Images/comment.ico",
                        ToolTip = "ItemComment"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(childBtn.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    childItem = new MenuItem();
                    childItem.Command = SSDPageCommands.NewObjectCommand;
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
                        Icon = "/Themes/Default/UMPS3101/Images/controlitem.png",
                        ToolTip = "ControlItem"
                    };
                    icon = new Image();
                    icon.Source = new BitmapImage(new Uri(btnItem.Icon, UriKind.Relative));
                    icon.SetResourceReference(StyleProperty, "ScoreObjectIconStyle");
                    menuItem = new MenuItem();
                    menuItem.Command = SSDPageCommands.NewObjectCommand;
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveLayout(object sender, ExecutedRoutedEventArgs e)
        {
            //try
            //{
            //    string dir = App.Session.LocalMachineInfo.StrCommonApplicationData;
            //    dir = Path.Combine(dir, "UMPS3101");
            //    if (!Directory.Exists(dir))
            //    {
            //        Directory.CreateDirectory(dir);
            //    }
            //    var serializer = new XmlLayoutSerializer(DockingManagerMain);
            //    string path = Path.Combine(dir, "layout.xml");
            //    using (var stream = new StreamWriter(path))
            //    {
            //        serializer.Serialize(stream);
            //    }
            //    path = Path.Combine(dir, "view.ini");
            //    string[] infos = new string[2];
            //    infos[0] = string.Format("[ViewHead]={0}", ViewHead.Visibility == Visibility.Visible);
            //    infos[1] = string.Format("[Status]={0}", StatusBar.Visibility == Visibility.Visible);
            //    File.WriteAllLines(path, infos, Encoding.UTF8);
            //    App.ShowInfoMessage(string.Format("SaveLayout end.\t{0}", path));
            //}
            //catch (Exception ex)
            //{
            //    App.ShowExceptionMessage(ex.Message);
            //}

            try
            {
                MemoryStream ms = new MemoryStream();
                var serializer = new XmlLayoutSerializer(DockingManagerMain);
                string strLayoutInfo;
                using (var stream = new StreamWriter(ms))
                {
                    serializer.Serialize(stream);
                    ms.Position = 0;
                    byte[] byteLayoutInfo = new byte[ms.Length];
                    ms.Read(byteLayoutInfo, 0, byteLayoutInfo.Length);
                    strLayoutInfo = Encoding.UTF8.GetString(byteLayoutInfo);
                }
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tLayoutInfo is null"));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSSaveLayoutInfo;
                webRequest.ListData.Add("310101");
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(strLayoutInfo);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                App.ShowInfoMessage(App.GetMessageLanguageInfo("003", "Save layout end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ResetLayout(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LoadLayout();
                InitLayout();
                SetViewStatus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveScoreSheetData(ScoreSheet scoreSheet)
        {
            MyWaiter.Visibility = Visibility.Visible;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                try
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(scoreSheet);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3101Codes.SaveScoreSheetInfo;
                    webRequest.Session = App.Session;
                    webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                        WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    App.WriteLog("SaveScoreSheet", webReturn.Data);

                    #region 写操作日志

                    string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001Name"),
                        scoreSheet.Title);
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001TotalScore"),
                        scoreSheet.TotalScore);
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ViewClassic"),
                     Utils.FormatOptLogString(string.Format("3101Tip002{0}", (int)scoreSheet.ViewClassic)));
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ScoreType"),
                     Utils.FormatOptLogString(string.Format("3101Tip003{0}", (int)scoreSheet.ScoreType)));
                    App.WriteOperationLog(App.IsModifyScoreSheet ?
                        S3101Consts.OPT_MODIFYSCORESHEET.ToString() : S3101Consts.OPT_CREATESCORESHEET.ToString(),
                        ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion

                    App.ShowInfoMessage(App.GetMessageLanguageInfo("002", "Save ScoreSheet end"));
                    mIsChanged = false;
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                MyWaiter.Visibility = Visibility.Collapsed;
            };
            mWorker.RunWorkerAsync();
        }

        private void LoadScoreSheetData(BasicScoreSheetInfo scoreSheetInfo)
        {
            try
            {
                if (scoreSheetInfo != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3101Codes.GetScoreSheetInfo;
                    webRequest.Session = App.Session;
                    webRequest.ListData.Add(scoreSheetInfo.ID.ToString());
                    Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                        WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                    if (scoreSheet == null)
                    {
                        App.ShowExceptionMessage(string.Format("ScoreSheet is null"));
                        return;
                    }
                    scoreSheet.ScoreSheet = scoreSheet;
                    scoreSheet.Init();
                    mCurrentScoreSheet = scoreSheet;
                    CreateScoreSheetItem(mRootObject, scoreSheet, true, true);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CheckScoreSheetValid();
                        CreateNewButtons();
                        if (mRootObject.Children.Count > 0)
                        {
                            mRootObject.Children[0].IsSelected = true;
                        }
                        var panel = GetPanleByContentID("PanelObject");
                        if (!panel.IsVisible)
                        {
                            panel.Show();
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                App.ShowExceptionMessage(ex.Message);
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
                        ScoreGroup sourceScoreGroup = sourceParent.Data as ScoreGroup;

                        //不允许移动到非ScoreGroup的下级
                        if (mDragType == 2 && targetGroup == null) { return; }
                        if (sourceScoreItem != null && targetScoreItem != null)
                        {
                            RemoveScoreItem(source);
                            InsertScoreItem(source, target);

                            if (sourceScoreGroup != null)
                            {
                                ResetScoreObjectOrder(sourceScoreItem, sourceScoreGroup);
                            }
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
                        ScoreGroup targetGroup = target.Data as ScoreGroup;
                        ScoreGroup sourceScoreGroup = sourceParent.Data as ScoreGroup;

                        if (sourceScoreItem != null && targetGroup != null)
                        {
                            RemoveScoreItem(source);
                            AddScoreItem(source, target);

                            if (sourceScoreGroup != null)
                            {
                                ResetScoreObjectOrder(sourceScoreItem, sourceScoreGroup);
                                ResetScoreObjectOrder(sourceScoreItem, targetGroup);
                            }
                        }
                    }
                }
                mIsChanged = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ResetScoreObjectOrder(ScoreObject item, ScoreObject scoreObject)
        {
            //重新设置OrderID
            var scoreGroup = scoreObject as ScoreGroup;
            var scoreItem = item as ScoreItem;
            if (scoreGroup != null && scoreItem != null)
            {
                for (int i = 0; i < scoreGroup.Items.Count; i++)
                {
                    scoreGroup.Items[i].OrderID = i;
                }
            }
            scoreItem = scoreObject as ScoreItem;
            var comment = item as Comment;
            if (scoreItem != null && comment != null)
            {
                for (int i = 0; i < scoreItem.Comments.Count; i++)
                {
                    scoreItem.Comments[i].OrderID = 0;
                }
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
                        //case ScorePropertyDataType.MString:
                        //    etd = new EditorTemplateDefinition();
                        //    etd.TargetProperties = new List<string> { scoreProperty.PropertyName };
                        //    dt = new DataTemplate();
                        //    fef = new FrameworkElementFactory(typeof(MultiLineTextEditor));
                        //    MultLineTextConverter converter = new MultLineTextConverter();
                        //    binding = new Binding("Value");
                        //    binding.Converter = converter;
                        //    fef.SetBinding(ContentProperty, binding);
                        //    fef.SetBinding(MultiLineTextEditor.TextProperty, new Binding("Value"));
                        //    fef.SetValue(BackgroundProperty, Brushes.Transparent);
                        //    fef.SetResourceReference(StyleProperty, "MultiLineEditorStyle");
                        //    dt.VisualTree = fef;
                        //    dt.Seal();
                        //    etd.EditingTemplate = dt;
                        //    propertyGrid.EditorDefinitions.Add(etd);
                        //    break;
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
                        ScoreSheetViewer viewer = new ScoreSheetViewer();
                        //评分模式
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
                long id = GetID();
                if (id < 0) { return; }
                ScoreSheet scoreSheet = new ScoreSheet();
                scoreSheet.ID = id;
                scoreSheet.Type = ScoreObjectType.ScoreSheet;
                scoreSheet.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ScoreSheet, "ScoreSheet"));
                scoreSheet.ViewClassic = ScoreItemClassic.Tree;
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.TotalScore = 100.00;
                scoreSheet.Creator = App.Session.UserInfo.UserID;
                scoreSheet.CreateTime = DateTime.Now;
                scoreSheet.Status = "Y";
                scoreSheet.DateFrom = DateTime.Now;
                scoreSheet.DateTo = DateTime.Parse("2199-12-31");
                scoreSheet.UseTag = 0;
                scoreSheet.QualifiedLine = 0;
                scoreSheet.ScoreType = ScoreType.Numeric;
                scoreSheet.Flag = 0;

                CreateScoreSheetItem(mRootObject, scoreSheet, true, true);

                Dispatcher.Invoke(new Action(() =>
                {
                    mCurrentScoreSheet = scoreSheet;
                    mRootObject.IsExpanded = true;
                    Title = string.Format("{0} - {1}", mCurrentScoreSheet.Title, GetLanguage("Designer", "002", "Score Sheet Designer"));
                    TvObjects.Focus();
                }));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                newGroup.ID = id;
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
                CreateScoreGroupItem(selectedItem, newGroup, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                newStandard.ID = id;
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
                CreateNumericStandardItem(selectedItem, newStandard, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                yesNoStandard.ID = id;
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
                CreateYesNoStandardItem(selectedItem, yesNoStandard, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                itemStandard.ID = id;
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
                CreateItemStandardItem(selectedItem, itemStandard, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                sliderStandard.ID = id;
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
                sliderStandard.MinValue = 0;
                sliderStandard.MaxValue = 0;
                sliderStandard.Interval = 1;
                scoreGroup.Items.Add(sliderStandard);

                selectedItem.IsExpanded = true;
                CreateSliderStandardItem(selectedItem, sliderStandard, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                textComment.ID = id;
                textComment.ScoreItem = scoreItem;
                textComment.ScoreSheet = scoreSheet;
                textComment.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.TextComment, "TextComment"));
                textComment.Style = CommentStyle.Text;
                scoreItem.Comments.Add(textComment);

                selectedItem.IsExpanded = true;
                CreateTextCommentItem(selectedItem, textComment, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                long id = GetID();
                if (id < 0) { return; }
                itemComment.ID = id;
                itemComment.ScoreItem = scoreItem;
                itemComment.ScoreSheet = scoreSheet;
                itemComment.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ItemComment, "ItemComment"));
                itemComment.Style = CommentStyle.Item;
                scoreItem.Comments.Add(itemComment);

                selectedItem.IsExpanded = true;
                CreateItemCommentItem(selectedItem, itemComment, true, true);

                TvObjects.Focus();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NewControlItem(ScoreSheet scoreSheet)
        {
            try
            {
                ControlItem controlItem = new ControlItem();
                long id = GetID();
                if (id < 0) { return; }
                controlItem.ID = id;
                controlItem.Type = ScoreObjectType.ControlItem;
                controlItem.Title = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.ControlItem, "ControlItem"));
                controlItem.ScoreSheet = scoreSheet;
                controlItem.OrderID = scoreSheet.ControlItems.Count;
                scoreSheet.ControlItems.Add(controlItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NewStandardItem(ItemStandard itemStandard)
        {
            try
            {
                StandardItem standardItem = new StandardItem();
                long id = GetID();
                if (id < 0) { return; }
                standardItem.ID = id;
                standardItem.Type = ScoreObjectType.StandardItem;
                standardItem.Display = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.StandardItem, "StandardItem"));
                standardItem.ScoreItem = itemStandard;
                itemStandard.ValueItems.Add(standardItem);
                mListItemStandardItems.Add(standardItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NewCommentItem(ItemComment itemComment)
        {
            try
            {
                CommentItem commentItem = new CommentItem();
                commentItem.Type = ScoreObjectType.CommentItem;
                long id = GetID();
                if (id < 0) { return; }
                commentItem.ID = id;
                commentItem.Text = string.Format("{0} {1}", GetLanguage("Designer", "001", "New"), GetObjectLanguage((int)ScoreObjectType.CommentItem, "CommentItem"));
                commentItem.Comment = itemComment;
                itemComment.ValueItems.Add(commentItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region CreateObjectItem

        private void CreateScoreSheetItem(ObjectItem parent, ScoreSheet scoreSheet, bool isExpanded, bool isSelected)
        {
            try
            {
                if (scoreSheet == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.ScoreSheet;
                item.Display = scoreSheet.Title;
                item.ToolTip = scoreSheet.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/template.ico";
                item.Data = scoreSheet;

                CreateChildItem(item, scoreSheet);
                CreateChildComment(item, scoreSheet);
                //CreateChildControlItem(item, scoreSheet);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateScoreGroupItem(ObjectItem parent, ScoreGroup scoreGroup, bool isExpanded, bool isSelected)
        {
            try
            {
                if (scoreGroup == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.ScoreGroup;
                item.Display = scoreGroup.Title;
                item.ToolTip = scoreGroup.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/TemplateItem.ico";
                item.Data = scoreGroup;

                CreateChildItem(item, scoreGroup);
                CreateChildComment(item, scoreGroup);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateNumericStandardItem(ObjectItem parent, NumericStandard numericStandard, bool isExpanded, bool isSelected)
        {
            try
            {
                if (numericStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.NumericStandard;
                item.Display = numericStandard.Title;
                item.ToolTip = numericStandard.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/standard.ico";
                item.Data = numericStandard;

                CreateChildComment(item, numericStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateYesNoStandardItem(ObjectItem parent, YesNoStandard yesNoStandard, bool isExpanded, bool isSelected)
        {
            try
            {
                if (yesNoStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.YesNoStandard;
                item.Display = yesNoStandard.Title;
                item.ToolTip = yesNoStandard.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/standard.ico";
                item.Data = yesNoStandard;

                CreateChildComment(item, yesNoStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateItemStandardItem(ObjectItem parent, ItemStandard itemStandard, bool isExpanded, bool isSelected)
        {
            try
            {
                if (itemStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.ItemStandard;
                item.Display = itemStandard.Title;
                item.ToolTip = itemStandard.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/standard.ico";
                item.Data = itemStandard;

                //CreateChildStandardItem(item, itemStandard);
                CreateChildComment(item, itemStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateSliderStandardItem(ObjectItem parent, SliderStandard sliderStandard, bool isExpanded, bool isSelected)
        {
            try
            {
                if (sliderStandard == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.SliderStandard;
                item.Display = sliderStandard.Title;
                item.ToolTip = sliderStandard.Title;
                item.Icon = "/Themes/Default/UMPS3101/Images/standard.ico";
                item.Data = sliderStandard;

                CreateChildComment(item, sliderStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateTextCommentItem(ObjectItem parent, TextComment textComment, bool isExpanded, bool isSelected)
        {
            try
            {
                if (textComment == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.TextComment;
                item.Display = textComment.Title;
                item.ToolTip = textComment.Description;
                item.Icon = "/Themes/Default/UMPS3101/Images/comment.ico";
                item.Data = textComment;

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateItemCommentItem(ObjectItem parent, ItemComment itemComment, bool isExpanded, bool isSelected)
        {
            try
            {
                if (itemComment == null) { return; }
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = (int)ScoreObjectType.ItemComment;
                item.Display = itemComment.Title;
                item.ToolTip = itemComment.Description;
                item.Icon = "/Themes/Default/UMPS3101/Images/comment.ico";
                item.Data = itemComment;

                //CreateChildCommentItem(item, itemComment);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                            CreateScoreGroupItem(parent, scoreItem as ScoreGroup, false, false);
                            break;
                        case ScoreObjectType.NumericStandard:
                            CreateNumericStandardItem(parent, scoreItem as NumericStandard, false, false);
                            break;
                        case ScoreObjectType.YesNoStandard:
                            CreateYesNoStandardItem(parent, scoreItem as YesNoStandard, false, false);
                            break;
                        case ScoreObjectType.ItemStandard:
                            CreateItemStandardItem(parent, scoreItem as ItemStandard, false, false);
                            break;
                        case ScoreObjectType.SliderStandard:
                            CreateSliderStandardItem(parent, scoreItem as SliderStandard, false, false);
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
                            CreateTextCommentItem(parent, comment as TextComment, false, false);
                            break;
                        case CommentStyle.Item:
                            CreateItemCommentItem(parent, comment as ItemComment, false, false);
                            break;
                    }
                }
            }
        }

        #endregion


        #region Basic

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
            //mObjectID++;
            //return Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss") + mObjectID.ToString("0000"));
            long id = GetSerialID();
            if (id < 0)
            {
                App.ShowExceptionMessage(string.Format("SerialID invalid"));
            }
            return id;
        }

        private long GetSerialID()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("301");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return -1;
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
            if (mCurrentScoreSheet != null)
            {
                Title = string.Format("{0} - {1}", mCurrentScoreSheet.Title, GetLanguage("Designer", "002", "Score Sheet Designer"));
            }
            else
            {
                Title = string.Format("{0}", GetLanguage("Designer", "002", "Score Sheet Designer"));
            }

            BtnNavSSM.Header = GetLanguage("ToolBar", "B012", "Navigate to SSM");
            BtnNew.Header = GetLanguage("ToolBar", "B003", "New");
            BtnSave.Header = GetLanguage("ToolBar", "B005", "Save");
            BtnDelete.Header = GetLanguage("ToolBar", "B007", "Delete");
            BtnPrint.Header = GetLanguage("ToolBar", "B008", "Print");
            BtnCaculate.Header = GetLanguage("ToolBar", "B009", "Caculate");
            BtnSaveLayout.Header = GetLanguage("ToolBar", "B010", "Save Layout");
            BtnResetLayout.Header = GetLanguage("ToolBar", "B011", "Reset Layout");
            BtnNavSSM.ToolTip = GetLanguage("ToolBar", "BT012", "Navigate to SSM");
            BtnNew.ToolTip = GetLanguage("ToolBar", "BT003", "New");
            BtnSave.ToolTip = GetLanguage("ToolBar", "BT005", "Save");
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
            CbChildList.Header = GetLanguage("ToolBar", "C005", "Child List");
            CbChildProperty.Header = GetLanguage("ToolBar", "C006", "Child Property");

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
            try
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
            catch (Exception)
            {
                return display;
            }
        }

        private string GetObjectLanguage(int type, string display)
        {
            try
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
            catch (Exception)
            {
                return display;
            }
        }

        private string GetPropertyLanguage(int id, string display)
        {
            try
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
            catch (Exception)
            {
                return display;
            }
        }

        private string GetDescriptionLanugage(int id, string display)
        {
            try
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
            catch (Exception)
            {

                return display;
            }
        }

        private string GetCategoryLanguage(int id, string display)
        {
            try
            {
                string strCode = string.Format("3101GRP301{0}", id.ToString("000"));
                ScoreLangauge language =
                   mListLanguageInfos.FirstOrDefault(
                       l => l.LangID == mLanguageID && l.Category == "PropertyViewer" && l.Code == strCode);
                if (language != null)
                {
                    display = language.Display;
                }
                return display;
            }
            catch (Exception)
            {

                return display;
            }
        }

        private string GetEnumValueLanguage(Type enumType, int enumValue, string display)
        {
            try
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
            catch (Exception)
            {
                return display;
            }
        }

        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                DockingManagerMain.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        private void TryLoadScoreSheet()
        {
            try
            {
                if (App.IsModifyScoreSheet && App.CurrentScoreSheetInfo != null)
                {
                    BasicScoreSheetInfo scoreSheetInfo = App.CurrentScoreSheetInfo;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Title = scoreSheetInfo.Name + " - " + "ScoreSheet Designer";
                    }));
                    LoadScoreSheetData(scoreSheetInfo);
                }
                else
                {
                    NewScoreSheet();
                    mIsChanged = true;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddChildItem(ObjectItem parent, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(item)));
        }

        private void AddChildItem(ObjectItem parent, ObjectItem item, bool isExpanded, bool isSelected)
        {
            item.IsExpanded = isExpanded;
            item.IsSelected = isSelected;
            AddChildItem(parent, item);
        }

        #endregion


        #region DragDrop

        private void ObjectItem_StartDragged(object sender, DragDropEventArgs e)
        {
            var dragSource = e.DragSource;
            var dragData = sender as ObjectItem;
            if (dragSource != null && dragData != null)
            {
                DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Move);
            }

        }

        private void ObjectItem_DragOver(object sender, DragDropEventArgs e)
        {

        }

        private void ObjectItem_Dropped(object sender, DragDropEventArgs e)
        {
            var targetItem = sender as ObjectItem;
            var sourceData = e.DragData as IDataObject;
            if (sourceData != null)
            {
                var sourceItem = sourceData.GetData(typeof(ObjectItem)) as ObjectItem;
                if (sourceItem == null || targetItem == null || sourceItem == targetItem)
                {
                    return;
                }
                MoveScoreObject(sourceItem, targetItem);
            }

        }

        #endregion


        #region NetPipeMessage

        void App_NetPipeEvent(WebRequest webRequest)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                MyWaiter.Visibility = Visibility.Visible;
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    MyWaiter.Visibility = Visibility.Collapsed;
                                    mLanguageID = LangTypeInfo.LangID;
                                    ShowLanguages();
                                    for (int i = 0; i < GalleryLanguages.Items.Count; i++)
                                    {
                                        LanguageTypeItem item = GalleryLanguages.Items[i] as LanguageTypeItem;
                                        if (item != null)
                                        {
                                            if (item.LangID == LangTypeInfo.LangID)
                                            {
                                                GalleryLanguages.SelectedIndex = i;
                                            }
                                        }
                                    }
                                    ChangeLanguage();
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                        case (int)RequestCode.SCIdleCheckStop:
                            StartStopIdleTimer(false);
                            break;
                        case (int)RequestCode.SCIdleCheckStart:
                            StartStopIdleTimer(true);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        private bool SendLoadingMessage()
        {
            try
            {
                WebRequest request = new WebRequest();
                request.Code = (int)RequestCode.CSModuleLoading;
                request.Session = App.Session;
                var webReturn = App.SendNetPipeMessage(request);
                if (webReturn.Result)
                {
                    var dbType = webReturn.Session.DBType;
                    var dbConnectionString = webReturn.Session.DBConnectionString;
                    var roleInfo = webReturn.Session.RoleInfo;
                    var roleName = roleInfo.Name;
                    var appServerInfo = webReturn.Session.AppServerInfo;
                    var host = appServerInfo.Address;
                    var port = appServerInfo.Port;
                    var userInfo = webReturn.Session.UserInfo;
                    var userAccount = userInfo.Account;
                    string strMsg = string.Format("DBType:{0}\r\nDBConnectionString:{1}\r\nRoleName:{2}\r\nAppServerHost:{3}\r\nAppServerPort:{4},\r\nUserAccount:{5}"
                        , dbType
                        , dbConnectionString
                        , roleName
                        , host
                        , port
                        , userAccount);
                    App.ShowInfoMessage(string.Format("{0}", strMsg));
                    App.Session.DBType = dbType;
                    App.Session.DBConnectionString = dbConnectionString;
                    App.Session.AppServerInfo = appServerInfo;
                    App.Session.RoleInfo = roleInfo;
                    App.Session.UserInfo = userInfo;
                    Init();
                    return true;
                }
                App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            return false;
        }

        private void SendLoadedMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSModuleLoaded;
            request.Session = App.Session;
            App.SendNetPipeMessage(request);
        }

        private void SendThemeChangeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSThemeChange;
            request.Session = App.Session;
            request.Data = ThemeInfo.Name;
            App.SendNetPipeMessage(request);
        }

        private void SendLanguageChangeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSLanguageChange;
            request.Session = App.Session;
            request.Data = LangTypeInfo.LangID.ToString();
            App.SendNetPipeMessage(request);
        }

        private void SendLogoutMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSLogout;
            request.Session = App.Session;
            App.SendNetPipeMessage(request);
        }

        private void SendChangePasswordMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSChangePassword;
            request.Session = App.Session;
            App.SendNetPipeMessage(request);
        }

        private void SendNavigateHomeMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSHome;
            request.Session = App.Session;
            App.SendNetPipeMessage(request);
        }

        private void SendIdleCheckMessage()
        {
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSIdleCheck;
            request.Session = App.Session;
            request.Data = mIdleCheckCount.ToString();
            App.SendNetPipeMessage(request);
        }

        #endregion


        #region Login Timeout 相关

        void mIdleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                mIdleCheckCount++;
                SendIdleCheckMessage();
            }
            catch { }
        }

        void GridMain_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                mIdleCheckCount = 0;
            }
            catch { }
        }

        void GridMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                mIdleCheckCount = 0;
            }
            catch { }
        }

        private void StartStopIdleTimer(bool isStart)
        {
            if (isStart)
            {
                mIdleCheckCount = 0;
                mIdleCheckTimer.Start();
            }
            else
            {
                mIdleCheckTimer.Stop();
                mIdleCheckCount = 0;
            }
        }

        #endregion
    }
}
