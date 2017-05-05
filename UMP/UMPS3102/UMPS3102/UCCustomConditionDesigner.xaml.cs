//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6b90482c-5487-41cd-93c5-9d6cf01773d9
//        CLR Version:              4.0.30319.18444
//        Name:                     UCCustomConditionDesigner
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCCustomConditionDesigner
//
//        created by Charley at 2014/11/23 13:35:53
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
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3102
{
    /// <summary>
    /// UCCustomConditionDesigner.xaml 的交互逻辑
    /// </summary>
    public partial class UCCustomConditionDesigner
    {
        #region Memeber

        private List<ConditionItemItem> mListCustomConditions;
        private ObservableCollection<ConditionItemItem> mListAvaliableConditions;
        private ObservableCollection<ConditionItemItem> mListUserConditions;
        private List<ToolButtonItem> mListToolButtons;
        private ObservableCollection<ViewColumnInfo> mListViewColumns;
        private ObservableCollection<ConditionTabItem> mListTabItems;

        #endregion


        public UCCustomConditionDesigner()
        {
            InitializeComponent();

            mListCustomConditions = new List<ConditionItemItem>();
            mListAvaliableConditions = new ObservableCollection<ConditionItemItem>();
            mListUserConditions = new ObservableCollection<ConditionItemItem>();
            mListToolButtons = new List<ToolButtonItem>();
            mListViewColumns = new ObservableCollection<ViewColumnInfo>();
            mListTabItems = new ObservableCollection<ConditionTabItem>();

            Loaded += UCCustomConditionDesigner_Loaded;

            CommandBindings.Add(new CommandBinding(UCConditionPreViewItem.RemoveCommand,
               RemoveCommand_Executed,
               (s, ee) => ee.CanExecute = true));

            LvAvaliableCondition.MouseDoubleClick += LvAvaliableCondition_MouseDoubleClick;
            BtnNewTabName.Click += BtnNewTabName_Click;

            LvAvaliableCondition.ItemsSource = mListAvaliableConditions;
            TabControlCondition.ItemsSource = mListTabItems;
        }

        void UCCustomConditionDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            LoadViewColumns();
            LoadAllConditionItems();
            LoadUserConditionItems();

            CreateViewColumns();
            CreateToolButton();
            CreateAvaliableConditionItems();
            CreateUserConditions();

            ChangeLanguage();
        }


        #region Load and Init

        private void LoadViewColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102005");
          //      Service11012Client client = new Service11012Client();
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                /* 
                 * 这个句子是从wcf11012里面打印出来的  可以看到执行过后结果就是在这个设计查询条件的框框内3列的列头的信息
                 * SELECT A.C001, A.C002, A.C003, B.C016, B.C011, B.C004
                 * FROM T_00_102 A, T_11_203_00000 B
                 * WHERE A.C001 = B.C002 AND A.C002 = B.C003 COLLATE DATABASE_DEFAULT
                 * AND B.C001 = 1020000000000000001 AND A.C001 = 3102005
                 * ORDER BY B.C004 ASC
                 */
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                mListViewColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListViewColumns.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAllConditionItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetAllCustomConditionItem;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
            //    Service31021Client client = new Service31021Client();
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. WebReturn ListData is null"));
                    return;
                }
                mListCustomConditions.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CustomConditionItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CustomConditionItem item = optReturn.Data as CustomConditionItem;//注意看这里的类型
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                    ConditionItemItem itemItem = new ConditionItemItem(item, CurrentApp);//再看下这里的类型  看下这个是怎么实现的
                    itemItem.CurrentApp = CurrentApp;
                    mListCustomConditions.Add(itemItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserConditionItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserCustomConditionItem;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
            //    Service31021Client client = new Service31021Client();
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. WebReturn ListData is null"));
                    return;
                }
                mListUserConditions.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CustomConditionItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CustomConditionItem item = optReturn.Data as CustomConditionItem;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                    if (mListCustomConditions.Count(c => c.ID == item.ID) > 0)
                    {
                        ConditionItemItem itemItem = new ConditionItemItem(item, CurrentApp);
                        mListUserConditions.Add(itemItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadDefaultConditionItems()//获得默认的查询条件  也就是当传入的参数为0的时候就为默认
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserCustomConditionItem;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("0");//传入的USERID 为0  那么就为默认的
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. WebReturn ListData is null"));
                    return;
                }
                mListUserConditions.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CustomConditionItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CustomConditionItem item = optReturn.Data as CustomConditionItem;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                    if (mListCustomConditions.Count(c => c.ID == item.ID) > 0)
                    {
                        ConditionItemItem itemItem = new ConditionItemItem(item, CurrentApp);
                        mListUserConditions.Add(itemItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateViewColumns()//创造每列的列头，在设计查询条件的设置里面，     规格，名称，类型
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;

                //mListViewColumns：在LoadViewColumns()里为这个变量塞值进去~
                for (int i = 0; i < mListViewColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListViewColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102005{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102005{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        if (columnInfo.ColumnName == "Name")
                        {
                            gvc.DisplayMemberBinding = new Binding("Display");
                        }
                        if (columnInfo.ColumnName == "Format")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrFormat");
                        }
                        if (columnInfo.ColumnName == "Type")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrType");
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvAvaliableCondition.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateToolButton()
        {
            try
            {
                mListToolButtons.Clear();

                ToolButtonItem item = new ToolButtonItem();
                item.Name = "TB" + "Add";
                item.Display = CurrentApp.GetLanguageInfo("31021701", "Add");
                item.Tip = CurrentApp.GetLanguageInfo("31021701", "Add new TabItem");
                item.Icon = "Images/add.png";
                mListToolButtons.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "Remove";
                item.Display = CurrentApp.GetLanguageInfo("31021702", "Remove");
                item.Tip = CurrentApp.GetLanguageInfo("31021702", "Remove TabItem");
                item.Icon = "Images/remove.png";
                mListToolButtons.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "Pre";
                item.Display = CurrentApp.GetLanguageInfo("31021703", "Pre");
                item.Tip = CurrentApp.GetLanguageInfo("31021703", "Move Pre");
                item.Icon = "Images/left.png";
                mListToolButtons.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "Next";
                item.Display = CurrentApp.GetLanguageInfo("31021704", "Next");
                item.Tip = CurrentApp.GetLanguageInfo("31021704", "Move next");
                item.Icon = "Images/right.png";
                mListToolButtons.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "Reset";
                item.Display = CurrentApp.GetLanguageInfo("31021705", "Reset");
                item.Tip = CurrentApp.GetLanguageInfo("31021705", "Reset to default");
                item.Icon = "Images/resetlayout.png";
                mListToolButtons.Add(item);

                PanelToolButtons.Children.Clear();
                for (int i = 0; i < mListToolButtons.Count; i++)
                {
                    item = mListToolButtons[i];

                    Button btn = new Button();
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += ToolButton_Click;
                    PanelToolButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateAvaliableConditionItems()//这个是读取可用的条件的，这些条件是在左边的Listview里面的，用户可以将其拖出来放到查询条件里面去使用
        {

            //mListUserConditions这个变量是在LoadAllConditionItems()和LoadDefaultConditionItems()里面塞值进去的
            //mListAvaliableConditions这个是在这里面塞值的,存的是Listview里面的可用查询条件,只是没有在查询窗口显示而已
            mListAvaliableConditions.Clear();
            for (int i = 0; i < mListCustomConditions.Count; i++)
            {
                ConditionItemItem item = mListCustomConditions[i];
                if (mListUserConditions.Count(c => c.ID == item.ID) <= 0)
                {
                    item.IsUserItem = false;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", item.ID), item.Name);
                    item.StrFormat =
                        CurrentApp.GetLanguageInfo(string.Format("3102TIP004{0}", ((int) item.Format).ToString("00")),
                            item.Format.ToString());
                    item.StrType = CurrentApp.GetLanguageInfo(string.Format("3102TIP003{0}", ((int)item.Type).ToString("00")), item.Type.ToString());
                    item.StartDragged += item_StartDragged;
                    item.DragOver += item_DragOver;
                    item.Dropped += item_Dropped;
                    mListAvaliableConditions.Add(item);
                }
            }
        }

        private void CreateUserConditions()
        {
            try
            {
                //
                //mListTabItems这个是在这个方法里面塞值进去的
                mListTabItems.Clear();
                List<ConditionTabItem> listTabItems = new List<ConditionTabItem>();
                for (int i = 0; i < mListUserConditions.Count; i++)
                {
                    ConditionItemItem item = mListUserConditions[i];
                    ConditionTabItem tabItem = listTabItems.FirstOrDefault(t => t.TabIndex == item.TabIndex);
                    if (tabItem == null)
                    {
                        tabItem = new ConditionTabItem();
                        tabItem.TabIndex = item.TabIndex;
                        tabItem.TabName = item.TabName;
                        if (string.IsNullOrEmpty(tabItem.TabName))
                        {
                            tabItem.TabName = string.Format("Condition {0}", tabItem.TabIndex);
                        }
                        if (tabItem.TabIndex < 10)
                        {
                            tabItem.TabName = CurrentApp.GetLanguageInfo(string.Format("3102TAB{0}", tabItem.TabIndex),
                               string.Format("Condition {0}", tabItem.TabIndex));
                        }
                        listTabItems.Add(tabItem);
                    }
                }
                listTabItems = listTabItems.OrderBy(t => t.TabIndex).ToList();
                for (int i = 0; i < listTabItems.Count; i++)
                {
                    ConditionTabItem tabItem = listTabItems[i];
                    tabItem.Items.Clear();
                    List<ConditionItemItem> listItems =
                        mListUserConditions.Where(c => c.TabIndex == tabItem.TabIndex).OrderBy(c => c.SortID).ToList();
                    for (int j = 0; j < listItems.Count; j++)
                    {
                        ConditionItemItem item = listItems[j];
                        item.IsUserItem = true;
                        item.Display = CurrentApp.GetLanguageInfo(string.Format("3102C{0}", item.ID), item.Name);
                        item.StrFormat =
                            CurrentApp.GetLanguageInfo(string.Format("3102TIP004{0}", ((int)item.Format).ToString("00")),
                                item.Format.ToString());
                        item.StrType = CurrentApp.GetLanguageInfo(string.Format("3102TIP003{0}", ((int)item.Type).ToString("00")), item.Type.ToString());
                        item.StartDragged += item_StartDragged;
                        item.DragOver += item_DragOver;
                        item.Dropped += item_Dropped;
                        item.SortID = j;
                        tabItem.Items.Add(item);
                    }
                    mListTabItems.Add(tabItem);
                }
                if (mListTabItems.Count > 0)
                {
                    TabControlCondition.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoDropOperation(ConditionItemItem sourceItem, ConditionItemItem targetItem)
        {
            try
            {
                if (sourceItem == null
                    || targetItem == null
                    || sourceItem == targetItem)
                {
                    return;
                }
                var item = mListUserConditions.FirstOrDefault(c => c.ID == sourceItem.ID);
                //如果源不在用户条件项集合中,将源添加到当前用户tab下的条件项集合中
                if (item == null)
                {
                    var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;
                    if (tabItem != null)
                    {
                        int index = tabItem.Items.IndexOf(targetItem);
                        sourceItem.IsUserItem = true;
                        sourceItem.TabIndex = tabItem.TabIndex;
                        sourceItem.TabName = tabItem.TabName;
                        tabItem.Items.Insert(index + 1, sourceItem);
                        tabItem.SetSortID();
                        mListUserConditions.Add(sourceItem);
                        mListAvaliableConditions.Remove(sourceItem);
                    }
                }
                else
                {
                    //调整顺序
                    var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;
                    if (tabItem != null)
                    {
                        tabItem.Items.Remove(sourceItem);
                        int index = tabItem.Items.IndexOf(targetItem);
                        tabItem.Items.Insert(index + 1, sourceItem);
                        tabItem.SetSortID();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region DragDrop

        void item_Dropped(object sender, DragDropEventArgs e)
        {
            var item = sender as ConditionItemItem;
            if (item != null && item.IsUserItem)
            {
                var source = e.DragData as IDataObject;
                if (source != null)
                {
                    var sourceItem = source.GetData(typeof(ConditionItemItem)) as ConditionItemItem;
                    if (sourceItem != null)
                    {
                        DoDropOperation(sourceItem, item);
                    }
                }
            }
        }

        void item_DragOver(object sender, DragDropEventArgs e)
        {

        }

        void item_StartDragged(object sender, DragDropEventArgs e)
        {
            var item = sender as ConditionItemItem;
            var dragSource = e.DragSource;
            if (item != null && dragSource != null)
            {
                DragDrop.DoDragDrop(dragSource, item, DragDropEffects.Move);
            }
        }

        #endregion


        #region EventHandler

        void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var item = btn.DataContext as ToolButtonItem;
                if (item != null)
                {
                    switch (item.Name)
                    {
                        case "TBAdd":
                            AddNewTabItem();
                            break;
                        case "TBRemove":
                            RemoveTabItem();
                            break;
                        case "TBPre":
                            MovePreTabItem();
                            break;
                        case "TBNext":
                            MoveNextTabItem();
                            break;
                        case "TBReset":
                            ResetToDefault();
                            break;
                    }
                }
            }
        }

        void LvAvaliableCondition_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvAvaliableCondition.SelectedItem as ConditionItemItem;
            if (item != null)
            {
                var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;
                if (tabItem != null)
                {
                    item.IsUserItem = true;
                    item.TabIndex = tabItem.TabIndex;
                    item.TabName = tabItem.TabName;
                    tabItem.Items.Add(item);
                    mListUserConditions.Add(item);
                    tabItem.SetSortID();
                    mListAvaliableConditions.Remove(item);
                }
            }
        }

        void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var uc = e.Parameter as UCConditionPreViewItem;
                if (uc != null)
                {
                    var item = uc.ConditionItemItem;
                    if (item != null)
                    {
                        var tabItem = mListTabItems.FirstOrDefault(t => t.TabIndex == item.TabIndex);
                        if (tabItem != null)
                        {
                            var conditionItem = tabItem.Items.FirstOrDefault(c => c.ID == item.ID);
                            if (conditionItem != null)
                            {
                                if (conditionItem.ConditionItem != null && conditionItem.ConditionItem.ViewMode == 2)
                                {
                                    ShowException(CurrentApp.GetMessageLanguageInfo("011", "Always show item can't be removed"));
                                    return;
                                }
                                tabItem.Items.Remove(conditionItem);
                                mListUserConditions.Remove(conditionItem);
                                tabItem.SetSortID();
                                conditionItem.IsUserItem = false;
                                mListAvaliableConditions.Add(conditionItem);
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

        void BtnNewTabName_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtNewTabName.Text))
            {
                //===================修改了提示框,并且为错误的提示信息设置了提示语言=============================
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N019", string.Format("Tab name is empty")));
                return;
                //=================by 俞强波 ====================================================================
            }
            if(string.IsNullOrWhiteSpace(TxtNewTabName.Text))
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N019", string.Format("Tab name is empty")));
                return;
            }
            try
            {
                string strTabName = TxtNewTabName.Text;
                int index = 0;
                for (int i = 0; i < mListTabItems.Count; i++)
                {
                    index = Math.Max(index, mListTabItems[i].TabIndex);
                }
                if (index < 10)
                {
                    index = 10;
                }
                else
                {
                    index++;
                }
                ConditionTabItem tabItem = new ConditionTabItem();
                tabItem.TabIndex = index;
                tabItem.TabName = strTabName;
                mListTabItems.Add(tabItem);
                TabControlCondition.SelectedItem = tabItem;
                BorderNewTabName.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddNewTabItem()
        {
            TxtNewTabName.Text = string.Empty;
            BorderNewTabName.Visibility = Visibility.Visible;
        }

        private void RemoveTabItem()
        {
            try
            {
                if (mListTabItems.Count <= 1)
                {
                    ShowException(CurrentApp.GetMessageLanguageInfo("013", "At least one tab item"));
                    return;
                }
                var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;

                for (int j = 0; j < tabItem.Items.Count; j++)
                {
                    if (tabItem.Items[j].Name == "TimeTypeFromTo")
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3102N038", "Can't delete the TabItem containing TimeTypeFromTo"));//加语言包~~
                        return;
                    }
                }
                if (tabItem != null)
                {
                    var result = MessageBox.Show(string.Format("{0}\r\n{1}",
                        CurrentApp.GetMessageLanguageInfo("012", "Confirm remove tab?"),
                        tabItem.TabName),
                        CurrentApp.AppName,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = tabItem.Items.Count - 1; i >= 0; i--)
                        {
                            ConditionItemItem item = tabItem.Items[i];
                            item.IsUserItem = false;
                            tabItem.Items.Remove(item);
                            mListUserConditions.Remove(item);
                            mListAvaliableConditions.Add(item);
                        }
                        mListTabItems.Remove(tabItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MovePreTabItem()
        {
            try
            {
                var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;
                var index = TabControlCondition.SelectedIndex;
                if (tabItem != null)
                {
                    if (index > 0)
                    {
                        var preTabItem = mListTabItems[index - 1];
                        if (preTabItem.TabIndex < 10) { return; }
                        //交换TabIndex
                        int temp = preTabItem.TabIndex;
                        preTabItem.TabIndex = tabItem.TabIndex;
                        for (int i = 0; i < preTabItem.Items.Count; i++)
                        {
                            preTabItem.Items[i].TabIndex = preTabItem.TabIndex;
                        }
                        tabItem.TabIndex = temp;
                        for (int i = 0; i < tabItem.Items.Count; i++)
                        {
                            tabItem.Items[i].TabIndex = tabItem.TabIndex;
                        }
                        mListTabItems.Remove(tabItem);
                        mListTabItems.Insert(index - 1, tabItem);
                        TabControlCondition.SelectedItem = tabItem;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MoveNextTabItem()
        {
            try
            {
                var tabItem = TabControlCondition.SelectedItem as ConditionTabItem;
                var index = TabControlCondition.SelectedIndex;
                if (tabItem != null)
                {
                    if (index < mListTabItems.Count - 1)
                    {
                        var nextTabItem = mListTabItems[index + 1];
                        if (tabItem.TabIndex < 10) { return; }
                        //交换TabIndex
                        int temp = nextTabItem.TabIndex;
                        nextTabItem.TabIndex = tabItem.TabIndex;
                        for (int i = 0; i < nextTabItem.Items.Count; i++)
                        {
                            nextTabItem.Items[i].TabIndex = nextTabItem.TabIndex;
                        }
                        tabItem.TabIndex = temp;
                        for (int i = 0; i < tabItem.Items.Count; i++)
                        {
                            tabItem.Items[i].TabIndex = tabItem.TabIndex;
                        }
                        mListTabItems.Remove(tabItem);
                        mListTabItems.Insert(index + 1, tabItem);
                        TabControlCondition.SelectedItem = tabItem;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ResetToDefault()
        {
            try
            {
                LoadDefaultConditionItems();
                CreateAvaliableConditionItems();
                CreateUserConditions();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public OperationReturn SaveConfig()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<CustomConditionItem> listItems = new List<CustomConditionItem>();
                for (int i = 0; i < mListTabItems.Count; i++)
                {
                    ConditionTabItem tabItem = mListTabItems[i];
                    for (int j = 0; j < tabItem.Items.Count; j++)
                    {
                        ConditionItemItem item = tabItem.Items[j];
                        item.Apply();
                        if (item.ConditionItem != null)
                        {
                            item.ConditionItem.ViewMode = item.ConditionItem.ViewMode == 2 ? 2 : 1;
                        }
                        listItems.Add(item.ConditionItem);
                    }
                }
                if (listItems.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3102Codes.SaveUserConditionItemInfos;
                    webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    int count = listItems.Count;
                    webRequest.ListData.Add(count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(listItems[i]);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31021Client client = new Service31021Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31021"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                TxtAvaliableCondition.Text = CurrentApp.GetLanguageInfo("31021700", "Avaliable Conditions");

                //视图列
                CreateViewColumns();

                //按钮
                CreateToolButton();

                //条件项
                CreateAvaliableConditionItems();
                CreateUserConditions();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

    }
}
