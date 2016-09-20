using Common5100;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.Common;
using UMPS5100.Entities;
using VoiceCyber.UMP.Communications;
using UMPS5100.Service11012;
using VoiceCyber.Common;
using UMPS5100.Service51001;
using UMPS5100.ChildUCs;

namespace UMPS5100.MainUserControls
{
    /// <summary>
    /// UC_KeyWorld.xaml 的交互逻辑
    /// </summary>
    public partial class UC_KeyWorld
    {
        #region 变量定义
        public static ObservableCollection<OperationInfo> ListOperationsOnlyAdd = new ObservableCollection<OperationInfo>();
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();
        private string mViewID;
        private ObservableCollection<ViewColumnInfo> mListColumnItems;
        public static ObservableCollection<KeyWorldsEntityInList> lstKeyWorldsInList = new ObservableCollection<KeyWorldsEntityInList>();
        public MainPage parentWin = null;
        #endregion

        public UC_KeyWorld()
        {
            InitializeComponent();
            Loaded += UC_KeyWorld_Loaded;
            lvKeyWorldsObject.SelectionChanged += lvKeyWorldsObject_SelectionChanged;
        }

        void UC_KeyWorld_Loaded(object sender, RoutedEventArgs e)
        {
            ListOperationsOnlyAdd.Clear();
            AllListOperations.Clear();
            lstKeyWorldsInList.Clear();
            lvKeyWorldsObject.ItemsSource = lstKeyWorldsInList;
            //获得需要显示的列
            LoadViewColumnItems();
            //获得用户可使用的按钮(操作权限)
            GetUserOpts();
            InitLanguage();
            GetAllKeyWorlds();
        }

        #region Init
        /// <summary>
        /// 初始化语言
        /// </summary>
        private void InitLanguage()
        {
            LbOperations.Text = CurrentApp.GetLanguageInfo("5102002", "Operations");
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("5102001", "Speech analysis --> keyword management");
            CreateColumnsItems();
            CreateOptButtons(ListOperationsOnlyAdd);
        }

        /// <summary>
        /// 获得用户可操作权限
        /// </summary>
        private void GetUserOpts()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)RequestCode.WSGetUserOptList;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
            webRequest.ListData.Add("51");
            webRequest.ListData.Add("5102");
            Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                 WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            ListOperationsOnlyAdd.Clear();
            AllListOperations.Clear();
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                OperationInfo optInfo = optReturn.Data as OperationInfo;
                if (optInfo != null)
                {
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                    AllListOperations.Add(optInfo);
                    if (optInfo.ID == S5100Const.OPT_KeyWorldAdd)
                    {
                        ListOperationsOnlyAdd.Add(optInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 获得需要显示的列
        /// </summary>
        private void LoadViewColumnItems()
        {
            mViewID = "5102001";
            mListColumnItems = new ObservableCollection<ViewColumnInfo>();
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
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
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
                listColumns.ForEach(obj => mListColumnItems.Add(obj));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 创建listview显示的列
        /// </summary>
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
                        string str = CurrentApp.Session.LangTypeID.ToString();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", mViewID, columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "KeyWorldColor")
                        {
                            dt = Resources["ColorCellTemplate"] as DataTemplate;
                        }
                        if (dt == null)
                        {
                            string strColName = columnInfo.ColumnName;
                            gvc.DisplayMemberBinding = new Binding(strColName);
                        }
                        else
                        {
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                int iCount = gv.Columns.Count;
                lvKeyWorldsObject.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 创建操作的按钮(仅显示基本操作--增加等)
        /// </summary>
        private void CreateOptButtons(ObservableCollection<OperationInfo> InOpts)
        {
            PanelOperationButtons.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < InOpts.Count; i++)
            {
                item = InOpts[i];

                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOperationButtons.Children.Add(btn);

            }
        }

        private void GetAllKeyWorlds()
        {
            Service510011Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.GetAllKeyWorlds;
                webRequest.Session = CurrentApp.Session;
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        ShowException(CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));
                        return;
                    }
                    else
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
                List<string> lstRecords = webReturn.ListData;
                OperationReturn optReturn = null;
                KeyWorldsEntity keyWrold = null;
                KeyWorldsEntityInList keyWroldInList = null;
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyWorldsEntity>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        keyWrold = optReturn.Data as KeyWorldsEntity;
                        keyWroldInList = new KeyWorldsEntityInList();
                        keyWroldInList.BookmarkLevelColor = keyWrold.BookmarkLevelcolor;
                        keyWroldInList.BookmarkLevelID = keyWrold.BookmarkLevelID;
                        keyWroldInList.KeyWorldContent = keyWrold.KeyWorldContent;
                        keyWroldInList.KeyWorldID = keyWrold.KeyWorldID;
                        keyWroldInList.LevelName = keyWrold.LevelName;
                        lstKeyWorldsInList.Add(keyWroldInList);
                        if (i % 2 == 1)
                        {
                            keyWroldInList.Background = Brushes.LightGray;
                        }
                        else
                        {
                            keyWroldInList.Background = Brushes.Transparent;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion

        #region Overried
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitLanguage();
            #region 循环修改操作按钮的显示文字
            Button btn;
            OperationInfo optInfo;
            ObservableCollection<OperationInfo> listOptInfos = new ObservableCollection<OperationInfo>();
            for (int i = 0; i < PanelOperationButtons.Children.Count; i++)
            {
                try
                {
                    btn = PanelOperationButtons.Children[i] as Button;
                    optInfo = btn.DataContext as OperationInfo;
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    listOptInfos.Add(optInfo);
                }
                catch
                {
                    continue;
                }
            }
            CreateOptButtons(listOptInfos);
            #endregion
            PopupPanel.ChangeLanguage();
        }
        #endregion

        #region 事件
        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case S5100Const.OPT_KeyWorldAdd:
                        UC_KeyWorldEdit ucKeyWorldEdit = new UC_KeyWorldEdit();
                        ucKeyWorldEdit.CurrentApp = CurrentApp;
                        ucKeyWorldEdit.iAddOrModify = OperationType.Add;
                        ucKeyWorldEdit.ParentPage = this;
                        PopupPanel.Content = ucKeyWorldEdit;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("5102004", "Add a new key world");
                        PopupPanel.IsOpen = true;
                        break;
                    case S5100Const.OPT_KeyWroldModify:
                        ucKeyWorldEdit = new UC_KeyWorldEdit();
                        ucKeyWorldEdit.CurrentApp = CurrentApp;
                        ucKeyWorldEdit.iAddOrModify = OperationType.Modify;
                        KeyWorldsEntityInList item = lvKeyWorldsObject.SelectedItem as KeyWorldsEntityInList;
                        ucKeyWorldEdit.keyWorldInModify = item;
                        ucKeyWorldEdit.ParentPage = this;
                        PopupPanel.Content = ucKeyWorldEdit;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("5102011", "Change the label level of keywords");
                        PopupPanel.IsOpen = true;
                        break;
                    case S5100Const.OPT_KeyWorldDelete:
                        string strConfirm = CurrentApp.GetLanguageInfo("5102013", "Confirm");
                        string strPopupMsg = CurrentApp.GetLanguageInfo("5102010", "Are you sure you delete this item?");
                        MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            DeleteKeyworld();
                        }
                        break;
                }
            }
        }

        private void DeleteKeyworld()
        {
            Service510011Client client = null;
            try
            {
                KeyWorldsEntityInList keyWorld = lvKeyWorldsObject.SelectedItem as KeyWorldsEntityInList;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.DeleteKeyWorld;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(keyWorld.KeyWorldID);
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        ShowException(CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));
                        return;
                    }
                    else
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("OPR1", "Success"));
                UpdateListView(keyWorld, OperationType.Delete);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        void lvKeyWorldsObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateOptButtons(AllListOperations);
        }
        #endregion

        #region 子窗口调用的函数
        public void UpdateListView(KeyWorldsEntityInList keyWorldEntity, OperationType optType)
        {
            switch (optType)
            {
                case OperationType.Add:
                    if (lstKeyWorldsInList.Count > 0)
                    {
                        KeyWorldsEntityInList last = lstKeyWorldsInList.Last();
                        if (last.Background == Brushes.LightGray)
                        {
                            keyWorldEntity.Background = Brushes.Transparent;
                        }
                        else
                        {
                            keyWorldEntity.Background = Brushes.LightGray;
                        }
                    }
                    else
                    {
                        keyWorldEntity.Background = Brushes.Transparent;
                    }
                    lstKeyWorldsInList.Add(keyWorldEntity);
                    break;
                case OperationType.Modify:
                    KeyWorldsEntityInList keyworld = lstKeyWorldsInList.Where(p => p.KeyWorldID == keyWorldEntity.KeyWorldID).First();
                    keyworld.BookmarkLevelColor = keyWorldEntity.BookmarkLevelColor;
                    break;
                case OperationType.Delete:
                    lstKeyWorldsInList.Remove(keyWorldEntity);
                    break;
            }
            PopupPanel.IsOpen = false;
        }
        #endregion
    }
}
