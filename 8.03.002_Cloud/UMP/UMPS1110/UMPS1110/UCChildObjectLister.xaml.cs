//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    17cc82ee-65e8-4ebc-9943-80e0d9f6cb60
//        CLR Version:              4.0.30319.18444
//        Name:                     UCChildObjectLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                UCChildObjectLister
//
//        created by Charley at 2015/1/30 14:18:31
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS1110.Commands;
using UMPS1110.Models;
using UMPS1110.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;

namespace UMPS1110
{
    /// <summary>
    /// UCChildObjectLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCChildObjectLister
    {

        #region Members

        public ObjectItem ObjectItem { get; set; }
        public ConfigObject ConfigObject { get; set; }
        public List<ConfigObject> ListConfigObjects { get; set; }
        public List<ResourceTypeParam> ListResourceTypeParams { get; set; } 

        private ObservableCollection<ViewColumnInfo> mListChildObjectColumns;
        private ObservableCollection<ChildObjectItem> mListChildObjects;

        #endregion


        static UCChildObjectLister()
        {
            ChildObjectListEventEvent = EventManager.RegisterRoutedEvent("ChildObjectListerEvent", RoutingStrategy.Bubble,
                 typeof(RoutedPropertyChangedEventHandler<ChildObjectListerEventEventArgs>),
                 typeof(UCChildObjectLister));
        }

        public UCChildObjectLister()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ChildObjectListerCommands.ModifyCommand, ModifyCommand_Executed, (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ChildObjectListerCommands.DeleteCommand, DeleteCommand_Executed, (s, e) => e.CanExecute = true));

            mListChildObjectColumns = new ObservableCollection<ViewColumnInfo>();
            mListChildObjects = new ObservableCollection<ChildObjectItem>();

            Loaded += UCChildObjectLister_Loaded;
            LvChildObjects.SelectionChanged += LvChildObjects_SelectionChanged;
            LvChildObjects.MouseDoubleClick += LvChildObjects_MouseDoubleClick;
        }

        void UCChildObjectLister_Loaded(object sender, RoutedEventArgs e)
        {
            LvChildObjects.ItemsSource = mListChildObjects;
            InitChildObjectColumns();
            Init();
            CreateChildObjectColumns();
            ChangeLanguage();
        }

        public void ReloadData()
        {
            LoadChildObjects();
        }


        #region Init and Load

        private void Init()
        {
            LoadChildObjects();
        }

        private void InitChildObjectColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1110001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                mListChildObjectColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListChildObjectColumns.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadChildObjects()
        {
            try
            {
                mListChildObjects.Clear();
                if (ListConfigObjects == null) { return; }
                if (ObjectItem == null) { return; }
                ConfigGroup configGroup = ObjectItem.Data as ConfigGroup;
                if (configGroup == null) { return; }
                ConfigObject = configGroup.ConfigObject;
                if (ConfigObject == null) { return; }
                int childType = configGroup.ChildType;
                long parentID = 0;
                if (ListResourceTypeParams != null)
                {
                    ResourceTypeParam typeParam = ListResourceTypeParams.FirstOrDefault(t => t.TypeID == childType);
                    if (typeParam != null)
                    {
                        if (typeParam.ParentID > 0)
                        {
                            parentID = ConfigObject.ObjectID;
                        }
                    }
                }
                List<ConfigObject> listChildObjects;
                if (parentID != 0)
                {
                    listChildObjects =
                        ListConfigObjects.Where(o => o.ObjectType == configGroup.ChildType && o.ParentID == parentID)
                            .ToList();
                }
                else
                {
                    listChildObjects =
                       ListConfigObjects.Where(o => o.ObjectType == configGroup.ChildType)
                           .ToList();
                }
                for (int i = 0; i < listChildObjects.Count; i++)
                {
                    ConfigObject child = listChildObjects[i];
                    ChildObjectItem item = new ChildObjectItem();
                    item.ObjID = child.ObjectID;
                    item.Key = child.ID;
                    item.Name = child.Name;
                    item.Description = child.Description;
                    item.Icon = string.Format("Images/{0}", child.Icon);
                    item.ConfigObject = child;
                    if ((i % 2) == 1)
                    {
                        item.Background = Brushes.LightGray;
                    }
                    mListChildObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateChildObjectColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListChildObjectColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListChildObjectColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = App.GetLanguageInfo(string.Format("COL1110001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = App.GetLanguageInfo(string.Format("COL1110001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        //Binding binding;
                        DataTemplate dt;
                        if (columnInfo.ColumnName == "Icon" || columnInfo.ColumnName == "Operation")
                        {
                            if (columnInfo.ColumnName == "Icon")
                            {
                                dt = Resources["IconCellTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                            if (columnInfo.ColumnName == "Operation")
                            {
                                dt = Resources["OperationCellTemplate"] as DataTemplate;
                                if (dt != null)
                                {
                                    gvc.CellTemplate = dt;
                                }
                                else
                                {
                                    gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                                }
                            }
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvChildObjects.View = gv;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandler

        void LvChildObjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvChildObjects.SelectedItem as ChildObjectItem;
            if (item != null)
            {
                ChildObjectListerEventEventArgs args = new ChildObjectListerEventEventArgs();
                args.Code = 2;
                args.Data = item;
                OnChildObjectListerEvent(args);
            }
        }

        void LvChildObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = LvChildObjects.SelectedItem as ChildObjectItem;
            if (item != null)
            {
                ChildObjectListerEventEventArgs args = new ChildObjectListerEventEventArgs();
                args.Code = 1;
                args.Data = item;
                OnChildObjectListerEvent(args);
            }
        }

        #endregion


        #region Commands

        private void ModifyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as ChildObjectItem;
            if (item != null)
            {
                ChildObjectListerEventEventArgs args = new ChildObjectListerEventEventArgs();
                args.Code = 2;
                args.Data = item;
                OnChildObjectListerEvent(args);
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as ChildObjectItem;
            if (item != null)
            {
                string strMsg = string.Empty;
                List<ChildObjectItem> listItems = new List<ChildObjectItem>();
                listItems.Add(item);
                strMsg += string.Format("{0}\r\n", item.Name);
                var items = LvChildObjects.SelectedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    var temp = items[i] as ChildObjectItem;
                    if (temp != null)
                    {
                        if (!listItems.Contains(temp))
                        {
                            listItems.Add(temp);
                            strMsg += string.Format("{0}\r\n", temp.ObjID);
                        }
                    }
                }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                     App.GetMessageLanguageInfo("009", "Confirm remove this resource?"),
                     strMsg),
                     App.AppName,
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                ChildObjectListerEventEventArgs args = new ChildObjectListerEventEventArgs();
                args.Code = 3;
                args.Data = listItems;
                OnChildObjectListerEvent(args);
            }
        }

        #endregion


        #region ChildObjectListerEvent

        public static readonly RoutedEvent ChildObjectListEventEvent;

        public event RoutedPropertyChangedEventHandler<ChildObjectListerEventEventArgs> ChildObjectListerEvent
        {
            add { AddHandler(ChildObjectListEventEvent, value); }
            remove { RemoveHandler(ChildObjectListEventEvent, value); }
        }

        private void OnChildObjectListerEvent(ChildObjectListerEventEventArgs args)
        {
            RoutedPropertyChangedEventArgs<ChildObjectListerEventEventArgs> e =
                new RoutedPropertyChangedEventArgs<ChildObjectListerEventEventArgs>(null, args);
            e.RoutedEvent = ChildObjectListEventEvent;
            RaiseEvent(e);
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            CreateChildObjectColumns();

            for (int i = 0; i < mListChildObjects.Count; i++)
            {
                ChildObjectItem item = mListChildObjects[i];
                ConfigObject configObject = item.ConfigObject;
                if (configObject != null)
                {
                    item.Name = configObject.Name;
                    item.Description = configObject.Description;
                }
            }
        }

        #endregion
    }
}
