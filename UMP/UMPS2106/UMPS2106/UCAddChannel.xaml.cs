//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    aeccfd58-ee94-4cde-a61d-12c580586998
//        CLR Version:              4.0.30319.42000
//        Name:                     UCAddChannel
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106
//        File Name:                UCAddChannel
//
//        Created by Charley at 2016/10/19 20:02:16
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS2106.Models;
using UMPS2106.Wcf21061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21061;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2106
{
    /// <summary>
    /// UCAddChannel.xaml 的交互逻辑
    /// </summary>
    public partial class UCAddChannel
    {

        #region Members

        public PackageRecoverMainView PageParent;
        public StrategyItem StrategyItem;

        private List<ResourceObject> mListVoiceObjects;
        private List<ResourceObject> mListChannelObjects;
        private ObservableCollection<ResourceObjectItem> mListVoiceItems;
        private ObservableCollection<ResourceObjectItem> mListChannelItems;
        private bool mIsInited;

        #endregion


        public UCAddChannel()
        {
            InitializeComponent();

            mListVoiceObjects = new List<ResourceObject>();
            mListChannelObjects = new List<ResourceObject>();
            mListVoiceItems = new ObservableCollection<ResourceObjectItem>();
            mListChannelItems = new ObservableCollection<ResourceObjectItem>();

            Loaded += UCAddChannel_Loaded;
            ListBoxVoiceServers.SelectionChanged += ListBoxVoiceServers_SelectionChanged;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }


        void UCAddChannel_Loaded(object sender, RoutedEventArgs e)
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
                ListBoxVoiceServers.ItemsSource = mListVoiceItems;
                ListBoxChannels.ItemsSource = mListChannelItems;

                CommandBindings.Add(new CommandBinding(ItemClickCommand, ItemClickCommand_Executed,
                    (s, e) => e.CanExecute = true));

                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadVoiceObjects();
                    LoadChannelObjects();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }

                    InitVoiceItems();
                    InitVoiceItemState();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadVoiceObjects()
        {
            try
            {
                mListVoiceObjects.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetVoiceList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null."));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("ResourceObject is null."));
                        return;
                    }
                    mListVoiceObjects.Add(obj);
                }

                CurrentApp.WriteLog("LoadVoiceObjects", string.Format("End.\t{0}", mListVoiceObjects.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadChannelObjects()
        {
            try
            {
                mListChannelObjects.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < mListVoiceObjects.Count; i++)
                {
                    var voice = mListVoiceObjects[i];
                    long voiceObjID = voice.ObjID;

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2106Codes.GetChannelList;
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(voiceObjID.ToString());
                    Service21061Client client = new Service21061Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("ListData is null."));
                        return;
                    }
                    int count = 0;
                    for (int j = 0; j < webReturn.ListData.Count; j++)
                    {
                        string strInfo = webReturn.ListData[j];

                        optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        ResourceObject obj = optReturn.Data as ResourceObject;
                        if (obj == null)
                        {
                            ShowException(string.Format("ResourceObject is null."));
                            return;
                        }
                        mListChannelObjects.Add(obj);
                        count++;
                    }

                    CurrentApp.WriteLog("LoadChannelObjects", string.Format("End.\t{0}\t{1}", voiceObjID, count));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitVoiceItems()
        {
            try
            {
                mListVoiceItems.Clear();
                for (int i = 0; i < mListVoiceObjects.Count; i++)
                {
                    var voice = mListVoiceObjects[i];

                    ResourceObjectItem item = new ResourceObjectItem();
                    item.Info = voice;
                    item.ObjID = voice.ObjID;
                    item.ObjType = voice.ObjType;
                    item.Display = string.Format("[{0}]{1}", voice.Name, voice.FullName);
                    item.IsChecked = false;
                    mListVoiceItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitChannelItems()
        {
            try
            {
                mListChannelItems.Clear();
                var voiceItem = ListBoxVoiceServers.SelectedItem as ResourceObjectItem;
                if (voiceItem == null) { return; }
                var voice = voiceItem.Info;
                if (voice == null) { return; }
                long voiceObjID = voice.ObjID;
                var channels = mListChannelObjects.Where(c => c.ParentObjID == voiceObjID).ToList();
                for (int i = 0; i < channels.Count; i++)
                {
                    var channel = channels[i];

                    ResourceObjectItem channelItem = new ResourceObjectItem();
                    channelItem.Info = channel;
                    channelItem.ObjID = channel.ObjID;
                    channelItem.ObjType = channel.ObjType;
                    channelItem.Display = string.Format("[{0}]{1}", channel.Name, channel.FullName);
                    channelItem.IsChecked = false;
                    mListChannelItems.Add(channelItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitVoiceItemState()
        {
            try
            {
                if (StrategyItem == null) { return; }
                var strategyChannels = StrategyItem.ListChannels;
                for (int i = 0; i < mListVoiceItems.Count; i++)
                {
                    var voiceItem = mListVoiceItems[i];
                    var voice = voiceItem.Info;
                    if (voice == null) { continue; }
                    int voiceID;
                    if (int.TryParse(voice.Name, out voiceID))
                    {
                        var voiceStrategyChannels = strategyChannels.Where(c => c.VoiceID == voiceID);
                        var channels = mListChannelObjects.Where(c => c.ParentObjID == voice.ObjID);
                        var count =
                            voiceStrategyChannels.Count(
                                a => channels.Select(b => b.Name).Contains(a.ChannelID.ToString()));
                        if (count >= channels.Count())
                        {
                            voiceItem.IsChecked = true;
                        }
                        else if (count > 0)
                        {
                            voiceItem.IsChecked = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitChannelItemState()
        {
            try
            {
                if (StrategyItem == null) { return; }
                var voiceItem = ListBoxVoiceServers.SelectedItem as ResourceObjectItem;
                if (voiceItem == null) { return; }
                var voice = voiceItem.Info;
                if (voice == null) { return; }
                var recoverChannels = StrategyItem.ListChannels.Where(c => c.VoiceID.ToString() == voice.Name).ToList();
                for (int i = 0; i < mListChannelItems.Count; i++)
                {
                    var channelItem = mListChannelItems[i];
                    var channel = channelItem.Info;
                    if (channel == null) { continue; }
                    var temp = recoverChannels.FirstOrDefault(c => c.ChannelID.ToString() == channel.Name);
                    if (temp != null)
                    {
                        channelItem.IsChecked = true;
                    }
                    else
                    {
                        channelItem.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SetResourceItemState(ResourceObjectItem item)
        {
            try
            {
                if (StrategyItem == null) { return; }
                if (item == null) { return; }
                int objType = item.ObjType;
                if (objType == S2106Consts.RESOURCE_VOICESERVER)
                {

                    #region 录音服务器

                    ListBoxVoiceServers.SelectedItem = item;
                    var voice = item.Info;
                    if (voice == null) { return; }
                    int voiceID;
                    if (!int.TryParse(voice.Name, out voiceID)) { return; }
                    if (item.IsChecked == true)
                    {
                        //如果录音服务器勾上，则勾选所有该服务器下的通道
                        for (int i = 0; i < mListChannelItems.Count; i++)
                        {
                            var channelItem = mListChannelItems[i];
                            channelItem.IsChecked = true;
                            var channel = channelItem.Info;
                            if (channel == null) { continue; }
                            int channelID;
                            if (!int.TryParse(channel.Name, out channelID)) { continue; }
                            var temp =
                                StrategyItem.ListChannels.FirstOrDefault(
                                    c => c.VoiceID == voiceID && c.ChannelID == channelID);
                            if (temp == null)
                            {
                                //添加
                                temp = new RecoverChannelInfo();
                                temp.VoiceID = voiceID;
                                temp.ChannelID = channelID;
                                temp.Extension = channel.FullName;
                                temp.VoiceIP = voice.FullName;
                                StrategyItem.ListChannels.Add(temp);
                            }
                        }
                    }
                    if (item.IsChecked == false)
                    {
                        //如果取消勾选录音服务器，则取消勾选该服务器下的所有通道
                        for (int i = 0; i < mListChannelItems.Count; i++)
                        {
                            var channelItem = mListChannelItems[i];
                            channelItem.IsChecked = false;
                            var channel = channelItem.Info;
                            if (channel == null) { continue; }
                            int channelID;
                            if (!int.TryParse(channel.Name, out channelID)) { continue; }
                            var temp =
                                StrategyItem.ListChannels.FirstOrDefault(
                                    c => c.VoiceID == voiceID && c.ChannelID == channelID);
                            if (temp != null)
                            {
                                //移除
                                StrategyItem.ListChannels.Remove(temp);
                            }
                        }
                    }

                    #endregion

                }
                if (objType == S2106Consts.RESOURCE_VOICECHANNEL)
                {

                    #region 通道

                    ListBoxChannels.SelectedItem = item;
                    var channel = item.Info;
                    if (channel == null) { return; }
                    int channelID;
                    if (!int.TryParse(channel.Name, out channelID)) { return; }
                    long voiceObjID = channel.ParentObjID;
                    var voiceItem = mListVoiceItems.FirstOrDefault(v => v.ObjID == voiceObjID);
                    if (voiceItem == null) { return; }
                    var voice = voiceItem.Info;
                    if (voice == null) { return; }
                    int voiceID;
                    if (!int.TryParse(voice.Name, out voiceID)) { return; }
                    var temp =
                        StrategyItem.ListChannels.FirstOrDefault(c => c.ChannelID == channelID && c.VoiceID == voiceID);
                    if (item.IsChecked == true)
                    {
                        if (temp == null)
                        {
                            //添加
                            temp = new RecoverChannelInfo();
                            temp.VoiceID = voiceID;
                            temp.ChannelID = channelID;
                            temp.Extension = channel.FullName;
                            temp.VoiceIP = voice.FullName;
                            StrategyItem.ListChannels.Add(temp);
                        }
                    }
                    if (item.IsChecked == false)
                    {
                        if (temp != null)
                        {
                            //移除
                            StrategyItem.ListChannels.Remove(temp);
                        }
                    }

                    //设置Voice项的选中状态
                    bool isExist = false;
                    bool isAll = true;
                    for (int i = 0; i < mListChannelItems.Count; i++)
                    {
                        var channelItem = mListChannelItems[i];
                        if (channelItem.IsChecked == true)
                        {
                            isExist = true;
                        }
                        if (channelItem.IsChecked == false)
                        {
                            isAll = false;
                        }
                    }
                    if (isAll)
                    {
                        voiceItem.IsChecked = true;
                    }
                    else if (isExist)
                    {
                        voiceItem.IsChecked = null;
                    }
                    else
                    {
                        voiceItem.IsChecked = false;
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void ListBoxVoiceServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitChannelItems();
            InitChannelItemState();
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.AddedStrategyChannel();
            }
            var panel = Parent as PopupPanel;
            if (panel == null) { return; }
            panel.IsOpen = false;
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.AddedStrategyChannel();
            }
            var panel = Parent as PopupPanel;
            if (panel == null) { return; }
            panel.IsOpen = false;
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
            var item = e.Parameter as ResourceObjectItem;
            if (item == null) { return; }
            SetResourceItemState(item);
        }

        #endregion


        #region ChangeLanaugage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("2106012", "Add Recover Channel");
                }

                GroupVoiceServer.Header = CurrentApp.GetLanguageInfo("2106013", "Voice Server");
                GroupChannel.Header = CurrentApp.GetLanguageInfo("2106014", "Channel");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");
            }
            catch { }
        }

        #endregion

    }
}
