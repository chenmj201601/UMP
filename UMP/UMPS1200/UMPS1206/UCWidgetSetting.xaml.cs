//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    737ab0d4-c3b7-4e2a-a052-24ff6bedd380
//        CLR Version:              4.0.30319.18408
//        Name:                     UCWidgetSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                UCWidgetSetting
//
//        created by Charley at 2016/5/5 16:53:28
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1206.Models;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1206
{
    /// <summary>
    /// UCWidgetSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCWidgetSetting
    {

        #region Members

        public DashboardMainView PageParent;

        private List<WidgetInfo> mListAllWidgetInfos;
        private List<WidgetInfo> mListMyWidgetInfos;
        private ObservableCollection<WidgetPreviewItem> mListAllWidgetItems;
        private ObservableCollection<WidgetPreviewItem> mListLeftWidgetItems;
        private ObservableCollection<WidgetPreviewItem> mListCenterWidgetItems;
        private bool mIsInited;

        #endregion


        public UCWidgetSetting()
        {
            InitializeComponent();

            mListAllWidgetInfos = new List<WidgetInfo>();
            mListMyWidgetInfos = new List<WidgetInfo>();
            mListAllWidgetItems = new ObservableCollection<WidgetPreviewItem>();
            mListLeftWidgetItems = new ObservableCollection<WidgetPreviewItem>();
            mListCenterWidgetItems = new ObservableCollection<WidgetPreviewItem>();

            Loaded += UCWidgetSetting_Loaded;
            BtnAdd.Click += BtnAdd_Click;
            BtnRemove.Click += BtnRemove_Click;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        void UCWidgetSetting_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ListBoxAvaliable.ItemsSource = mListAllWidgetItems;
                ListBoxLeft.ItemsSource = mListLeftWidgetItems;
                ListBoxCenter.ItemsSource = mListCenterWidgetItems;

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAllWidgetInfos();
                    LoadMyWidgetInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateAllWidgetItems();
                    CreateMyWidgetItems();

                    InitLang();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAllWidgetInfos()
        {
            try
            {
                mListAllWidgetInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetAllWidgetList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
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
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<WidgetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WidgetInfo info = optReturn.Data as WidgetInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("WidgetInfo is null"));
                        return;
                    }
                    mListAllWidgetInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAllWidgetInfos", string.Format("Load end.\t{0}", mListAllWidgetInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadMyWidgetInfos()
        {
            try
            {
                mListMyWidgetInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserWidgetList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
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
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<WidgetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WidgetInfo info = optReturn.Data as WidgetInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("WidgetInfo is null"));
                        return;
                    }
                    mListMyWidgetInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadMyWidgetInfos", string.Format("Load end.\t{0}", mListMyWidgetInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Createb Items

        private void CreateAllWidgetItems()
        {
            try
            {
                mListAllWidgetItems.Clear();
                for (int i = 0; i < mListAllWidgetInfos.Count; i++)
                {
                    var info = mListAllWidgetInfos[i];

                    WidgetPreviewItem item = new WidgetPreviewItem();
                    item.WidgetInfo = info;
                    item.WidgetID = info.WidgetID;
                    item.Name = info.Name;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                    item.Icon = string.Format("/UMPS1206;component/Themes/Default/UMPS1206/Images/Widgets/{0}.png", info.WidgetID);
                    mListAllWidgetItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateMyWidgetItems()
        {
            try
            {
                mListLeftWidgetItems.Clear();

                var list = mListMyWidgetInfos.Where(w => w.IsCenter == false).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var info = list[i];

                    WidgetPreviewItem item = new WidgetPreviewItem();
                    item.WidgetInfo = info;
                    item.WidgetID = info.WidgetID;
                    item.Name = info.Name;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                    item.Icon = string.Format("/UMPS1206;component/Themes/Default/UMPS1206/Images/Widgets/{0}.png", info.WidgetID);
                    mListLeftWidgetItems.Add(item);
                }

                list = mListMyWidgetInfos.Where(w => w.IsCenter).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var info = list[i];

                    WidgetPreviewItem item = new WidgetPreviewItem();
                    item.WidgetInfo = info;
                    item.WidgetID = info.WidgetID;
                    item.Name = info.Name;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                    item.Icon = string.Format("/UMPS1206;component/Themes/Default/UMPS1206/Images/Widgets/{0}.png", info.WidgetID);
                    mListCenterWidgetItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SaveUserWidgetInfos()
        {
            try
            {
                List<WidgetInfo> listInfos = new List<WidgetInfo>();
                for (int i = 0; i < mListLeftWidgetItems.Count; i++)
                {
                    var item = mListLeftWidgetItems[i];
                    var info = item.WidgetInfo;
                    if (info == null) { continue;}
                    info.IsCenter = false;
                    info.SortID = i;
                    listInfos.Add(info);
                }
                for (int i = 0; i < mListCenterWidgetItems.Count; i++)
                {
                    var item = mListCenterWidgetItems[i];
                    var info = item.WidgetInfo;
                    if (info == null) { continue; }
                    info.IsCenter = true;
                    info.SortID = i;
                    listInfos.Add(info);
                }
                int count = listInfos.Count;
                WebRequest webRequest=new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S1200Codes.SaveUserWidgetInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    var info = listInfos[i];
                    optReturn = XMLHelper.SeriallizeObject(info);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
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

                ShowInformation(CurrentApp.GetLanguageInfo("1206N001", "Save End"));

                if (PageParent != null)
                {
                    PageParent.Reload();
                }

                var popup = Parent as PopupPanel;
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void InitLang()
        {
            try
            {
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnCancel.Content = CurrentApp.GetLanguageInfo("COM002", "Cancel");

                var popup = Parent as PopupPanel;
                if (popup != null)
                {
                    popup.Title = CurrentApp.GetLanguageInfo("1206004", "Widget Setting");
                }

                TabAvaliable.Header = CurrentApp.GetLanguageInfo("1206005", "Avaliable Widget");
                TabLeftWidgets.Header = CurrentApp.GetLanguageInfo("1206006", "Left Widget");
                TabCenterWidget.Header = CurrentApp.GetLanguageInfo("1206007", "Center Widget");

                BtnAdd.ToolTip = CurrentApp.GetLanguageInfo("1206008", "Add");
                BtnRemove.ToolTip = CurrentApp.GetLanguageInfo("1206009", "Remove");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var popup = Parent as PopupPanel;
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveUserWidgetInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var indexTarget = TabControlMyWidgets.SelectedIndex;
                if (indexTarget == 0)
                {
                    var item = ListBoxLeft.SelectedItem as WidgetPreviewItem;
                    if (item == null) { return; }
                    mListLeftWidgetItems.Remove(item);
                }
                if (indexTarget == 1)
                {
                    var item = ListBoxCenter.SelectedItem as WidgetPreviewItem;
                    if (item == null) { return; }
                    mListCenterWidgetItems.Remove(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sourceItem = ListBoxAvaliable.SelectedItem as WidgetPreviewItem;
                if (sourceItem == null) { return; }
                var indexTarget = TabControlMyWidgets.SelectedIndex;
                if (indexTarget == 0)
                {
                    var temp = mListLeftWidgetItems.FirstOrDefault(w => w.WidgetID == sourceItem.WidgetID);
                    if (temp != null) { return; }//已经存在
                    var index = ListBoxLeft.SelectedIndex;
                    if (index >= 0)
                    {
                        mListLeftWidgetItems.Insert(index + 1, sourceItem);
                    }
                    else
                    {
                        mListLeftWidgetItems.Add(sourceItem);
                    }
                    ListBoxLeft.SelectedItem = sourceItem;
                    //Left 和 Center 不能存在相同的Widget
                    temp = mListCenterWidgetItems.FirstOrDefault(w => w.WidgetID == sourceItem.WidgetID);
                    if (temp != null)
                    {
                        mListCenterWidgetItems.Remove(temp);
                    }
                }
                if (indexTarget == 1)
                {
                    var temp = mListCenterWidgetItems.FirstOrDefault(w => w.WidgetID == sourceItem.WidgetID);
                    if (temp != null) { return; }//已经存在
                    var index = ListBoxCenter.SelectedIndex;
                    if (index >= 0)
                    {
                        mListCenterWidgetItems.Insert(index + 1, sourceItem);
                    }
                    else
                    {
                        mListCenterWidgetItems.Add(sourceItem);
                    }
                    ListBoxCenter.SelectedItem = sourceItem;
                    temp = mListLeftWidgetItems.FirstOrDefault(w => w.WidgetID == sourceItem.WidgetID);
                    if (temp != null)
                    {
                        mListLeftWidgetItems.Remove(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                for (int i = 0; i < mListAllWidgetItems.Count; i++)
                {
                    var item = mListAllWidgetItems[i];
                    var info = item.WidgetInfo;
                    if (info == null) { continue;}
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                }
                for (int i = 0; i < mListLeftWidgetItems.Count; i++)
                {
                    var item = mListLeftWidgetItems[i];
                    var info = item.WidgetInfo;
                    if (info == null) { continue; }
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                }
                for (int i = 0; i < mListCenterWidgetItems.Count; i++)
                {
                    var item = mListCenterWidgetItems[i];
                    var info = item.WidgetInfo;
                    if (info == null) { continue; }
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Name);
                    item.Description = item.Display;
                }

                InitLang();
            }
            catch { }
        }

        #endregion

    }
}
