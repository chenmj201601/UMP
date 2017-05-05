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
using UMPS5100.ChildUCs;
using UMPS5100.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common5100;
using UMPS5100.Entities;
using UMPS5100.Service51001;

namespace UMPS5100.MainUserControls
{
    /// <summary>
    /// UC_BookmarkLevel.xaml 的交互逻辑
    /// </summary>
    public partial class UC_BookmarkLevel
    {
        #region 变量定义
        public static ObservableCollection<OperationInfo> ListOperationsOnlyAdd = new ObservableCollection<OperationInfo>();
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();
        private string mViewID;
        private ObservableCollection<ViewColumnInfo> mListColumnItems;
        public static ObservableCollection<BookmarkLevelEntityInList> lstLevelsInList = new ObservableCollection<BookmarkLevelEntityInList>();
        #endregion
    
        public UC_BookmarkLevel()
        {
            InitializeComponent();
            Loaded += UC_BookmarkLevel_Loaded;
            lvBookmarkLevelsObject.SelectionChanged += lvBookmarkLevelsObject_SelectionChanged;
        }

        void UC_BookmarkLevel_Loaded(object sender, RoutedEventArgs e)
        {
            lvBookmarkLevelsObject.ItemsSource = lstLevelsInList;
            //获得需要显示的列
            LoadViewColumnItems();
            //获得用户可使用的按钮(操作权限)
            GetUserOpts();
            InitLanguage();
            GetAllBookmarkLevel();

            int asdf = lvBookmarkLevelsObject.Items.Count;
            string st = asdf.ToString();
        }

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

        #region Init
        /// <summary>
        /// 初始化语言
        /// </summary>
        private void InitLanguage()
        {
            LbOperations.Text = CurrentApp.GetLanguageInfo("5101003", "Operations");
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("5101001", "Speech analysis-->Bookmark level");
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
            webRequest.ListData.Add("5101");
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
                    if (optInfo.ID == S5100Const.OPT_BookmarkLevelAdd)
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
            mViewID = "5101001";
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
                        if (columnInfo.ColumnName == "BookmarkLevelStatusIcon")
                        {
                            dt = Resources["EnableIconCellTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "BookmarkLeveColor")
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
                lvBookmarkLevelsObject.View = gv;
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

                //如果是“禁用“  就先创建按钮  在按钮下加一条线 
                if (item.ID == S5100Const.OPT_BookmarkLevelDisable)
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Background = Brushes.LightGray;
                    txtBlock.Height = 2;
                    txtBlock.Margin = new Thickness(0, 10, 0, 10);
                    txtBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    PanelOperationButtons.Children.Add(txtBlock);
                }
            }
        }

        private void GetAllBookmarkLevel()
        {
            lstLevelsInList.Clear();
            Service510011Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.GetAllBookmarkLevels;
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
                BookmarkLevelEntity level = null;
                BookmarkLevelEntityInList levelInList = null;
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BookmarkLevelEntity>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        level = optReturn.Data as BookmarkLevelEntity;
                        levelInList = new BookmarkLevelEntityInList();
                        levelInList.BookmarkLevelID = level.BookmarkLevelID;
                        levelInList.BookmarkLevelName = level.BookmarkLevelName;
                        levelInList.BookmarkLevelStatus = level.BookmarkLevelStatus;
                        levelInList.BookmarkLevelColor = level.BookmarkLevelColor;
                        if (level.BookmarkLevelStatus == "0")
                        {
                            levelInList.BookmarkLevelStatusIcon = "Images/disabled.ico";
                        }
                        else
                        {
                            levelInList.BookmarkLevelStatusIcon = "Images/avaliable.ico";
                        }
                        lstLevelsInList.Add(levelInList);
                        if (i % 2 == 1)
                        {
                            levelInList.Background = Brushes.LightGray;
                        }
                        else
                        {
                            levelInList.Background = Brushes.Transparent;
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

        #region 按钮操作事件
        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case S5100Const.OPT_BookmarkLevelAdd:
                        UC_BookmarkLevelEdit uc_Edit = new UC_BookmarkLevelEdit();
                        uc_Edit.CurrentApp = CurrentApp;
                        uc_Edit.iAddOrModify = OperationType.Add;
                        uc_Edit.ParentPage = this;
                        PopupPanel.Content = uc_Edit;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("5101004", "Add a new bookmark level ");
                        PopupPanel.IsOpen = true;
                        break;
                    case S5100Const.OPT_BookmarkLevelModify:
                        BookmarkLevelEntityInList level = lvBookmarkLevelsObject.SelectedItem as BookmarkLevelEntityInList;
                        if (level == null)
                        {
                            return;
                        }
                        uc_Edit = new UC_BookmarkLevelEdit();
                        uc_Edit.CurrentApp = CurrentApp;
                        uc_Edit.iAddOrModify = OperationType.Modify;
                        uc_Edit.ParentPage = this;
                        PopupPanel.Content = uc_Edit;
                        
                        uc_Edit.LevelInModify = level;
                        PopupPanel.Title = CurrentApp.GetLanguageInfo("5101006", "Modify bookmark level");
                        PopupPanel.IsOpen = true;
                        break;
                    case S5100Const.OPT_BookmarkLevelDisable:
                        LevelEnableDisable(Common5100.OperationType.Disable);
                        break;
                    case S5100Const.OPT_BookmarkLevelEnable:
                        LevelEnableDisable(Common5100.OperationType.Enable);
                        break;
                    case S5100Const.OPT_BookmarkLevelDelete:
                        DeleteLevel();
                        break;
                }
            }
        }

        private void LevelEnableDisable(OperationType optType)
        {
            BookmarkLevelEntityInList level = lvBookmarkLevelsObject.SelectedItem as BookmarkLevelEntityInList;
            if (level == null)
            {
                return;
            }
            string strPopupMsg = string.Empty;
            if (optType == Common5100.OperationType.Enable)
            {
                if (level.BookmarkLevelStatus == "1")
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("5101007", "This is already enabled, without the need to repeat the"));
                    return;
                }
                else
                {
                    strPopupMsg = CurrentApp.GetLanguageInfo("5101010", "Are you sure to enable this?");
                }
            }
            else if (optType == Common5100.OperationType.Disable)
            {
                if (level.BookmarkLevelStatus == "0")
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("5101008", "This is a disabled state, no need to repeat the disabled"));
                    return;
                }
                else
                {
                    strPopupMsg = CurrentApp.GetLanguageInfo("5101009", "Are you sure to disable this?");
                }
            }
            string strConfirm = CurrentApp.GetLanguageInfo("5102013", "Confirm");
            MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SetLevelStatus(optType,level);
            }
            else
            {
                string msg = string.Empty;
                if (optType == Common5100.OperationType.Enable)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101003")), level.BookmarkLevelName);
                    CurrentApp.WriteOperationLog("5101003", ConstValue.OPT_RESULT_CANCEL, msg);
                }
                else if (optType == Common5100.OperationType.Disable)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101004")), level.BookmarkLevelName);
                    CurrentApp.WriteOperationLog("5101004", ConstValue.OPT_RESULT_CANCEL, msg);
                }
            }
        }

        private void SetLevelStatus(OperationType optType, BookmarkLevelEntityInList level)
        {
            Service510011Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(level.BookmarkLevelID);
                if (optType == Common5100.OperationType.Enable)
                {
                    webRequest.ListData.Add("1");
                }
                else
                {
                    webRequest.ListData.Add("0");
                }
                webRequest.Code = (int)S5100RequestCode.SetBookmarkLevelStatus;
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                string msg = string.Empty;
                if (!webReturn.Result)
                {
                    if (optType == Common5100.OperationType.Enable)
                    {
                        msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101003")), level.BookmarkLevelName);
                        CurrentApp.WriteOperationLog("5101003", ConstValue.OPT_RESULT_FAIL, msg);
                    }
                    else if (optType == Common5100.OperationType.Disable)
                    {
                        msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101004")), level.BookmarkLevelName);
                        CurrentApp.WriteOperationLog("5101004", ConstValue.OPT_RESULT_FAIL, msg);
                    }
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
                if (optType == Common5100.OperationType.Disable)
                {
                    level.BookmarkLevelStatus = "0";
                    level.BookmarkLevelStatusIcon = "Images/disabled.ico";
                }
                else if (optType == Common5100.OperationType.Enable)
                {
                    level.BookmarkLevelStatus = "1";
                    level.BookmarkLevelStatusIcon = "Images/avaliable.ico";
                }
                if (optType == Common5100.OperationType.Enable)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101003")), level.BookmarkLevelName);
                    CurrentApp.WriteOperationLog("5101003", ConstValue.OPT_RESULT_SUCCESS, msg);
                }
                else if (optType == Common5100.OperationType.Disable)
                {
                    msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101004")), level.BookmarkLevelName);
                    CurrentApp.WriteOperationLog("5101004", ConstValue.OPT_RESULT_SUCCESS, msg);
                }
                UpdateListView(level, optType);
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

        private void DeleteLevel()
        {
            BookmarkLevelEntityInList level = lvBookmarkLevelsObject.SelectedItem as BookmarkLevelEntityInList;
            if (level == null)
            {
                return;
            }
            string strPopupMsg = CurrentApp.GetLanguageInfo("5101015", "Are you sure to delete this?");
            string strConfirm = CurrentApp.GetLanguageInfo("5102013", "Confirm");
            MessageBoxResult result = MessageBox.Show(strPopupMsg, strConfirm, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101005")), level.BookmarkLevelName);

            if (result == MessageBoxResult.Yes)
            {
                Service510011Client client = null;
                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(level.BookmarkLevelID);
                    webRequest.Code = (int)S5100RequestCode.DeleteBookmarkLevel;
                    client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                          WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                    client.Close();

                    if (!webReturn.Result)
                    {
                        CurrentApp.WriteOperationLog("5101005", ConstValue.OPT_RESULT_FAIL, msg);
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
                    CurrentApp.WriteOperationLog("5101005", ConstValue.OPT_RESULT_SUCCESS, msg);
                    UpdateListView(level, OperationType.Delete);
                }
                catch (Exception ex)
                {
                    CurrentApp.WriteOperationLog("5101005", ConstValue.OPT_RESULT_FAIL, msg);
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
            else
            {
                CurrentApp.WriteOperationLog("5101005", ConstValue.OPT_RESULT_FAIL, msg);
            }
            if (lstLevelsInList.Count > 0)
            {
                lvBookmarkLevelsObject.SelectedIndex = 0;
            }
        }

        void lvBookmarkLevelsObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateOptButtons(AllListOperations);
        }
        #endregion

        #region 子窗口调用的函数
        public void UpdateListView(BookmarkLevelEntityInList level,OperationType optType)
        {
            switch (optType)
            {
                case OperationType.Add:
                    if (lstLevelsInList.Count > 0)
                    {
                        BookmarkLevelEntityInList last = lstLevelsInList.Last();
                        if (last.Background == Brushes.Transparent)
                        {
                            level.Background = Brushes.LightGray;
                        }
                        else
                        {
                            level.Background = Brushes.Transparent;
                        }
                    }
                    else
                    {
                        level.Background = Brushes.Transparent;
                    }
                    lstLevelsInList.Add(level);
                    break;
                case OperationType.Modify:
                    BookmarkLevelEntityInList entity = lstLevelsInList.Where(p => p.BookmarkLevelID == level.BookmarkLevelID).First();
                    entity.BookmarkLevelColor = level.BookmarkLevelColor;
                    break;
                case OperationType.Delete:
                    entity = lstLevelsInList.Where(p => p.BookmarkLevelID == level.BookmarkLevelID).First();
                    lstLevelsInList.Remove(entity);
                    break;
                case Common5100.OperationType.Enable:
                     entity = lstLevelsInList.Where(p => p.BookmarkLevelID == level.BookmarkLevelID).First();
                    entity.BookmarkLevelStatus = level.BookmarkLevelStatus;
                    entity.BookmarkLevelStatusIcon = level.BookmarkLevelStatusIcon;
                    break;
                case Common5100.OperationType.Disable:
                     entity = lstLevelsInList.Where(p => p.BookmarkLevelID == level.BookmarkLevelID).First();
                    entity.BookmarkLevelStatus = level.BookmarkLevelStatus;
                    entity.BookmarkLevelStatusIcon = level.BookmarkLevelStatusIcon;
                    break;
            }
            PopupPanel.IsOpen = false;
        }
        #endregion
    }
}
