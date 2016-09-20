//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dac86791-0438-4819-9acd-c71293030f1d
//        CLR Version:              4.0.30319.42000
//        Name:                     UCAllModuleView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.WidgetViews
//        File Name:                UCAllModuleView
//
//        created by Charley at 2016/3/10 13:44:25
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using UMPS1206.Models;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1206.WidgetViews
{
    /// <summary>
    /// UCAllModuleView.xaml 的交互逻辑
    /// </summary>
    public partial class UCAllModuleView : IWidgetView
    {

        #region Members

        public WidgetItem WidgetItem { get; set; }
        public IList<BasicDataInfo> ListBasicDataInfos { get; set; }
        public bool IsCenter { get; set; }
        public bool IsFull { get; set; }

        private bool mIsInited;
        private List<BasicAppInfo> mListModuleInfos;
        private ObservableCollection<BasicModuleItem> mListModuleItems;
        private ObservableCollection<ModuleGroupItem> mListGroupItems;
        private List<DiagramNode> mListModuleNodes;
        private DiagramNode mRootNode;

        #endregion


        public UCAllModuleView()
        {
            InitializeComponent();

            mListModuleInfos = new List<BasicAppInfo>();
            mListModuleItems = new ObservableCollection<BasicModuleItem>();
            mListModuleNodes = new List<DiagramNode>();
            mListGroupItems = new ObservableCollection<ModuleGroupItem>();

            Loaded += UCAllModuleView_Loaded;
        }
       

        void UCAllModuleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                if (WidgetItem == null) { return; }
                WidgetItem.View = this;
                CommandBindings.Add(new CommandBinding(ItemClickCommand, ItemClickCommand_Executed,
                  (s, ce) => ce.CanExecute = true));

                ListBoxModuleItems.ItemsSource = mListModuleItems;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBoxModuleItems.ItemsSource);
                if (view != null)
                {
                    if (view.GroupDescriptions != null)
                    {
                        view.GroupDescriptions.Clear();
                        view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    }
                }

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadModuleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetViewMode();
                    CreateModuleItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadModuleInfos()
        {
            try
            {
                mListModuleInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetAppBasicInfoList;
                webRequest.ListData.Add(CurrentApp.Session.RoleID.ToString());
                webRequest.ListData.Add("0");
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicAppInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicAppInfo info = optReturn.Data as BasicAppInfo;
                    if (info == null) { continue; }
                    mListModuleInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadModules", string.Format("Load end.\t{0}", mListModuleInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateModuleItems()
        {
            try
            {
                mListModuleNodes.Clear();
                mListModuleItems.Clear();
                mListGroupItems.Clear();
                string strIcon = string.Format("{0}://{1}:{2}/Logo/logo.png",
                   CurrentApp.Session.AppServerInfo.Protocol,
                   CurrentApp.Session.AppServerInfo.Address,
                   CurrentApp.Session.AppServerInfo.Port);
                ModuleGroupItem rootGroup = new ModuleGroupItem();
                rootGroup.CurrentApp = CurrentApp;
                rootGroup.Name = ConstValue.UMP_PRODUCTER_SHORTNAME;
                rootGroup.Title = ConstValue.UMP_PRODUCTER_SHORTNAME;
                mListGroupItems.Add(rootGroup);
                mRootNode = new DiagramNode(rootGroup.Title, null, strIcon, rootGroup.Title);
                mRootNode.Expanded += ModuleItemNode_Expanded;
                mRootNode.DataContext = rootGroup;
                mListModuleNodes.Add(mRootNode);
                var groups = mListModuleInfos.GroupBy(m => m.Category);
                foreach (var group in groups)
                {
                    string strGroupName = group.Key;
                    string strGroupDisplay = CurrentApp.GetLanguageInfo(string.Format("{0}Content", strGroupName), strGroupName);
                    strIcon = string.Format("{0}://{1}:{2}/Themes/{3}/Images/S0002/{4}.png",
                        CurrentApp.Session.AppServerInfo.Protocol,
                        CurrentApp.Session.AppServerInfo.Address,
                        CurrentApp.Session.AppServerInfo.Port,
                        CurrentApp.Session.ThemeName,
                        strGroupName);
                    ModuleGroupItem groupItem = new ModuleGroupItem();
                    groupItem.CurrentApp = CurrentApp;
                    groupItem.Name = strGroupName;
                    groupItem.Title = strGroupDisplay;
                    mListGroupItems.Add(groupItem);
                    DiagramNode groupNode = new DiagramNode(strGroupDisplay, mRootNode, strIcon, strGroupDisplay);
                    groupNode.Expanded += ModuleItemNode_Expanded;
                    groupNode.DataContext = groupItem;
                    mListModuleNodes.Add(groupNode);
                    foreach (var app in group)
                    {
                        strIcon = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                            CurrentApp.Session.AppServerInfo.Protocol,
                            CurrentApp.Session.AppServerInfo.Address,
                            CurrentApp.Session.AppServerInfo.Port,
                            CurrentApp.Session.ThemeName,
                            app.Icon);
                        BasicModuleItem moduleItem = new BasicModuleItem();
                        moduleItem.CurrentApp = CurrentApp;
                        moduleItem.ModuleID = app.ModuleID;
                        moduleItem.Name = app.Title;
                        moduleItem.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", app.ModuleID), app.Title);
                        moduleItem.Icon = strIcon;
                        moduleItem.Tip = moduleItem.Title;
                        moduleItem.Category = CurrentApp.GetLanguageInfo(string.Format("{0}Content", strGroupName), strGroupName);
                        moduleItem.Info = app;
                        mListModuleItems.Add(moduleItem);
                        DiagramNode moduleNode = new DiagramNode(moduleItem.Title, groupNode, strIcon, moduleItem.Title);
                        moduleNode.Expanded += ModuleItemNode_Expanded;
                        moduleNode.DataContext = moduleItem;
                        mListModuleNodes.Add(moduleNode);
                    }
                }
                MyDiagramViewer.AutoExpandRoot = true;
                MyDiagramViewer.RootNode = mRootNode;
                MyScrollViewer.AutoScrollTarget = mRootNode.Location;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ModuleItemNode_Expanded(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            try
            {
                MyScrollViewer.AutoScrollTarget = sender.Location;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetViewMode()
        {
            try
            {
                if (WidgetItem == null) { return; }
                if (WidgetItem.IsFull)
                {
                    GridChart.Width = new GridLength(50, GridUnitType.Star);
                    GridDetail.Width = new GridLength(50, GridUnitType.Star);
                    SplitterDetail.IsEnabled = true;
                }
                else
                {
                    if (WidgetItem.IsCenter)
                    {
                        Height = 550;
                    }
                    else
                    {
                        Height = 650;
                    }
                    SplitterDetail.IsEnabled = false;
                    //GridChart.Width = new GridLength(100, GridUnitType.Star);
                    //GridDetail.Width = new GridLength(0);
                    GridChart.Width = new GridLength(0);
                    GridDetail.Width = new GridLength(100, GridUnitType.Star);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Refresh

        public void Refresh()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadModuleInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetViewMode();
                    CreateModuleItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ItemClickCommand

        private static RoutedUICommand mItemClickCommand = new RoutedUICommand();

        public static RoutedUICommand ItemClickCommand
        {
            get { return mItemClickCommand; }
        }

        private void ItemClickCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as BasicModuleItem;
            if (item != null)
            {
                var info = item.Info;
                if (info == null) { return; }
                int appID = info.AppID;
                string strArgs = info.Args;
                string strIcon = info.Icon;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.ACTaskNavigateApp;
                webRequest.ListData.Add(appID.ToString());
                webRequest.ListData.Add(strArgs);
                webRequest.ListData.Add(strIcon);
                CurrentApp.PublishEvent(webRequest);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListModuleItems.Count; i++)
                {
                    var item = mListModuleItems[i];
                    var info = item.Info;
                    if (info != null)
                    {
                        item.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", info.ModuleID), info.Title);
                        item.Category = CurrentApp.GetLanguageInfo(string.Format("{0}Content", info.Category), info.Category);
                    }
                }
                for (int i = 0; i < mListModuleNodes.Count; i++)
                {
                    var node = mListModuleNodes[i];
                    var groupItem = node.DataContext as ModuleGroupItem;
                    if (groupItem != null)
                    {
                        node.NodeName = CurrentApp.GetLanguageInfo(string.Format("{0}Content", groupItem.Name), groupItem.Name);
                        node.Description = node.NodeName;
                    }
                    var item = node.DataContext as BasicModuleItem;
                    if (item != null)
                    {
                        var info = item.Info;
                        if (info != null)
                        {
                            node.NodeName = CurrentApp.GetLanguageInfo(string.Format("FO{0}", info.ModuleID), info.Title);
                            node.Description = node.NodeName;
                        }
                    }
                }
            }
            catch {}
        }

        #endregion
    }
}
