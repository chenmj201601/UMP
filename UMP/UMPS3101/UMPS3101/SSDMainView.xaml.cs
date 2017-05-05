//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    06d8f9a3-50c9-4391-9e96-0e70d4c9b811
//        CLR Version:              4.0.30319.42000
//        Name:                     SSDMainView
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                SSDMainView
//
//        created by Charley at 2016/2/23 16:21:22
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS3101.Commands;
using UMPS3101.Models;
using UMPS3101.Wcf11012;
using UMPS3101.Wcf31011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.UMP.ScoreSheets.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3101
{
    /// <summary>
    /// SSDMainView.xaml 的交互逻辑
    /// </summary>
    public partial class SSDMainView
    {

        #region Members

        private BackgroundWorker mWorker;
        private ObjectItem mRootObject;
        private ObjectItem mRootStatisticalObject;

        private List<StatisticalInfo> mListStatisticalInfos;
        private List<StatisticalParamInfo> mListStatisticalParamInfos;
        private List<ScoreSetting> mListScoreSettings;
        private List<ScoreLangauge> mListLanguageInfos;
        private List<PanelItem> mListPanels;
        private ScoreSheet mCurrentScoreSheet;
        private ScoreObject mCurrentScoreObject;
        private int mLanguageID;
        private double mViewerScale;
        private int mDragType;
        private string mLayoutInfo;
        private bool mIsChanged;

        #endregion


        public SSDMainView()
        {
            InitializeComponent();

            mRootObject = new ObjectItem();
            mRootStatisticalObject = new ObjectItem();
            mListScoreSettings = new List<ScoreSetting>();
            mListLanguageInfos = new List<ScoreLangauge>();
            mListStatisticalInfos = new List<StatisticalInfo>();
            mListStatisticalParamInfos = new List<StatisticalParamInfo>();
            mListPanels = new List<PanelItem>();
            mLanguageID = 2052;
            mViewerScale = 1;
            mDragType = 0;
            mIsChanged = false;

            TvObjects.SelectedItemChanged += TvObjects_SelectedItemChanged;
            TvStatisticalObjects.SelectedItemChanged += TvStatisticalObjects_SelectedItemChanged;
            SliderScale.ValueChanged += SliderScale_ValueChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "SSDMainPage";
                StylePath = "UMPS3101/SSDMainPage.xaml";

                base.Init();

                TvObjects.ItemsSource = mRootObject.Children;
                TvStatisticalObjects.ItemsSource = mRootStatisticalObject.Children;
                BindCommands();
                SliderScale.Tag = mViewerScale;
                mLanguageID = CurrentApp.Session.LangTypeID;

                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    CurrentApp.SendLoadedMessage();

                    InitPanels();
                    InitScoreSettings();
                    LoadLanguageInfos();
                    LoadLayout();
                    TryLoadScoreSheet();
                    InitStatisticalParamInfos();
                    LoadStatisticalInfos();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    InitLayout();
                    CreateToolBarButtons();
                    CreateBasicButtons();
                    CreateOtherPosition();
                    InitStatisticalItems();
                    SetPanelVisible();
                    SetViewStatus();

                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBinding binding = new CommandBinding(SSDPageCommands.NavigateCommand);
            //binding.Executed += NavigateCommand_Executed;
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
            //binding.Executed += NewObjectButton_Click;
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
            //binding.Executed += (s, e) => SetPanelVisibility();
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
            binding.Executed += (s, e) => SaveLayout();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.ResetLayoutCommand);
            binding.Executed += (s, e) => ResetLayout();
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);

            binding = new CommandBinding(SSDPageCommands.ChildListCommand);
            binding.Executed += ExecuteChildListCommand;
            binding.CanExecute += (s, e) => e.CanExecute = true;
            CommandBindings.Add(binding);
        }

        private void InitPanels()
        {
            try
            {
                mListPanels.Clear();

                PanelItem panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_SCOREOBJECT;
                panelItem.Name = S3101Consts.PANEL_NAME_SCOREOBJECT;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_SCOREOBJECT;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_SCOREOBJECT.ToString("000")), "Score object browser");
                panelItem.Icon = "Images/00001.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_SCORETEMPLATE;
                panelItem.Name = S3101Consts.PANEL_NAME_SCORETEMPLATE;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_SCORETEMPLATE;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_SCORETEMPLATE.ToString("000")), "Score template");
                panelItem.Icon = "Images/00002.png";
                panelItem.IsVisible = false;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_AUTOSTANDARD;
                panelItem.Name = S3101Consts.PANEL_NAME_AUTOSTANDARD;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_AUTOSTANDARD;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_AUTOSTANDARD.ToString("000")), "Auto standard");
                panelItem.Icon = "Images/00003.png";
                panelItem.IsVisible = false;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_SCOREVIEWER;
                panelItem.Name = S3101Consts.PANEL_NAME_SCOREVIEWER;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_SCOREVIEWER;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_SCOREVIEWER.ToString("000")), "Score viewer");
                panelItem.Icon = "Images/00007.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = false;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_SCOREPROPERTY;
                panelItem.Name = S3101Consts.PANEL_NAME_SCOREPROPERTY;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_SCOREPROPERTY;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_SCOREPROPERTY.ToString("000")), "Score property");
                panelItem.Icon = "Images/00004.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_CHILDLISTER;
                panelItem.Name = S3101Consts.PANEL_NAME_CHILDLISTER;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_CHILDLISTER;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_CHILDLISTER.ToString("000")), "Child lister");
                panelItem.Icon = "Images/00006.png";
                panelItem.IsVisible = false;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelID = S3101Consts.PANEL_ID_CHILDPROPERTY;
                panelItem.Name = S3101Consts.PANEL_NAME_CHILDPROPERTY;
                panelItem.ContentID = S3101Consts.PANEL_CONTENTID_CHILDPROPERTY;
                panelItem.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", S3101Consts.PANEL_ID_CHILDPROPERTY.ToString("000")), "Child property");
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = false;
                panelItem.CanClose = true;
                mListPanels.Add(panelItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                if (CurrentApp.Session == null || CurrentApp.Session.LangTypeInfo == null) { return; }
                mListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("31{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("3101{0}0", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowException(string.Format("LanguageInfo is null"));
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

                CurrentApp.WriteLog("PageLoad", string.Format("Load Language"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadStatisticalInfos()
        {
            try
            {
                mListStatisticalInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3101Codes.GetStatisticalInfoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<StatisticalInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalInfo info = optReturn.Data as StatisticalInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("StatisticalInfo is null"));
                        return;
                    }
                    mListStatisticalInfos.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadLayout()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetLayoutInfo;
                webRequest.ListData.Add("310101");
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("LoadLayout", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strLayoutInfo = webReturn.Data;
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    CurrentApp.WriteLog("LoadLayout", string.Format("Fail.\tLayoutInfo is empty"));
                    return;
                }
                mLayoutInfo = strLayoutInfo;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                var serializer = new XmlLayoutSerializer(PanelManager);
                using (var stream = new StreamReader(ms))
                {
                    serializer.Deserialize(stream);
                }
                for (int i = 0; i < mListPanels.Count; i++)
                {
                    PanelItem item = mListPanels[i];
                    var panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == item.ContentID);
                    if (panel != null)
                    {
                        item.IsVisible = panel.IsVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void TryLoadScoreSheet()
        {
            try
            {
                if (S3101App.IsModifyScoreSheet && S3101App.CurrentScoreSheetInfo != null)
                {
                    BasicScoreSheetInfo scoreSheetInfo = S3101App.CurrentScoreSheetInfo;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //Title = scoreSheetInfo.Name + " - " + "ScoreSheet Designer";
                        if (scoreSheetInfo.UseFlag > 0)
                        {
                            //评分表被使用了，不能进行修改
                            PanelCreateObject.IsEnabled = false;
                            PanelBasicOpts.IsEnabled = false;
                        }
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
                ShowException(ex.Message);
            }
        }

        private void LoadScoreSheetData(BasicScoreSheetInfo scoreSheetInfo)
        {
            try
            {
                if (scoreSheetInfo != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3101Codes.GetScoreSheetInfo;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(scoreSheetInfo.ID.ToString());
                    Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                    if (scoreSheet == null)
                    {
                        ShowException(string.Format("ScoreSheet is null"));
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
                        //var panel = GetPanleByContentID("PanelObject");
                        //if (!panel.IsVisible)
                        //{
                        //    panel.Show();
                        //}
                    }));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStatisticalParamInfos()
        {
            try
            {
                mListStatisticalParamInfos.Clear();

                StatisticalParamInfo info = new StatisticalParamInfo();
                info.ID = 3110000000000000001;
                info.Name = "ServiceAttitude";
                info.Description = "ServiceAttitude";
                info.LangID = S3101Consts.SP_LANG_SERVICEATTITUDE;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000002;
                info.Name = "ProfessionalLevel";
                info.Description = "ProfessionalLevel";
                info.LangID = S3101Consts.SP_LANG_PROFESSIONALLEVEL;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000003;
                info.Name = "RepeatedCallin";
                info.Description = "RepeatedCallin";
                info.LangID = S3101Consts.SP_LANG_REPEATEDCALLIN;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000004;
                info.Name = "CallPeak";
                info.Description = "CallPeak";
                info.LangID = S3101Consts.SP_LANG_CALLPEAK;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000005;
                info.Name = "ACSpeExceptProportion";
                info.Description = "ACSpeExceptProportion";
                info.LangID = S3101Consts.SP_LANG_ACSPEEXCEPTPROPORTION;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000006;
                info.Name = "RecordDurationExcept";
                info.Description = "RecordDurationExcept";
                info.LangID = S3101Consts.SP_LANG_RECORDDURATIONEXCEPT;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000007;
                info.Name = "AfterDealDurationExcept";
                info.Description = "AfterDealDurationExcept";
                info.LangID = S3101Consts.SP_LANG_AFTERDEALDURATIONEXCEPT;
                mListStatisticalParamInfos.Add(info);

                info = new StatisticalParamInfo();
                info.ID = 3110000000000000008;
                info.Name = "ExceptionScore";
                info.Description = "ExceptionScore";
                info.LangID = S3101Consts.SP_LANG_EXCEPTIONSCORE;
                mListStatisticalParamInfos.Add(info);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStatisticalItems()
        {
            try
            {
                mRootStatisticalObject.Children.Clear();
                ObjectItem rootItem = new ObjectItem();
                rootItem.ObjType = 0;
                rootItem.Display = CurrentApp.GetLanguageInfo("31011200", "Auto Score Item");
                rootItem.ToolTip = rootItem.Display;
                rootItem.Icon = "Images/TemplateItem.ico";
                rootItem.IsExpanded = true;
                AddChildItem(mRootStatisticalObject, rootItem);

                for (int i = 0; i < mListStatisticalInfos.Count; i++)
                {
                    var info = mListStatisticalInfos[i];

                    CreateStatisticalItem(rootItem, info, false, false);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandler

        private void TvObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                CreateNewButtons();
                var obj = TvObjects.SelectedItem;
                ObjectItem item = obj as ObjectItem;
                if (item == null) { return; }
                CheckScoreValid(false);
                ImageObject.DataContext = item;
                TxtTitle.DataContext = item;
                ScoreObject scoreObject = item.Data as ScoreObject;
                if (scoreObject != null)
                {
                    mCurrentScoreObject = scoreObject;
                    ShowScoreViewer(scoreObject);
                    BindPropertyGrid(scoreObject);
                    BindChildPropertyGrid(null);
                }
                if (ViewHead != null && ViewHead.Visibility != Visibility.Visible)
                {
                    ViewHead.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TvStatisticalObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                CreateNewButtons();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as ToolButtonItem;
                if (optItem == null) { return; }
                string strName = optItem.Name;
                if (strName.Length < 1) { return; }
                string strBtnType = strName.Substring(0, 1);
                if (strBtnType == "M")
                {
                    string strType = strName.Substring(1);
                    int intType;
                    if (!int.TryParse(strType, out intType)) { return; }
                    switch (intType)
                    {
                        case (int)ScoreObjectType.ScoreGroup:
                            NewScoreGroup();
                            break;
                        case (int)ScoreObjectType.NumericStandard:
                            NewNumericStandard();
                            break;
                        case (int)ScoreObjectType.YesNoStandard:
                            NewYesNoStandard();
                            break;
                        case (int)ScoreObjectType.SliderStandard:
                            NewSliderStandard();
                            break;
                        case (int)ScoreObjectType.ItemStandard:
                            NewItemStandard();
                            break;
                        case (int)ScoreObjectType.TextComment:
                            NewTextComment();
                            break;
                        case (int)ScoreObjectType.ItemComment:
                            NewItemComment();
                            break;
                    }
                }
                if (strBtnType == "B")
                {
                    string strOpt = strName.Substring(1);
                    long optID;
                    if (long.TryParse(strOpt, out optID))
                    {
                        switch (optID)
                        {
                            case S3101Consts.OPT_DELETESCOREOBJECT:
                                DeleteScoreObject();
                                break;
                            case S3101Consts.OPT_SAVESCORESHEET:
                                SaveScoreSheet();
                                break;
                            case S3101Consts.OPT_SCORECACULATE:
                                ScoreCaculate();
                                break;
                            case S3101Consts.OPT_CHECKVALID:
                                CheckScoreValid(true);
                                break;
                            case S3101Consts.OPT_NEWAUTOSTANDARD:
                                NewAutoStandard();
                                break;
                        }
                    }
                }
                if (strBtnType == "N")
                {
                    string strPos = strName.Substring(1);
                    int pos;
                    if (int.TryParse(strPos, out pos))
                    {
                        switch (pos)
                        {
                            case S3101Consts.LNK_SSM:
                                NavigateSSM();
                                break;
                        }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //if (mRootObject.Children.Count <= 0) { return; }
                //ObjectItem item = mRootObject.Children[0];
                //CheckObjectItemValid(item);
                //var scoreSheet = item.Data as ScoreSheet;
                //if (scoreSheet == null) { return; }
                //string invalidMessage;
                //CheckValidResult checkResult = scoreSheet.CheckValid();
                //item.InvalidCode = checkResult.Code;
                //if (checkResult.Code != 0)
                //{
                //    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                //    item.InvalidMessage = invalidMessage;
                //    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N003", "Confirm to save ScoreSheet?")),
                //          "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                //    if (confirmResult != MessageBoxResult.Yes) { return; }
                //}
                //SaveScoreSheetData(scoreSheet);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //var selectedItem = TvObjects.SelectedItem;
            //if (selectedItem == null) { return; }
            //var selectedObj = selectedItem as ObjectItem;
            //if (selectedObj == null) { return; }
            //var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", GetLanguage("Designer", "N006", "Confirm Delete?"), selectedObj.Display), "UMP Score Designer", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //    ObjectItem parent = selectedObj.Parent;
            //    if (parent != null)
            //    {
            //        ScoreGroup scoreGroup = parent.Data as ScoreGroup;
            //        ScoreItem scoreItem = selectedObj.Data as ScoreItem;
            //        if (scoreGroup != null && scoreItem != null)
            //        {
            //            scoreGroup.Items.Remove(scoreItem);
            //        }
            //        scoreItem = parent.Data as ScoreItem;
            //        Comment comment = selectedObj.Data as Comment;
            //        if (scoreItem != null && comment != null)
            //        {
            //            scoreItem.Comments.Remove(comment);
            //        }
            //        ObjectItem_PropertyChanged(selectedObj, scoreItem);
            //        mIsChanged = true;
            //        parent.RemoveChild(selectedObj);
            //        if (selectedObj.ObjType == (int)ScoreObjectType.ScoreSheet)
            //        {
            //            mIsChanged = false;
            //        }
            //        BorderViewer.Child = null;
            //        TxtTitle.DataContext = null;
            //        ImageObject.DataContext = null;
            //        BindPropertyGrid(null, ScoreItemPropertyGrid);
            //    }
            //}
        }

        void BtnCaculate_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //var selectObj = TvObjects.SelectedItem as ObjectItem;
                //if (selectObj != null)
                //{
                //    string invalidMessage;
                //    CheckValidResult result;
                //    ScoreSheet scoreSheet = selectObj.Data as ScoreSheet;
                //    if (scoreSheet != null)
                //    {
                //        result = scoreSheet.CheckValid();
                //        selectObj.InvalidCode = result.Code;
                //        if (result.Code != 0)
                //        {
                //            scoreSheet.SetFlag(0);
                //            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
                //            selectObj.InvalidMessage = invalidMessage;
                //            ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                //            return;
                //        }
                //        scoreSheet.ResetFlag(0);
                //        double score = scoreSheet.CaculateScore();
                //        ShowInformation(string.Format("{0}\t{1}", GetLanguage("Designer", "N_002", "Score:"), score.ToString("0.00")));
                //        return;
                //    }
                //    ScoreGroup scoreGroup = selectObj.Data as ScoreGroup;
                //    if (scoreGroup != null)
                //    {
                //        result = scoreGroup.CheckValid();
                //        selectObj.InvalidCode = result.Code;
                //        if (result.Code != 0)
                //        {
                //            invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", result.Code), result.Message), result.ScoreObject);
                //            selectObj.InvalidMessage = invalidMessage;
                //            ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                //            return;
                //        }
                //        double score = scoreGroup.CaculateScore();
                //        ShowInformation(string.Format("{0}\t{1}", GetLanguage("Designer", "N002", "Score:"), score.ToString("0.00")));
                //    }
                //}
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
            }
        }

        void PanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleBtn = e.Source as ToggleButton;
            if (toggleBtn != null)
            {
                ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                if (item != null)
                {
                    PanelItem panelItem = mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem == null) { return; }
                    panelItem.IsVisible = toggleBtn.IsChecked == true;
                }
                SetPanelVisible();
            }
        }

        private void PanelDocument_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                var panel = sender as LayoutAnchorable;
                if (panel != null)
                {
                    panel.Hide();
                }
            }
            catch { }
        }

        void ScorePropertyLister_PropertyListerEvent(object sender, RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs> e)
        {
            try
            {
                var args = e.NewValue;
                if (args == null) { return; }
                var scoreObject = args.ScoreObject;
                if (scoreObject == null) { return; }
                var lister = sender as UCScorePropertyLister;
                ScoreItem scoreItem;
                LayoutAnchorable panel;
                switch (args.Code)
                {
                    case PropertyListerEventEventArgs.CODE_PRO_ITEM_CHANGED:
                        var a = args.Data as PropertyItemChangedEventArgs;
                        if (a == null) { return; }
                        var item = a.PropertyItem;


                        #region Style

                        if (item.PropertyID == ScoreProperty.PRO_TITLESTYLE
                                                   || item.PropertyID == ScoreProperty.PRO_PANELSTYLE)
                        {
                            scoreItem = scoreObject as ScoreItem;
                            if (scoreItem == null) { return; }
                            VisualStyle style;
                            if (item.PropertyID == ScoreProperty.PRO_TITLESTYLE)
                            {
                                style = scoreItem.TitleStyle;
                            }
                            else
                            {
                                style = scoreItem.PanelStyle;
                            }
                            if (style == null)
                            {
                                long id = GetSerialID();
                                if (id < 0) { return; }
                                style = new VisualStyle();
                                style.ID = id;
                                style.Type = ScoreObjectType.VisualStyle;
                                style.ScoreObject = scoreItem;
                                style.ScoreSheet = scoreItem.ScoreSheet;
                                style.ForeColor = Colors.Black;
                                if (item.PropertyID == ScoreProperty.PRO_TITLESTYLE)
                                {
                                    scoreItem.TitleStyle = style;
                                }
                                else
                                {
                                    scoreItem.PanelStyle = style;
                                }
                            }
                            BindChildPropertyGrid(style);
                            panel =
                                PanelManager.Layout.Descendents()
                                    .OfType<LayoutAnchorable>()
                                    .FirstOrDefault(p => p.ContentId == S3101Consts.PANEL_CONTENTID_CHILDPROPERTY);
                            if (panel != null)
                            {
                                panel.Show();
                                panel.IsVisible = true;
                                panel.IsActive = true;
                            }
                        }

                        #endregion


                        #region ChildItem

                        if (item.PropertyID == ScoreProperty.PRO_CONTROLITEMS
                            || item.PropertyID == ScoreProperty.PRO_I_VALUEITEMS
                            || item.PropertyID == ScoreProperty.PRO_C_VALUEITEMS)
                        {
                            BindChildLister(scoreObject);
                            panel =
                               PanelManager.Layout.Descendents()
                                   .OfType<LayoutAnchorable>()
                                   .FirstOrDefault(p => p.ContentId == S3101Consts.PANEL_CONTENTID_CHILDLISTER);
                            if (panel != null)
                            {
                                panel.Show();
                                panel.IsVisible = true;
                                panel.IsActive = true;
                            }
                        }

                        #endregion


                        break;
                    case PropertyListerEventEventArgs.CODE_PRO_VALUE_CHANGED:
                        var b = args.Data as PropertyValueChangedEventArgs;
                        if (b == null) { return; }
                        var pro = b.PropertyItem;
                        if (pro == null) { return; }
                        var objItem = TvObjects.SelectedItem as ObjectItem;


                        #region 更新显示名

                        if (objItem != null)
                        {
                            RefreshObjectItem(objItem);
                        }

                        #endregion


                        #region 检查完整性

                        scoreItem = scoreObject as ScoreItem;
                        if (scoreItem != null)
                        {
                            if (objItem != null)
                            {
                                //检查完整性
                                CheckScoreValid(objItem, false);
                            }
                        }

                        #endregion


                        #region ViewClassic

                        if (pro.PropertyID == ScoreProperty.PRO_VIEWCLASS)
                        {
                            var scoreSheet = scoreObject as ScoreSheet;
                            if (scoreSheet == null) { return; }
                            if (scoreSheet == mCurrentScoreSheet)
                            {
                                //刷新视图
                                ShowScoreViewer(scoreObject);
                            }
                        }

                        #endregion


                        #region 总分,TotalScore

                        if (pro.PropertyID == ScoreProperty.PRO_TOTALSCORE)
                        {
                            //自动设置最大值
                            var numeric = scoreObject as NumericStandard;
                            if (numeric != null)
                            {
                                numeric.MaxValue = numeric.TotalScore;
                                //刷新编辑框
                                if (lister != null)
                                {
                                    lister.RefershProperty(ScoreProperty.PRO_N_MAXVALUE);
                                }
                            }
                            var slider = scoreObject as SliderStandard;
                            if (slider != null)
                            {
                                slider.MaxValue = slider.TotalScore;
                                //刷新编辑框
                                if (lister != null)
                                {
                                    lister.RefershProperty(ScoreProperty.PRO_S_MAXVALUE);
                                }
                            }
                        }

                        #endregion


                        #region 评分标准类型

                        //if (pro.PropertyID == ScoreProperty.PRO_STANDARDTYPE)
                        //{
                        //    var standard = scoreObject as Standard;
                        //    if (standard != null)
                        //    {
                        //        var newType = standard.StandardType;
                        //        ChangeStandardType(standard, newType);
                        //        mIsChanged = true;
                        //    }
                        //}

                        #endregion


                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ChildPropertyLister_PropertyListEvent(object sender, RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs> e)
        {
            try
            {
                var args = e.NewValue;
                switch (args.Code)
                {
                    case PropertyListerEventEventArgs.CODE_PRO_ITEM_CHANGED:

                        break;
                    case PropertyListerEventEventArgs.CODE_PRO_VALUE_CHANGED:
                        var a = args.Data as PropertyValueChangedEventArgs;
                        if (a != null)
                        {

                            #region 样式改变，重新加载预览界面

                            var style = a.ScoreObject as VisualStyle;
                            if (style != null)
                            {
                                var obj = style.ScoreObject;
                                if (obj == mCurrentScoreObject)
                                {
                                    ShowScoreViewer(obj);
                                }
                            }

                            #endregion


                            #region 子项名称改变

                            var pro = a.ScoreProperty;
                            if (pro.ID == ScoreProperty.PRO_SI_DISPLAY
                                || pro.ID == ScoreProperty.PRO_CI_TEXT
                                || pro.ID == ScoreProperty.PRO_CTL_TITLE)
                            {
                                var childLister = BorderChildLister.Child as UCPropertyChildList;
                                if (childLister != null)
                                {
                                    childLister.RefreshItemDisplay();
                                }
                            }

                            #endregion

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PropertyChildLister_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<ScoreChildInfoItem> e)
        {
            try
            {
                var child = e.NewValue;
                if (child == null) { return; }
                var scoreObject = child.Data;
                if (scoreObject == null) { return; }
                BindChildPropertyGrid(scoreObject);
                var panel =
                    PanelManager.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(p => p.ContentId == S3101Consts.PANEL_CONTENTID_CHILDPROPERTY);
                if (panel != null)
                {
                    panel.Show();
                    panel.IsVisible = true;
                    panel.IsActive = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ExecuteChildListCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter as ChildListCommandArgs;
            if (param == null) { return; }
            string name = param.Name;
            var propertyChildList = param.Data as UCPropertyChildList;
            if (propertyChildList == null) { return; }
            var scoreObject = propertyChildList.ScoreObject;
            if (scoreObject == null) { return; }
            VoiceCyber.UMP.ScoreSheets.PropertyChangedEventArgs args = new VoiceCyber.UMP.ScoreSheets.PropertyChangedEventArgs();
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
                        var selectedItem = propertyChildList.GetSelectedItem();
                        if (selectedItem != null)
                        {
                            var standardItem = selectedItem.Data as StandardItem;
                            if (standardItem != null)
                            {
                                itemStandard.ValueItems.Remove(standardItem);
                            }
                        }
                        propertyChildList.InitChildItems();
                        mIsChanged = true;
                    }
                    itemComment = scoreObject as ItemComment;
                    if (itemComment != null)
                    {
                        args.PropertyName = "ValueItems";
                        var selectedItem = propertyChildList.GetSelectedItem();
                        if (selectedItem != null)
                        {
                            var commentItem = selectedItem.Data as CommentItem;
                            if (commentItem != null)
                            {
                                itemComment.ValueItems.Remove(commentItem);
                            }
                        }
                        propertyChildList.InitChildItems();
                        mIsChanged = true;
                    }
                    scoreSheet = scoreObject as ScoreSheet;
                    if (scoreSheet != null)
                    {
                        args.PropertyName = "ControlItems";
                        var selectedItem = propertyChildList.GetSelectedItem();
                        if (selectedItem != null)
                        {
                            var controlItem = selectedItem.Data as ControlItem;
                            if (controlItem != null)
                            {
                                scoreSheet.ControlItems.Remove(controlItem);
                            }
                        }
                        propertyChildList.InitChildItems();
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
                    propertyChildList.MoveScoreObject(name);
                    mIsChanged = true;
                    break;
            }
            scoreObject.PropertyChanged(this, args);
        }

        #endregion


        #region Operations

        private void SaveLayout()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var serializer = new XmlLayoutSerializer(PanelManager);
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
                    ShowException(string.Format("Fail.\tLayoutInfo is null"));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSSaveLayoutInfo;
                webRequest.ListData.Add("310101");
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(strLayoutInfo);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                ShowInformation(CurrentApp.GetMessageLanguageInfo("003", "Save layout end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ResetLayout()
        {
            try
            {
                LoadLayout();
                InitLayout();
                SetPanelVisible();
                SetViewStatus();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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

        private void DeleteScoreObject()
        {
            try
            {
                var selectedItem = TvObjects.SelectedItem;
                if (selectedItem == null) { return; }
                var selectedObj = selectedItem as ObjectItem;
                if (selectedObj == null) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", GetLanguage("Designer", "N006", "Confirm Delete?"), selectedObj.Display), "UMP Score Designer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ObjectItem parent = selectedObj.Parent;
                    if (parent != null && parent != mRootObject)
                    {
                        //根评分表对象不能被删除
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
                        mIsChanged = true;
                        parent.RemoveChild(selectedObj);
                        BorderViewer.Child = null;
                        TxtTitle.DataContext = null;
                        ImageObject.DataContext = null;
                        parent.IsSelected = true;
                    }
                    if (parent != null && parent == mRootObject)
                    {
                        ShowInformation(string.Format(GetLanguage("Designer", "N016", "The root object can not be deleted.")));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveScoreSheet()
        {
            try
            {
                var scoreSheet = mCurrentScoreSheet;
                if (scoreSheet == null) { return; }
                if (string.IsNullOrWhiteSpace(scoreSheet.Title)) { ShowInformation(string.Format(GetLanguage("Designer", "N017", "Title cannot be blank."))); return; }
                string invalidMessage;
                CheckValidResult checkResult = scoreSheet.CheckValid();
                if (checkResult.Code != 0)
                {
                    scoreSheet.SetFlag(0);
                    invalidMessage = string.Format("{0}\t{1}", GetLanguage("ObjectViewer", string.Format("NI{0}", checkResult.Code), checkResult.Message), checkResult.ScoreObject);
                    var confirmResult = MessageBox.Show(string.Format("{0}\t{1}\r\n\r\n{2}", GetLanguage("Designer", "N001", "Check ScoreSheet valid fail."), invalidMessage, GetLanguage("Designer", "N003", "Confirm to save ScoreSheet?")),
                          "UMPScoreSheetDesigner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirmResult != MessageBoxResult.Yes) { return; }
                }
                else
                {
                    scoreSheet.ResetFlag(0);
                }
                SaveScoreSheetData(scoreSheet);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveScoreSheetData(ScoreSheet scoreSheet)
        {
            if (scoreSheet == null) { return; }
            SetBusy(true, string.Empty);
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                try
                {
                    foreach (Comment comItem in scoreSheet.Comments)
                    {
                        if (string.IsNullOrWhiteSpace(comItem.Display)) { ShowInformation(string.Format(GetLanguage("Designer", "N017", "Title cannot be blank."))); return; }//评分表的备注不允许为空
                    }
                    bool flag = true;
                    flag = CheckTitleIsEmpty(scoreSheet.Items);
                    if (!flag) { ShowInformation(string.Format(GetLanguage("Designer", "N017", "Title cannot be blank."))); return; }
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(scoreSheet);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3101Codes.SaveScoreSheetInfo;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    CurrentApp.WriteLog("SaveScoreSheet", webReturn.Data);

                    #region 写操作日志

                    string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001Name"),
                        scoreSheet.Title);
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001TotalScore"),
                        scoreSheet.TotalScore);
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ViewClassic"),
                     Utils.FormatOptLogString(string.Format("3101Tip002{0}", (int)scoreSheet.ViewClassic)));
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ScoreType"),
                     Utils.FormatOptLogString(string.Format("3101Tip003{0}", (int)scoreSheet.ScoreType)));
                    CurrentApp.WriteOperationLog(S3101App.IsModifyScoreSheet ?
                        S3101Consts.OPT_MODIFYSCORESHEET.ToString() : S3101Consts.OPT_CREATESCORESHEET.ToString(),
                        ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion

                    ShowInformation(CurrentApp.GetMessageLanguageInfo("002", "Save ScoreSheet end"));
                    mIsChanged = false;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                SetBusy(false, string.Empty);
            };
            mWorker.RunWorkerAsync();
        }

        //评分标准/评分类别标题不能为空
        private bool CheckTitleIsEmpty(List<ScoreItem> items)
        {
            bool flag = true;
            try
            {
                foreach (ScoreItem item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.Display)) { return false; }
                    ScoreGroup sgItems = item as ScoreGroup;
                    if (sgItems!=null && sgItems.Items.Count() > 0)
                    {
                        flag = CheckTitleIsEmpty(sgItems.Items);
                        if (!flag) { return false; }
                    }
                    if (item.Comments.Count() > 0) 
                    { 
                        flag=CheckTitleIsEmpty(item.Comments);
                        if (!flag) { return false; }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        //评分标准中的备注不允许为空
        private bool CheckTitleIsEmpty(List<Comment> items)
        {
            try
            {
                foreach (Comment comItem in items)
                {
                    if (string.IsNullOrWhiteSpace(comItem.Display)) { return false; }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private void ScoreCaculate()
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
                            ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        scoreSheet.ResetFlag(0);
                        double score = scoreSheet.CaculateScore();
                        ShowInformation(string.Format("{0}\t{1}", GetLanguage("Designer", "N_002", "Score:"), score.ToString("0.00")));
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
                            ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            return;
                        }
                        double score = scoreGroup.CaculateScore();
                        ShowInformation(string.Format("{0}\t{1}", GetLanguage("Designer", "N002", "Score:"), score.ToString("0.00")));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CheckScoreValid(bool isShowMsgBox)
        {
            var selectObj = TvObjects.SelectedItem as ObjectItem;
            if (selectObj != null)
            {
                CheckScoreValid(selectObj, isShowMsgBox);
            }
        }

        private void CheckScoreValid(ObjectItem item, bool isShowMsgBox)
        {
            try
            {
                var selectObj = item;
                if (selectObj != null)
                {
                    ResetItemInvalid(selectObj);
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
                            if (isShowMsgBox)
                            {
                                ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            }
                            return;
                        }
                        if (isShowMsgBox)
                        {
                            ShowInformation(GetLanguage("Designer", "N015", "ScoreSheet valid!"));
                        }
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
                            if (isShowMsgBox)
                            {
                                ShowException(string.Format("{0}\t{1}", GetLanguage("Designer", "N001", "Check valid fail."), invalidMessage));
                            }
                            return;
                        }
                        if (isShowMsgBox)
                        {
                            ShowInformation(GetLanguage("Designer", "N015", "ScoreSheet valid!"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void NavigateSSM()
        {
            try
            {
                //if (NavigationService != null)
                //    NavigationService.Navigate(new Uri("SSMMainPage.xaml", UriKind.Relative));

                try
                {
                    SSMMainView view = new SSMMainView();
                    view.CurrentApp = CurrentApp;
                    view.PageName = "SSMMainPage";
                    CurrentApp.CurrentView = view;
                    if (CurrentApp.RunAsModule)
                    {
                        CurrentApp.InitCurrentView();
                    }
                    else
                    {
                        var app = App.Current;
                        if (app != null)
                        {
                            var window = app.MainWindow;
                            if (window != null)
                            {
                                var shell = window.Content as Shell;
                                if (shell != null)
                                {
                                    shell.SetView(view);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/template.ico";
                item.Data = scoreSheet;

                CreateChildItem(item, scoreSheet);
                CreateChildComment(item, scoreSheet);
                //CreateChildControlItem(item, scoreSheet);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/TemplateItem.ico";
                item.Data = scoreGroup;

                CreateChildItem(item, scoreGroup);
                CreateChildComment(item, scoreGroup);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/standard.ico";
                item.Data = numericStandard;

                CreateChildComment(item, numericStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/standard.ico";
                item.Data = yesNoStandard;

                CreateChildComment(item, yesNoStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/standard.ico";
                item.Data = itemStandard;

                //CreateChildStandardItem(item, itemStandard);
                CreateChildComment(item, itemStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/standard.ico";
                item.Data = sliderStandard;

                CreateChildComment(item, sliderStandard);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/comment.ico";
                item.Data = textComment;

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                item.Icon = "Images/comment.ico";
                item.Data = itemComment;

                //CreateChildCommentItem(item, itemComment);

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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

        private void CreateStatisticalItem(ObjectItem parent, StatisticalInfo statisticalInfo, bool isExpanded,
            bool isSelected)
        {
            try
            {
                if (statisticalInfo == null) { return; }

                string strName;
                long paramid = statisticalInfo.ParamID;
                var param = mListStatisticalParamInfos.FirstOrDefault(p => p.ID == paramid);
                if (param != null)
                {
                    string paramName = CurrentApp.GetLanguageInfo(param.LangID, param.Name);
                    strName = string.Format("{0}[{1}]", paramName, statisticalInfo.OwnerName);
                }
                else
                {
                    strName = statisticalInfo.Name;
                }

                ObjectItem item = new ObjectItem();
                item.ObjType = S3101Consts.RESOURCE_STATISTICALINFO;
                item.Display = strName;
                item.ToolTip = strName;
                item.Icon = "Images/standard.ico";
                item.Data = statisticalInfo;

                AddChildItem(parent, item, isExpanded, isSelected);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                scoreSheet.Creator = CurrentApp.Session.UserInfo.UserID;
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
                    //Title = string.Format("{0} - {1}", mCurrentScoreSheet.Title, GetLanguage("Designer", "002", "Score Sheet Designer"));
                    TvObjects.Focus();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                ShowException(ex.Message);
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
                //mListItemStandardItems.Add(standardItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
            }
        }

        private void NewAutoStandard()
        {
            try
            {
                //自动评分标准是一种YesNoStandard
                var staItem = TvStatisticalObjects.SelectedItem as ObjectItem;
                if (staItem == null) { return; }
                var staInfo = staItem.Data as StatisticalInfo;
                if (staInfo == null) { return; }
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
                yesNoStandard.Title = staItem.Display;
                yesNoStandard.ViewClassic = scoreSheet.ViewClassic;
                yesNoStandard.ScoreType = scoreSheet.ScoreType;
                yesNoStandard.Parent = scoreGroup;
                yesNoStandard.ScoreSheet = scoreSheet;
                yesNoStandard.UsePointSystem = scoreSheet.UsePointSystem;
                yesNoStandard.IsAllowNA = true;
                yesNoStandard.StandardType = StandardType.YesNo;
                yesNoStandard.ScoreClassic = StandardClassic.YesNo;
                yesNoStandard.DefaultValue = false;

                yesNoStandard.IsAutoStandard = true;
                yesNoStandard.StatisticalID = staInfo.ID;
                scoreGroup.Items.Add(yesNoStandard);
                scoreSheet.HasAutoStandard = true;

                selectedItem.IsExpanded = true;
                CreateYesNoStandardItem(selectedItem, yesNoStandard, true, true);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateToolBarButtons()
        {
            try
            {
                PanelToolButton.Children.Clear();
                ToolButtonItem toolItem;
                ToggleButton toggleBtn;
                Button btn;
                for (int i = 0; i < mListPanels.Count; i++)
                {
                    PanelItem item = mListPanels[i];
                    if (!item.CanClose) { continue; }
                    toolItem = new ToolButtonItem();
                    toolItem.Name = "TB" + item.Name;
                    toolItem.Display = item.Title;
                    toolItem.Tip = item.Title;
                    toolItem.Icon = item.Icon;
                    toggleBtn = new ToggleButton();
                    toggleBtn.Click += PanelToggleButton_Click;
                    toggleBtn.DataContext = toolItem;
                    toggleBtn.IsChecked = item.IsVisible;
                    toggleBtn.SetResourceReference(StyleProperty, "ToolBarToggleBtnStyle");
                    PanelToolButton.Children.Add(toggleBtn);
                }

                toolItem = new ToolButtonItem();
                toolItem.Name = S3101Consts.OPT_NAME_SAVELAYOUT;
                toolItem.Display = GetLanguage("ToolBar", "B010", "Save Layout");
                toolItem.Tip = GetLanguage("ToolBar", "B010", "Save Layout");
                toolItem.Icon = "Images/savelayout.png";
                btn = new Button();
                btn.Command = SSDPageCommands.SaveLayoutCommand;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                PanelToolButton.Children.Add(btn);

                toolItem = new ToolButtonItem();
                toolItem.Name = S3101Consts.OPT_NAME_RESETLAYOUT;
                toolItem.Display = GetLanguage("ToolBar", "B011", "Save Layout");
                toolItem.Tip = GetLanguage("ToolBar", "B011", "Save Layout");
                toolItem.Icon = "Images/reset.png";
                btn = new Button();
                btn.Command = SSDPageCommands.ResetLayoutCommand;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                PanelToolButton.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetPanelVisible()
        {
            try
            {
                for (int i = 0; i < mListPanels.Count; i++)
                {
                    var item = mListPanels[i];
                    var panel =
                           PanelManager.Layout.Descendents()
                               .OfType<LayoutAnchorable>()
                               .FirstOrDefault(p => p.ContentId == item.ContentID);
                    if (panel != null)
                    {
                        panel.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", item.PanelID.ToString("000")),
                            item.Name);

                        if (item.IsVisible)
                        {
                            panel.Show();
                        }
                        else
                        {
                            panel.Hide();
                        }
                        LayoutAnchorable panel1 = panel;
                        panel.IsVisibleChanged += (s, e) =>
                        {
                            item.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                        panel.Closing += PanelDocument_Closing;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetViewStatus()
        {
            for (int i = 0; i < PanelToolButton.Children.Count; i++)
            {
                var toggleBtn = PanelToolButton.Children[i] as ToggleButton;
                if (toggleBtn != null)
                {
                    ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                    if (item == null) { continue; }
                    PanelItem panelItem = mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem != null)
                    {
                        toggleBtn.IsChecked = panelItem.IsVisible;
                    }
                }
            }
        }

        public string GetLanguage(string category, string code, string display)
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

        private long GetID()
        {
            //mObjectID++;
            //return Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss") + mObjectID.ToString("0000"));
            long id = GetSerialID();
            if (id < 0)
            {
                ShowException(string.Format("SerialID invalid"));
            }
            return id;
        }

        private long GetSerialID()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("301");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return -1;
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
                ShowException(ex.Message);
            }
        }

        private void CreateNewButtons()
        {
            try
            {
                PanelCreateObject.Children.Clear();
                var selectedItem = TvObjects.SelectedItem as ObjectItem;
                if (selectedItem == null) { return; }
                var scoreObject = selectedItem.Data as ScoreObject;
                if (scoreObject == null) { return; }
                ToolButtonItem item;
                Button btn;
                int intType;

                //ScoreGroup
                if (scoreObject.Type == ScoreObjectType.ScoreGroup
                    || scoreObject.Type == ScoreObjectType.ScoreSheet)
                {
                    intType = (int)ScoreObjectType.ScoreGroup;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                        "ScoreGroup");
                    item.Tip = item.Name;
                    item.Icon = "Images/template.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //Standard
                //NumericStandard
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup)
                {
                    intType = (int)ScoreObjectType.NumericStandard;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                        "NumericStandard");
                    item.Tip = item.Name;
                    item.Icon = "Images/standard.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //YesNoStandard
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup)
                {
                    intType = (int)ScoreObjectType.YesNoStandard;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                       "YesNoStandard");
                    item.Tip = item.Name;
                    item.Icon = "Images/standard.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //SliderStandard
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup)
                {
                    intType = (int)ScoreObjectType.SliderStandard;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                      "SliderStandard");
                    item.Tip = item.Name;
                    item.Icon = "Images/standard.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //ItemStandard
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup)
                {
                    intType = (int)ScoreObjectType.ItemStandard;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                      "ItemStandard");
                    item.Tip = item.Name;
                    item.Icon = "Images/standard.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //AutoStandard
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup)
                {
                    var sitem = TvStatisticalObjects.SelectedItem as ObjectItem;
                    if (sitem != null
                        && sitem.ObjType == S3101Consts.RESOURCE_STATISTICALINFO)
                    {
                        item = new ToolButtonItem();
                        item.Name = string.Format("B{0}", S3101Consts.OPT_NEWAUTOSTANDARD);
                        item.Display = CurrentApp.GetLanguageInfo("31011201", "Auto Standard");
                        item.Tip = item.Name;
                        item.Icon = "Images/standard.ico";
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = item;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelCreateObject.Children.Add(btn);
                    }
                }
                //TextComment
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup
                    || scoreObject.Type == ScoreObjectType.Standard
                    || scoreObject.Type == ScoreObjectType.NumericStandard
                    || scoreObject.Type == ScoreObjectType.YesNoStandard
                    || scoreObject.Type == ScoreObjectType.SliderStandard
                    || scoreObject.Type == ScoreObjectType.ItemStandard)
                {
                    intType = (int)ScoreObjectType.TextComment;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                     "TextComment");
                    item.Tip = item.Name;
                    item.Icon = "Images/comment.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
                //ItemComment
                if (scoreObject.Type == ScoreObjectType.ScoreSheet
                    || scoreObject.Type == ScoreObjectType.ScoreGroup
                    || scoreObject.Type == ScoreObjectType.Standard
                    || scoreObject.Type == ScoreObjectType.NumericStandard
                    || scoreObject.Type == ScoreObjectType.YesNoStandard
                    || scoreObject.Type == ScoreObjectType.SliderStandard
                    || scoreObject.Type == ScoreObjectType.ItemStandard)
                {
                    intType = (int)ScoreObjectType.ItemComment;
                    item = new ToolButtonItem();
                    item.Name = string.Format("M{0}", intType.ToString("000"));
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("OBJ301{0}", intType.ToString("000")),
                     "ItemComment");
                    item.Tip = item.Name;
                    item.Icon = "Images/comment.ico";
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelCreateObject.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateBasicButtons()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                ToolButtonItem item;
                Button btn;

                //Delete ScoreObject
                item = new ToolButtonItem();
                item.Name = string.Format("B{0}", S3101Consts.OPT_DELETESCOREOBJECT);
                item.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3101Consts.OPT_DELETESCOREOBJECT),
                    "Delete ScoreObject");
                item.Tip = item.Display;
                item.Icon = "Images/delete.ico";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);

                //Save ScoreSheet
                item = new ToolButtonItem();
                item.Name = string.Format("B{0}", S3101Consts.OPT_SAVESCORESHEET);
                item.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3101Consts.OPT_SAVESCORESHEET),
                    "Save ScoreSheet");
                item.Tip = item.Display;
                item.Icon = "Images/save.ico";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);

                //Caculate
                item = new ToolButtonItem();
                item.Name = string.Format("B{0}", S3101Consts.OPT_SCORECACULATE);
                item.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3101Consts.OPT_SCORECACULATE),
                    "Score Caculate");
                item.Tip = item.Display;
                item.Icon = "Images/caculate.ico";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);

                //Check Valid
                item = new ToolButtonItem();
                item.Name = string.Format("B{0}", S3101Consts.OPT_CHECKVALID);
                item.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3101Consts.OPT_CHECKVALID),
                    "Check Valid");
                item.Tip = item.Display;
                item.Icon = "Images/invalid.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateOtherPosition()
        {
            try
            {
                PanelOtherPosition.Children.Clear();
                ToolButtonItem item;
                Button btn;

                //SSM
                item = new ToolButtonItem();
                item.Name = string.Format("N{0}", S3101Consts.LNK_SSM.ToString("000"));
                item.Display = CurrentApp.GetLanguageInfo("31011010", "Navigate to SSM");
                item.Tip = item.Display;
                item.Icon = "Images/back.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOtherPosition.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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

        private void ShowScoreViewer(ScoreObject scoreObject)
        {
            ScoreSheet scoreSheet;
            ScoreItemClassic viewClassic = ScoreItemClassic.Tree;
            var panel =
                    PanelManager.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(p => p.ContentId == S3101Consts.PANEL_CONTENTID_CHILDLISTER);
            if (panel != null)
            {
                panel.Hide();
            } 
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

        private void BindPropertyGrid(ScoreObject scoreObject)
        {
            try
            {
                BorderScoreProperty.Child = null;
                if (scoreObject == null) { return; }
                UCScorePropertyLister lister = new UCScorePropertyLister();
                lister.PropertyListerEvent += ScorePropertyLister_PropertyListerEvent;
                lister.CurrentApp = CurrentApp;
                lister.ScoreObject = scoreObject;
                BorderScoreProperty.Child = lister;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BindChildLister(ScoreObject scoreObject)
        {
            if (scoreObject == null)
            {
                BorderChildLister.Child = null;
            }
            else
            {
                UCPropertyChildList lister = new UCPropertyChildList();
                lister.SelectedItemChanged += PropertyChildLister_SelectedItemChanged;
                lister.CurrentApp = CurrentApp;
                lister.PageParent = this;
                lister.CurrentApp = CurrentApp;
                lister.ScoreObject = scoreObject;
                BorderChildLister.Child = lister;
            }
        }

        private void BindChildPropertyGrid(ScoreObject scoreObject)
        {
            try
            {
                BorderChildProperty.Child = null;
                if (scoreObject == null) { return; }
                UCScorePropertyLister lister = new UCScorePropertyLister();
                lister.PropertyListerEvent += ChildPropertyLister_PropertyListEvent;
                lister.CurrentApp = CurrentApp;
                lister.ScoreObject = scoreObject;
                BorderChildProperty.Child = lister;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RefreshObjectItem(ObjectItem item)
        {
            try
            {
                if (item == null) { return; }
                var scoreItem = item.Data as ScoreItem;
                if (scoreItem != null)
                {
                    item.Display = scoreItem.Title;
                    item.ToolTip = scoreItem.Title;
                }
                var comment = item.Data as Comment;
                if (comment != null)
                {
                    item.Display = comment.Title;
                    item.ToolTip = comment.Title;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                    //如果有子项，清除
                    item.Children.Clear();
                }
            }
        }

        private void ResetItemInvalid(ObjectItem parent)
        {
            if (parent == null) { return; }
            parent.InvalidCode = 0;
            parent.InvalidMessage = string.Empty;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                var child = parent.Children[i];
                if (child != null)
                {
                    ResetItemInvalid(child);
                }
            }
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


        #region ChangeTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3101;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS3101;component/Themes/Default/UMPS3101/SSDStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
            try
            {
                string uri = string.Format("/UMPS3101;component/Themes/Default/UMPS3101/SSDAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }

            var popup = PopupPanel;
            if (popup != null)
            {
                popup.ChangeTheme();
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                   "ScoreSheet Management");

                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("31011000", "Basic Operation");
                ExpCreateObject.Header = CurrentApp.GetLanguageInfo("31011002", "Create Operation");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("31011001", "Other Position");

                CreateNewButtons();
                CreateBasicButtons();
                CreateOtherPosition();

                for (int i = 0; i < mListPanels.Count; i++)
                {
                    var item = mListPanels[i];
                    var panel =
                        PanelManager.Layout.Descendents()
                            .OfType<LayoutContent>()
                            .FirstOrDefault(p => p.ContentId == item.ContentID);
                    if (panel != null)
                    {
                        panel.Title = CurrentApp.GetLanguageInfo(string.Format("3101P{0}", item.PanelID.ToString("000")),
                            item.Name);
                    }
                }

                ShowScoreViewer(mCurrentScoreObject);

                var subPanel = BorderScoreProperty.Child as UMPUserControl;
                if (subPanel != null)
                {
                    subPanel.ChangeLanguage();
                }
                subPanel = BorderChildLister.Child as UMPUserControl;
                if (subPanel != null)
                {
                    subPanel.ChangeLanguage();
                }
                subPanel = BorderChildProperty.Child as UMPUserControl;
                if (subPanel != null)
                {
                    subPanel.ChangeLanguage();
                }
                subPanel = PopupPanel;
                if (subPanel != null)
                {
                    subPanel.ChangeLanguage();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);
            var code = webRequest.Code;
            switch (code)
            {
                case (int)RequestCode.ACPageHeadLeftPanel:
                    if (GridLeft.Width.Value == 0)
                    {
                        GridLeft.Width = new GridLength(200);
                    }
                    else
                    {
                        GridLeft.Width = new GridLength(0);
                    }
                    break;
            }
        }
    }
}
