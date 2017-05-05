//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8327c550-fe92-4993-9fb8-6ea9d998d05e
//        CLR Version:              4.0.30319.18052
//        Name:                     UCResourceObjectLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                UCResourceObjectLister
//
//        created by Charley at 2015/4/19 15:37:21
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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS1110.Commands;
using UMPS1110.Models;
using UMPS1110.Models.ConfigObjects;
using UMPS1110.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.CustomControls.ListItem.Implementation;

namespace UMPS1110
{
    /// <summary>
    /// UCResourceObjectLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCResourceObjectLister
    {

        #region Members

        public ObjectItem ObjectItem { get; set; }
        public List<ResourceTypeParam> ListAllTypeParams { get; set; }
        public List<ConfigObject> ListAllConfigObjects { get; set; }

        private bool mIsInited;
        private int mObjectType;
        private string mViewID;
        private ConfigObject mParentObject;
        private ObservableCollection<ConfigObject> mListConfigObjects;
        private ObservableCollection<ViewColumnInfo> mListColumnItems;

        #endregion


        static UCResourceObjectLister()
        {
            ResourceObjectListerEventEvent = EventManager.RegisterRoutedEvent("ResourceObjectListerEvent",
                RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ResourceObjectListerEventEventArgs>),
                typeof(UCResourceObjectLister));
        }

        public UCResourceObjectLister()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ResourceObjectListerCommands.ModifyCommand, ModifyCommand_Executed, (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ResourceObjectListerCommands.DeleteCommand, DeleteCommand_Executed, (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ResourceObjectListerCommands.EnableDisableCommand, EnableDisableCommand_Executed, (s, e) => e.CanExecute = true));

            mListConfigObjects = new ObservableCollection<ConfigObject>();
            mListColumnItems = new ObservableCollection<ViewColumnInfo>();

            Loaded += UCResourceObjectLister_Loaded;
            LvResourceObjects.SelectionChanged += LvResourceObjects_SelectionChanged;

            LvResourceObjects.AddHandler(ListItemPanel.ItemMouseDoubleClickEvent,
                new RoutedPropertyChangedEventHandler<ListItemEventArgs>(LvResourceObjects_MouseDoubleClick));
        }

        void UCResourceObjectLister_Loaded(object sender, RoutedEventArgs e)
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
                LvResourceObjects.ItemsSource = mListConfigObjects;
                mViewID = string.Format("1110010");

                if (ObjectItem == null) { return; }
                ConfigGroup configGroup = ObjectItem.Data as ConfigGroup;
                if (configGroup == null) { return; }
                if (configGroup.GroupType != ResourceGroupType.ChildList) { return; }
                mParentObject = configGroup.ConfigObject;
                mObjectType = configGroup.ChildType;
                if (mObjectType > 0)
                {
                    switch (mObjectType)
                    {
                        case S1110Consts.RESOURCE_MACHINE:
                            mViewID = string.Format("1110210");
                            break;
                        case S1110Consts.RESOURCE_LICENSESERVER:
                        case S1110Consts.RESOURCE_DATATRANSFERSERVER:
                        case S1110Consts.RESOURCE_CTIHUBSERVER:
                        case S1110Consts.RESOURCE_DBBRIDGE:
                        case S1110Consts.RESOURCE_ALARMMONITOR:
                        case S1110Consts.RESOURCE_ALARMSERVER:
                        case S1110Consts.RESOURCE_SFTP:
                        case S1110Consts.RESOURCE_VOICESERVER:
                        case S1110Consts.RESOURCE_SCREENSERVER:
                        case S1110Consts.RESOURCE_ISASERVER:
                        case S1110Consts.RESOURCE_CMSERVER:
                        case S1110Consts.RESOURCE_KEYGENERATOR:
                        case S1110Consts.RESOURCE_FILEOPERATOR:
                        case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:
                            mViewID = string.Format("1110011");
                            break;
                        case S1110Consts.RESOURCE_STORAGEDEVICE:
                            mViewID = string.Format("1110214");
                            break;
                        case S1110Consts.RESOURCE_PBXDEVICE:
                            mViewID = string.Format("1110220");
                            break;
                        case S1110Consts.RESOURCE_CHANNEL:
                            mViewID = string.Format("1110225");
                            break;
                        case S1110Consts.RESOURCE_SCREENCHANNEL:
                            mViewID = string.Format("1110012");
                            break;
                        case S1110Consts.RESOURCE_DOWNLOADPARAM:
                            mViewID = string.Format("1110291");
                            break;
                    }
                }

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadViewColumnItems();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    InitResourceObjects();
                    CreateColumnsItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadViewColumnItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(mViewID);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListColumnItems.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListColumnItems.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException("LoadViewColumnItems:"+ex.Message);
            }
        }

        private void InitResourceObjects()
        {
            try
            {
                mListConfigObjects.Clear();
                if (mParentObject == null) { return; }
                List<ConfigObject> listItems =
                    mParentObject.ListChildObjects.Where(o => o.ObjectType == mObjectType).ToList();
                listItems = listItems.OrderBy(o => o.ID).ToList();
                for (int i = 0; i < listItems.Count; i++)
                {
                    ConfigObject configObject = listItems[i];
                    configObject.StrIcon = string.Format("Images/{0}", configObject.Icon);
                    if (i % 2 == 1)
                    {
                        configObject.Background = Brushes.LightGray;
                    }
                    else
                    {
                        configObject.Background = Brushes.Transparent;
                    }
                    mListConfigObjects.Add(listItems[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException("InitResourceObjects:"+ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateColumnsItems()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListColumnItems.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListColumnItems[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "Icon")
                        {
                            dt = Resources["IconCellTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "IsEnabled")
                        {
                            dt = Resources["StateCellTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "Operation")
                        {
                            dt = Resources["OperationCellTemplate"] as DataTemplate;
                        }
                        if (dt == null)
                        {
                            string strColName = columnInfo.ColumnName;
                            if (strColName == "StartType")
                            {
                                strColName = "StrStartType";
                            }
                            if (strColName == "CTIType")
                            {
                                strColName = "StrCTIType";
                            }
                            if (strColName == "DeviceType")
                            {
                                strColName = "StrDeviceType";
                            }
                            if (strColName == "MonitorMode")
                            {
                                strColName = "StrMonitorMode";
                            }
                            gvc.DisplayMemberBinding = new Binding(strColName);
                        }
                        else
                        {
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvResourceObjects.View = gv;
            }
            catch (Exception ex)
            {
                ShowException("CreateColumnsItems:"+ex.Message);
            }
        }

        public void ReloadData()
        {
            InitResourceObjects();
        }

        #endregion


        #region EventHandlers

        void LvResourceObjects_MouseDoubleClick(object sender, RoutedPropertyChangedEventArgs<ListItemEventArgs> e)
        {
            var item = LvResourceObjects.SelectedItem as ConfigObject;
            if (item != null)
            {
                ResourceObjectListerEventEventArgs args = new ResourceObjectListerEventEventArgs();
                args.Code = 2;
                args.Data = item;
                OnResourceObjectListerEvent(args);
            }
        }

        void LvResourceObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = LvResourceObjects.SelectedItem as ConfigObject;
            if (item != null)
            {
                ResourceObjectListerEventEventArgs args = new ResourceObjectListerEventEventArgs();
                args.Code = 1;
                args.Data = item;
                OnResourceObjectListerEvent(args);
            }
        }

        #endregion


        #region Commands

        private void ModifyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as ConfigObject;
            if (item != null)
            {
                List<ConfigObject> listItems = new List<ConfigObject>();
                listItems.Add(item);
                var items = LvResourceObjects.SelectedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    var temp = items[i] as ConfigObject;
                    if (temp != null)
                    {
                        if (!listItems.Contains(temp))
                        {
                            listItems.Add(temp);
                        }
                    }
                }
                ResourceObjectListerEventEventArgs args = new ResourceObjectListerEventEventArgs();
                args.Code = 2;
                args.Data = listItems;
                OnResourceObjectListerEvent(args);
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as ConfigObject;
            if (item != null)
            {
                string strMsg = string.Empty;
                List<ConfigObject> listItems = new List<ConfigObject>();
                listItems.Add(item);
                strMsg += string.Format("{0}\r\n", item.Name);
                var items = LvResourceObjects.SelectedItems;
                bool over = false;
                for (int i = 0; i < items.Count; i++)
                {
                    var temp = items[i] as ConfigObject;
                    if (temp != null)
                    {
                        if (!listItems.Contains(temp))
                        {
                            listItems.Add(temp);
                            //提示消息最长128个字符
                            if (strMsg.Length < 128 && !over)
                            {
                                strMsg += string.Format("{0}\r\n", temp.Name);
                            }
                            else if(!over)
                            {
                                strMsg += string.Format("...");
                                over = true;
                            }
                        }
                    }
                }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                     CurrentApp.GetMessageLanguageInfo("009", "Confirm remove this resource?"),
                     strMsg),
                     CurrentApp.AppName,
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                ResourceObjectListerEventEventArgs args = new ResourceObjectListerEventEventArgs();
                args.Code = 3;
                args.Data = listItems;
                OnResourceObjectListerEvent(args);
            }
        }

        private void EnableDisableCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as ConfigObject;
            if (item != null)
            {
                IEnableDisableObject obj = item as IEnableDisableObject;
                if (obj != null)
                {
                    item.SetPropertyValue(S1110Consts.PROPERTYID_ENABLEDISABLE, obj.IsEnabled ? "0" : "1");
                }
            }
        }

        #endregion


        #region ResourceObjectListerEvent

        public static readonly RoutedEvent ResourceObjectListerEventEvent;

        public event RoutedPropertyChangedEventHandler<ResourceObjectListerEventEventArgs> ResourceObjectListerEvent
        {
            add { AddHandler(ResourceObjectListerEventEvent, value); }
            remove { RemoveHandler(ResourceObjectListerEventEvent, value); }
        }

        private void OnResourceObjectListerEvent(ResourceObjectListerEventEventArgs args)
        {
            RoutedPropertyChangedEventArgs<ResourceObjectListerEventEventArgs> e =
                new RoutedPropertyChangedEventArgs<ResourceObjectListerEventEventArgs>(null, args);
            e.RoutedEvent = ResourceObjectListerEventEvent;
            RaiseEvent(e);
        }

        #endregion


        #region Change Language

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            CreateColumnsItems();
        }

        #endregion

    }
}
