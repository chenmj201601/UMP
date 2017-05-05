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
using UMPS3108.Models;
using UMPS3108.Wcf31081;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3108
{
    /// <summary>
    /// CombStatiParaItemsDesigner.xaml 的交互逻辑
    /// </summary>
    public partial class CombStatiParaItemsDesigner
    {
        public SCMainView PageParent;
        //public ParamsItemsConfigPage paramsItemsconfigPage;

        private List<CombinedParamItemModel> mListAllCombinedParamItem;

        private ObservableCollection<CombinedParamItemModel> mListAvaliableParamItems;
        private ObservableCollection<ViewColumnInfo> mListViewColumns;
        private List<CombinedParamItemModel> mListCustomParamsItems;

        //放入到可组合的大项里面的可组合的参数子项,
        private ObservableCollection<CombinedParamItemModel> mListAddedParamsItems;

        private ObservableCollection<CombinedParamTabItem> mListTabItems;

        //private ParamsItemsConfigPage paramsItemsConfigPage;

        public CombStatiParaItemsDesigner()
        {
            InitializeComponent();

            mListAvaliableParamItems = new ObservableCollection<CombinedParamItemModel>();
            mListCustomParamsItems = new List<CombinedParamItemModel>();
            mListViewColumns = new ObservableCollection<ViewColumnInfo>();
            mListAddedParamsItems = new ObservableCollection<CombinedParamItemModel>();
            mListAllCombinedParamItem = new List<CombinedParamItemModel>();
            mListTabItems = new ObservableCollection<CombinedParamTabItem>();

            LvAvaliableParamItem.MouseDoubleClick += LvAvaliableParamItem_MouseDoubleClick;
            BtnSave.Click += BtnSave_Click;
            BtnCancel.Click += BtnCancel_Click;

            Loaded+=CombStatiParaItemsDesigner_Loaded;

            
        }

        private void CombStatiParaItemsDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            LvAvaliableParamItem.ItemsSource = mListAvaliableParamItems;
            TabControlCondition.ItemsSource = mListTabItems;

            LoadViewColumns();
            LoadAllCombinedParamItems();
            LoadAddedCombinedParamItems();
            CreateViewColumns();
            CreateAvaliableParamsItems();
            CreateCombinedParamTabItem();

            ChangeLanguage();

            CommandBindings.Add(new CommandBinding(CombinedParamItemsPreViewItem.RemoveCommand,
                RemoveCommand_Executed,
                (s, ee) => ee.CanExecute = true));
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
                webRequest.ListData.Add("3108001");
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

        private void LoadAllCombinedParamItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetAllCombinedParamItems;
                webRequest.Session = CurrentApp.Session;
                //webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
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
                mListAllCombinedParamItem.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalParamItem item = optReturn.Data as StatisticalParamItem;//注意看这里的类型
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                    CombinedParamItemModel itemItem = new CombinedParamItemModel(item,CurrentApp);//再看下这里的类型  看下这个是怎么实现的
                    mListAllCombinedParamItem.Add(itemItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAddedCombinedParamItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetAddedCombinedParamItems;
                webRequest.Session = CurrentApp.Session;
                //webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
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
                mListAddedParamsItems.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalParamItem item = optReturn.Data as StatisticalParamItem;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. StatisticalParamItem is null"));
                        return;
                    }
                    if (mListAllCombinedParamItem.Count(c => c.StatisticalParamItemID == item.StatisticalParamItemID.ToString()) > 0)
                    {
                        CombinedParamItemModel itemItem = new CombinedParamItemModel(item, CurrentApp);
                        mListAddedParamsItems.Add(itemItem);
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

        private void CreateViewColumns()//创造每列的列头， 规格，名称，类型
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
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3108001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3108001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        if (columnInfo.ColumnName == "NAME")
                        {
                            gvc.DisplayMemberBinding = new Binding("Display");
                        }
                        if (columnInfo.ColumnName == "FORMAT")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrFormat");
                        }
                        if (columnInfo.ColumnName == "TYPE")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrType");
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvAvaliableParamItem.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateAvaliableParamsItems()//这个是读取可用的条件的，这些条件是在左边的Listview里面的，用户可以将其拖出来放到查询条件里面去使用
        {
            //mListUserConditions这个变量是在LoadAllConditionItems()和LoadDefaultConditionItems()里面塞值进去的
            //mListAvaliableConditions这个是在这里面塞值的,存的是Listview里面的可用查询条件,只是没有在查询窗口显示而已
            mListAvaliableParamItems.Clear();
            for (int i = 0; i < mListAllCombinedParamItem.Count; i++)
            {
                CombinedParamItemModel item = mListAllCombinedParamItem[i];
                if (item.ID == 0 || mListAddedParamsItems.Count(c => c.ID == item.ID) <= 0)
                {
                    item.IsAddedItem = false;
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", item.StatisticalParamItemID), item.Name);
                    item.StrFormat =
                        CurrentApp.GetLanguageInfo(string.Format("310801FORMAT0{0}", ((int)item.Format).ToString("00")),
                            item.Format.ToString());
                    item.StrType = CurrentApp.GetLanguageInfo(string.Format("310801TYPE0{0}", ((int)item.Type).ToString("00")), item.Type.ToString());
                    item.StartDragged += item_StartDragged;
                    item.DragOver += item_DragOver;
                    item.Dropped += item_Dropped;
                    mListAvaliableParamItems.Add(item);
                }
            }
        }

        private void CreateCombinedParamTabItem()
        {
            try
            {
                //mListTabItems这个是在这个方法里面塞值进去的
                mListTabItems.Clear();
                List<CombinedParamTabItem> listTabItems = new List<CombinedParamTabItem>();
                //先写个方法
                for (int i = 0; i < 2; i++)
                {
                    CombinedParamTabItem tabItem = new CombinedParamTabItem();
                    if (i == 0)
                    {
                        tabItem.TabIndex = 0;
                        tabItem.ID = 3110000000000000001;
                        tabItem.TabName = CurrentApp.GetLanguageInfo("FO31080102001", "ServiceAttitude");
                    }
                    if (i == 1)
                    {
                        tabItem.TabIndex = 1;
                        tabItem.ID = 3110000000000000002;
                        tabItem.TabName = CurrentApp.GetLanguageInfo("FO31080102002", "ProfessionalLevel");
                    }
                    listTabItems.Add(tabItem);
                }
                listTabItems = listTabItems.OrderBy(t => t.TabIndex).ToList();
                for (int i = 0; i < listTabItems.Count; i++)
                {
                    CombinedParamTabItem tabItem = listTabItems[i];
                    tabItem.Items.Clear();
                    List<CombinedParamItemModel> listItems =
                        mListAddedParamsItems.Where(c => c.ID == tabItem.ID).OrderBy(c => c.SortID).ToList();
                    for (int j = 0; j < listItems.Count; j++)
                    {
                        CombinedParamItemModel item = listItems[j];
                        item.IsAddedItem = true;
                        item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C{0}", item.ID), item.Name);
                        item.StrFormat =
                            CurrentApp.GetLanguageInfo(string.Format("3108TIP004{0}", ((int)item.Format).ToString("00")),
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
                    StatisticalParam temps = ParamsItemsConfigPage.StatisticalParam_;
                    if (temps.StatisticalParamID == 3110000000000000001)
                    {
                        TabControlCondition.SelectedIndex = 0;
                        var item = mListTabItems.FirstOrDefault(t => t.ID == 3110000000000000001);
                        if (item != null)
                        {
                            item.IsEnable = true;
                        }
                    }
                    if (temps.StatisticalParamID == 3110000000000000002)
                    {
                        TabControlCondition.SelectedIndex = 1;
                        var item = mListTabItems.FirstOrDefault(t => t.ID == 3110000000000000002);
                        if (item != null)
                        {
                            item.IsEnable = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoDropOperation(CombinedParamItemModel sourceItem, CombinedParamItemModel targetItem)
        {
            try
            {
                if (sourceItem == null
                    || targetItem == null
                    || sourceItem == targetItem)
                {
                    return;
                }
                var item = mListAddedParamsItems.FirstOrDefault(c => c.ID == sourceItem.ID);
                //如果源不在用户条件项集合中,将源添加到当前用户tab下的条件项集合中
                if (item == null)
                {
                    var tabItem = TabControlCondition.SelectedItem as CombinedParamTabItem;
                    if (tabItem != null)
                    {
                        int index = tabItem.Items.IndexOf(targetItem);
                        sourceItem.IsAddedItem = true;
                        sourceItem.TabIndex = tabItem.TabIndex;
                        sourceItem.TabName = tabItem.TabName;
                        tabItem.Items.Insert(index + 1, sourceItem);
                        tabItem.SetSortID();
                        mListAddedParamsItems.Add(sourceItem);
                        mListAvaliableParamItems.Remove(sourceItem);
                    }
                }
                else
                {
                    //调整顺序
                    var tabItem = TabControlCondition.SelectedItem as CombinedParamTabItem;
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


        #region EventHandler

        private void LvAvaliableParamItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvAvaliableParamItem.SelectedItem as CombinedParamItemModel;
            if (item != null)
            {
                var tabItem = TabControlCondition.SelectedItem as CombinedParamTabItem;
                if (tabItem != null)
                {
                    item.IsAddedItem = true;
                    item.TabIndex = tabItem.TabIndex;
                    item.TabName = tabItem.TabName;
                    tabItem.Items.Add(item);
                    mListAddedParamsItems.Add(item);
                    tabItem.SetSortID();
                    mListAvaliableParamItems.Remove(item);
                }
            }
        }

        void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var uc = e.Parameter as CombinedParamItemsPreViewItem;
                uc.CurrentApp = CurrentApp;
                if (uc != null)
                {
                    var item = uc.CombinedParamItem;
                    if (item != null)
                    {
                        var tabItem = mListTabItems.FirstOrDefault(t => t.ID == item.ID);
                        if (tabItem != null)
                        {
                            var conditionItem = tabItem.Items.FirstOrDefault(c => c.StatisticalParamItemID == item.StatisticalParamItemID);
                            if (conditionItem != null)
                            {
                                tabItem.Items.Remove(conditionItem);
                                mListAddedParamsItems.Remove(conditionItem);
                                tabItem.SetSortID();
                                conditionItem.IsAddedItem = false;
                                conditionItem.ID = 0;
                                conditionItem.Remove();
                                    conditionItem.IsAddedItem = false;
                                    conditionItem.Display = CurrentApp.GetLanguageInfo(string.Format("3108A{0}", item.StatisticalParamItemID), item.Name);
                                    conditionItem.StrFormat =
                                        CurrentApp.GetLanguageInfo(string.Format("310801FORMAT0{0}", ((int)item.Format).ToString("00")),
                                            item.Format.ToString());
                                    conditionItem.StrType = CurrentApp.GetLanguageInfo(string.Format("310801TYPE0{0}", ((int)item.Type).ToString("00")), item.Type.ToString());
                                    conditionItem.StartDragged += item_StartDragged;
                                    conditionItem.DragOver += item_DragOver;
                                    conditionItem.Dropped += item_Dropped;
                                mListAvaliableParamItems.Add(conditionItem);
                            }
                        }
                    }
                }
                //CreateAvaliableParamsItems();先注释掉 明天再看~~这样不会有问题~
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OperationReturn SavaTemp = new OperationReturn();
                SavaTemp = SaveConfig();
                ModifyValue();
                string msg = string.Empty;
                if (SavaTemp.Result != true)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3108T002", "保存fail"));
                    #region 记录日志
                    msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108004")));
                    CurrentApp.WriteOperationLog(string.Format("3108004"), ConstValue.OPT_RESULT_FAIL, msg);
                    #endregion
                    return;
                }
                ShowInformation(CurrentApp.GetLanguageInfo("3108T001", "保存成功"));
                #region 记录日志
                msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108004")));
                CurrentApp.WriteOperationLog(string.Format("3108004"), ConstValue.OPT_RESULT_SUCCESS, msg);
                #endregion
                if (PageParent.paramsItemsconfigPage != null)
                {
                    PageParent.paramsItemsconfigPage.InitmStatisticalParam();
                }
                PageParent.PopupPanel.IsOpen = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        #endregion


        #region  Operations

        public OperationReturn SaveConfig()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<StatisticalParamItem> listItems = new List<StatisticalParamItem>();
                for (int i = 0; i < mListTabItems.Count; i++)
                {
                    CombinedParamTabItem tabItem = mListTabItems[i];
                    for (int j = 0; j < tabItem.Items.Count; j++)
                    {
                        CombinedParamItemModel item = tabItem.Items[j];
                        //这个是将我要的东西给到公共类StatisticalParamItem里面  赋好值
                        item.Apply();
                        listItems.Add(item.ParamItem);
                    }
                }
                if (listItems.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3108Codes.SaveAddedParamItemsInfos;
                    int count = listItems.Count;
                    //webRequest.ListData.Add(count.ToString());
                    //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(listItems[i]);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31081Client client = new Service31081Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31081"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                }
                //那些没有放入Tab里的也需修改里面的字段保存(有的做了删除参数的 也是需要将其存入数据库的)
                if (mListAvaliableParamItems.Count > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3108Codes.SaveAddedParamItemsInfos;
                    int count = mListAvaliableParamItems.Count;
                    //webRequest.ListData.Add(count.ToString());
                    //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        optReturn = XMLHelper.SeriallizeObject(mListAvaliableParamItems[i].ParamItem);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    Service31081Client client = new Service31081Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31081"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
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

        //根据放入的统计参数子项来给其赋初始值[增加值的记录]或者删除那条记录,对T_31_044表进行的操作
        private OperationReturn ModifyValue()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
              
                //如果没有加入到的,或者已经从可组合参数大项里面移除的,那么就对T_31_044表进行删除操作
                if (mListAvaliableParamItems != null)
                {
                    for (int i = 0; i < mListAvaliableParamItems.Count; i++)
                    {
                        CombinedParamItemModel temp = mListAvaliableParamItems[i];
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S3108Codes.ModifyCombinedParamItems;
                        //此子项表示已经移除了参数大项,要对T_31_044里面的相应的记录做删除操作
                        webRequest.ListData.Add("Delete");
                        webRequest.ListData.Add(temp.ID.ToString());
                        webRequest.ListData.Add(temp.StatisticalParamItemID);
                        Service31081Client client = new Service31081Client(
                           WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                           WebHelper.CreateEndpointAddress(
                               CurrentApp.Session.AppServerInfo,
                               "Service31081"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            optReturn.Result = false;
                            optReturn.Code = webReturn.Code;
                            optReturn.Message = webReturn.Message;
                            return optReturn;
                        }
                    }
                }
                //如果已经加入到了可组合的参数大项里面,那么就在T_31_044表里面增加一个记录,并且初始值(C004)为空
                if (mListAddedParamsItems != null)
                {
                    for (int i = 0; i < mListAddedParamsItems.Count; i++)
                    {
                        CombinedParamItemModel temp = mListAddedParamsItems[i];
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S3108Codes.ModifyCombinedParamItems;
                        //此子项表示已经添加到参数大项里,要对T_31_044里增加相应的记录
                        webRequest.ListData.Add("Add");
                        webRequest.ListData.Add(temp.ID.ToString());
                        webRequest.ListData.Add(temp.StatisticalParamItemID);
                        Service31081Client client = new Service31081Client(
                           WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                           WebHelper.CreateEndpointAddress(
                               CurrentApp.Session.AppServerInfo,
                               "Service31081"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            optReturn.Result = false;
                            optReturn.Code = webReturn.Code;
                            optReturn.Message = webReturn.Message;
                            ShowException(optReturn.Message);
                            return optReturn;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(ex.Message);
            }
            return optReturn;
        }

        #endregion


        #region DragDrop

        void item_Dropped(object sender, DragDropEventArgs e)
        {
            var item = sender as CombinedParamItemModel;
            if (item != null && item.IsAddedItem)
            {
                var source = e.DragData as IDataObject;
                if (source != null)
                {
                    var sourceItem = source.GetData(typeof(CombinedParamItemModel)) as CombinedParamItemModel;
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
            var item = sender as CombinedParamItemModel;
            var dragSource = e.DragSource;
            if (item != null && dragSource != null)
            {
                DragDrop.DoDragDrop(dragSource, item, DragDropEffects.Move);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                TxtAvaliableParamItem.Text = CurrentApp.GetLanguageInfo("310801T001", "Avaliable ParamItems");
                BtnSave.Content = CurrentApp.GetLanguageInfo("310801B001", "Save");
                BtnCancel.Content = CurrentApp.GetLanguageInfo("310801B002", "Cancel");
                //视图列
                CreateViewColumns();

                //按钮
                //CreateToolButton();

                //条件项
                CreateAvaliableParamsItems();
                //CreateCombinedParamTabItem();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
    }
}