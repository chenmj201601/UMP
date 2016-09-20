//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6ba1e35c-c1a0-4abc-ac54-621b30d1997d
//        CLR Version:              4.0.30319.42000
//        Name:                     UCFavoriteModuleView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.WidgetViews
//        File Name:                UCFavoriteModuleView
//
//        created by Charley at 2016/3/10 11:25:40
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UMPS1206.Models;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Communications;

namespace UMPS1206.WidgetViews
{
    /// <summary>
    /// UCFavoriteModuleView.xaml 的交互逻辑
    /// </summary>
    public partial class UCFavoriteModuleView : IWidgetView
    {

        #region Members

        public WidgetItem WidgetItem { get; set; }
        public IList<BasicDataInfo> ListBasicDataInfos { get; set; }
        public bool IsCenter { get; set; }
        public bool IsFull { get; set; }

        private bool mIsInited;
        private List<BasicAppInfo> mListModuleInfos;
        private List<ModuleUsageInfo> mListUsageInfos;
        private List<FavoriteModuleItem> mListFavoriteItems;
        private ObservableCollection<FavoriteModuleItem> mListTopFavoriteItems;
        private ObservableCollection<ModuleUsageItem> mListModuleUsageItems;
        private List<UserWidgetPropertyValue> mListPropertyValues;

        #endregion


        #region 配置参数的属性编号

        public const int WIDGET_PROPERTY_ID_MAXMOUDLENUM = 1;
        public const int WIDGET_PROPERTY_ID_LASTDATETYPE = 2;

        #endregion


        public UCFavoriteModuleView()
        {
            InitializeComponent();

            mListModuleInfos = new List<BasicAppInfo>();
            mListUsageInfos = new List<ModuleUsageInfo>();
            mListFavoriteItems = new List<FavoriteModuleItem>();
            mListTopFavoriteItems = new ObservableCollection<FavoriteModuleItem>();
            mListModuleUsageItems = new ObservableCollection<ModuleUsageItem>();
            mListPropertyValues = new List<UserWidgetPropertyValue>();

            Loaded += UCFavoriteModuleView_Loaded;
        }

        void UCFavoriteModuleView_Loaded(object sender, RoutedEventArgs e)
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
                WidgetItem.View = this;
                ChartFavoriteModule.ItemsSource = mListTopFavoriteItems;
                CommandBindings.Add(new CommandBinding(ItemClickCommand, ItemClickCommand_Executed,
                    (s, ce) => ce.CanExecute = true));

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadWidgetPropertyValues();
                    LoadModuleInfos();
                    LoadUsageInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetViewMode();
                    CreateFavoriteModuleItems();
                    CreateModuleUsageItems();
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

        private void LoadUsageInfos()
        {
            try
            {

                #region Test

                //mListUsageInfos.Clear();
                //ModuleUsageInfo info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3102;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-43);
                //info.EndTime = DateTime.Now.AddHours(-42);
                //info.HostName = "Charley-PC1";
                //info.HostAddress = "192.168.4.1";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3102;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-23);
                //info.EndTime = DateTime.Now.AddHours(-22);
                //info.HostName = "Charley-PC2";
                //info.HostAddress = "192.168.4.2";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3102;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-13);
                //info.EndTime = DateTime.Now.AddHours(-12);
                //info.HostName = "Charley-PC3";
                //info.HostAddress = "192.168.4.3";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3102;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-3);
                //info.EndTime = DateTime.Now.AddHours(-2);
                //info.HostName = "Charley-PC2";
                //info.HostAddress = "192.168.4.2";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3101;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-43);
                //info.EndTime = DateTime.Now.AddHours(-42);
                //info.HostName = "Charley-PC5";
                //info.HostAddress = "192.168.4.5";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3101;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-23);
                //info.EndTime = DateTime.Now.AddHours(-22);
                //info.HostName = "Charley-PC3";
                //info.HostAddress = "192.168.4.3";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 3101;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-3);
                //info.EndTime = DateTime.Now.AddHours(-2);
                //info.HostName = "Charley-PC3";
                //info.HostAddress = "192.168.4.3";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 1110;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-43);
                //info.EndTime = DateTime.Now.AddHours(-42);
                //info.HostName = "Charley-PC2";
                //info.HostAddress = "192.168.4.2";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 1110;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-43);
                //info.EndTime = DateTime.Now.AddHours(-42);
                //info.HostName = "Charley-PC1";
                //info.HostAddress = "192.168.4.1";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 1110;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-23);
                //info.EndTime = DateTime.Now.AddHours(-22);
                //info.HostName = "Charley-PC5";
                //info.HostAddress = "192.168.4.5";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 1110;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-13);
                //info.EndTime = DateTime.Now.AddHours(-12);
                //info.HostName = "Charley-PC1";
                //info.HostAddress = "192.168.4.1";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 1110;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-3);
                //info.EndTime = DateTime.Now.AddHours(-2);
                //info.HostName = "Charley-PC3";
                //info.HostAddress = "192.168.4.3";
                //mListUsageInfos.Add(info);
                //info = new ModuleUsageInfo();
                //info.SessionID = Guid.NewGuid().ToString();
                //info.ModuleID = 2102;
                //info.UserID = CurrentApp.Session.UserID;
                //info.StartArgs = string.Empty;
                //info.BeginTime = DateTime.Now.AddHours(-43);
                //info.EndTime = DateTime.Now.AddHours(-42);
                //info.HostName = "Charley-PC2";
                //info.HostAddress = "192.168.4.2";
                //mListUsageInfos.Add(info);

                #endregion


                mListUsageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetAppUsageInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                int dateType = 4;
                var propertyValue =
                    mListPropertyValues.FirstOrDefault(p => p.PropertyID == WIDGET_PROPERTY_ID_LASTDATETYPE);
                if (propertyValue != null)
                {
                    int intValue;
                    if (int.TryParse(propertyValue.Value01, out intValue))
                    {
                        dateType = intValue;
                    }
                }
                DateTime lastDate = DateTime.Now.ToUniversalTime().AddYears(-1);
                switch (dateType)
                {
                    case 1:
                        lastDate = DateTime.Now.ToUniversalTime().AddMonths(-1);
                        break;
                    case 2:
                        lastDate = DateTime.Now.ToUniversalTime().AddMonths(-3);
                        break;
                    case 3:
                        lastDate = DateTime.Now.ToUniversalTime().AddMonths(-6);
                        break;
                    case 4:
                        lastDate = DateTime.Now.ToUniversalTime().AddYears(-1);
                        break;
                    case 5:
                        lastDate = DateTime.Now.ToUniversalTime().AddYears(-2);
                        break;
                    case 6:
                        lastDate = DateTime.Now.ToUniversalTime().AddYears(-5);
                        break;
                    case 7:
                        lastDate = DateTime.Now.ToUniversalTime().AddYears(-10);
                        break;
                }
                webRequest.ListData.Add(lastDate.ToString("yyyy-MM-dd HH:mm:ss"));
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ModuleUsageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ModuleUsageInfo info = optReturn.Data as ModuleUsageInfo;
                    if (info == null) { continue; }
                    mListUsageInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadUsageInfos", string.Format("Load end.\t{0}", mListUsageInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadWidgetPropertyValues()
        {
            try
            {
                mListPropertyValues.Clear();
                if (WidgetItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserWidgetPropertyValueList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(WidgetItem.WidgetID.ToString());
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserWidgetPropertyValue>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserWidgetPropertyValue info = optReturn.Data as UserWidgetPropertyValue;
                    if (info == null) { continue; }
                    mListPropertyValues.Add(info);
                }

                CurrentApp.WriteLog("LoadPropertyValues", string.Format("Load end.\t{0}", mListPropertyValues.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create Operations

        private void CreateFavoriteModuleItems()
        {
            try
            {
                mListFavoriteItems.Clear();
                mListTopFavoriteItems.Clear();
                List<FavoriteModuleItem> listItems = new List<FavoriteModuleItem>();
                for (int i = 0; i < mListModuleInfos.Count; i++)
                {
                    var info = mListModuleInfos[i];

                    var usages = mListUsageInfos.Where(u => u.AppID == info.AppID && u.ModuleID == info.ModuleID);
                    int count = usages.Count();
                    if (count <= 0) { continue; }
                    FavoriteModuleItem item = new FavoriteModuleItem();
                    item.CurrentApp = CurrentApp;
                    item.AppID = info.AppID;
                    item.ModuleID = info.ModuleID;
                    item.Name = info.Title;
                    item.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", info.ModuleID), info.Title);
                    string strTip = string.Empty;
                    strTip += string.Format("{0}:{1}\r\n", CurrentApp.GetLanguageInfo("1206001", "Title"), item.Title);
                    strTip += string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("1206002", "Count"), count);
                    item.Tip = strTip;
                    item.Icon = GetAppIcon(info);
                    item.Background = GetBackground(item);
                    item.UseCount = count;
                    item.ModuleInfo = info;
                    listItems.Add(item);
                }
                listItems = listItems.OrderByDescending(i => i.UseCount).ToList();
                int maxNum = 5;
                var propertyValue =
                    mListPropertyValues.FirstOrDefault(p => p.PropertyID == WIDGET_PROPERTY_ID_MAXMOUDLENUM);
                if (propertyValue != null)
                {
                    string strValue = propertyValue.Value01;
                    int intValue;
                    if (int.TryParse(strValue, out intValue)
                        && intValue > 0)
                    {
                        maxNum = intValue;
                    }
                }
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
                    mListFavoriteItems.Add(item);
                    if (i < maxNum)
                    {
                        mListTopFavoriteItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateModuleUsageItems()
        {
            try
            {
                mListModuleUsageItems.Clear();
                int moduleCount = mListFavoriteItems.Count;
                Grid gridModule = new Grid();
                ColumnDefinition columnModule = new ColumnDefinition();
                columnModule.Width = new GridLength(50, GridUnitType.Star);
                gridModule.ColumnDefinitions.Add(columnModule);
                columnModule = new ColumnDefinition();
                columnModule.Width = new GridLength(50, GridUnitType.Star);
                gridModule.ColumnDefinitions.Add(columnModule);
                for (int k = 0; k < moduleCount; k++)
                {
                    gridModule.RowDefinitions.Add(new RowDefinition());
                }
                Border borderModuleBg;
                Border borderUsageBg;
                for (int i = 0; i < moduleCount; i++)
                {
                    var moduleItem = mListFavoriteItems[i];
                    borderModuleBg = new Border();
                    borderModuleBg.Background = moduleItem.Background;
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;

                    //Icon
                    Image imageIcon = new Image();
                    imageIcon.SetValue(StyleProperty, Resources["ImageIconStyle"]);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(moduleItem.Icon, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    imageIcon.Source = bitmap;
                    sp.Children.Add(imageIcon);

                    //Title
                    TextBlock txtTitle = new TextBlock();
                    txtTitle.Text = moduleItem.Title;
                    txtTitle.SetValue(StyleProperty, Resources["TxtDetailStyle"]);
                    sp.Children.Add(txtTitle);

                    borderModuleBg.Child = sp;
                    borderModuleBg.SetValue(Grid.ColumnProperty, 0);
                    borderModuleBg.SetValue(Grid.RowProperty, i);

                    gridModule.Children.Add(borderModuleBg);

                    borderModuleBg = new Border();
                    borderModuleBg.Background = moduleItem.Background;

                    Grid gridUsage = new Grid();
                    ColumnDefinition columnUsage = new ColumnDefinition();
                    columnUsage.Width = new GridLength(50, GridUnitType.Star);
                    gridUsage.ColumnDefinitions.Add(columnUsage);
                    columnUsage = new ColumnDefinition();
                    columnUsage.Width = new GridLength(50, GridUnitType.Star);
                    gridUsage.ColumnDefinitions.Add(columnUsage);

                    var usages =
                        mListUsageInfos.Where(u => u.AppID == moduleItem.AppID
                            && u.ModuleID == moduleItem.ModuleID)
                            .OrderByDescending(u => u.BeginTime)
                            .ToList();
                    int useCount = usages.Count;
                    for (int k = 0; k < useCount; k++)
                    {
                        RowDefinition row = new RowDefinition();
                        gridUsage.RowDefinitions.Add(row);
                    }
                    for (int j = 0; j < useCount; j++)
                    {
                        var usage = usages[j];
                        var usageItem = new ModuleUsageItem();
                        usageItem.CurrentApp = CurrentApp;
                        usageItem.ModuleItem = moduleItem;
                        usageItem.BeginTime = usage.BeginTime;
                        usageItem.UseTime = usage.BeginTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        usageItem.HostInfo = string.Format("{0}[{1}]", usage.HostName, usage.HostAddress);
                        usageItem.UsageInfo = usage;

                        //UseTime
                        borderUsageBg = new Border();
                        if (j % 2 == 1)
                        {
                            borderUsageBg.Background = Brushes.LightGray;
                        }
                        else
                        {
                            borderUsageBg.Background = Brushes.Transparent;
                        }
                        TextBlock txtUseTime = new TextBlock();
                        txtUseTime.Text = usageItem.UseTime;
                        txtUseTime.SetValue(StyleProperty, Resources["TxtDetailStyle"]);
                        borderUsageBg.Child = txtUseTime;
                        borderUsageBg.SetValue(Grid.ColumnProperty, 0);
                        borderUsageBg.SetValue(Grid.RowProperty, j);
                        gridUsage.Children.Add(borderUsageBg);

                        //HostInfo
                        borderUsageBg = new Border();
                        if (j % 2 == 1)
                        {
                            borderUsageBg.Background = Brushes.LightGray;
                        }
                        else
                        {
                            borderUsageBg.Background = Brushes.Transparent;
                        }
                        TextBlock txtHostInfo = new TextBlock();
                        txtHostInfo.Text = usageItem.HostInfo;
                        txtHostInfo.SetValue(StyleProperty, Resources["TxtDetailStyle"]);
                        borderUsageBg.Child = txtHostInfo;
                        borderUsageBg.SetValue(Grid.ColumnProperty, 1);
                        borderUsageBg.SetValue(Grid.RowProperty, j);
                        gridUsage.Children.Add(borderUsageBg);

                        mListModuleUsageItems.Add(usageItem);
                    }

                    borderModuleBg.Child = gridUsage;
                    borderModuleBg.SetValue(Grid.ColumnProperty, 1);
                    borderModuleBg.SetValue(Grid.RowProperty, i);
                    gridModule.Children.Add(borderModuleBg);
                }
                BorderDetail.Child = gridModule;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Refresh

        public void Refresh()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadWidgetPropertyValues();
                    LoadModuleInfos();
                    LoadUsageInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetViewMode();
                    CreateFavoriteModuleItems();
                    CreateModuleUsageItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void SetViewMode()
        {
            try
            {
                if (WidgetItem == null) { return; }
                if (WidgetItem.IsFull || WidgetItem.IsCenter)
                {
                    GridChart.Width = new GridLength(50, GridUnitType.Star);
                    GridDetail.Width = new GridLength(50, GridUnitType.Star);
                    SplitterDetail.IsEnabled = true;
                    BorderDetailView.Visibility = Visibility.Visible;
                    if (!WidgetItem.IsCenter)
                    {
                        //Height = 350;
                    }
                }
                else
                {
                    Height = 350;
                    SplitterDetail.IsEnabled = false;
                    BorderDetailView.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string GetAppIcon(BasicAppInfo info)
        {
            try
            {
                return string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                    CurrentApp.Session.AppServerInfo.Protocol,
                    CurrentApp.Session.AppServerInfo.Address,
                    CurrentApp.Session.AppServerInfo.Port,
                    CurrentApp.Session.ThemeName,
                    info.Icon);
            }
            catch
            {
                return string.Empty;
            }
        }

        private Brush GetBackground(FavoriteModuleItem item)
        {
            Random r = new Random(item.ModuleID + (int)DateTime.Now.Ticks);
            return
                new SolidColorBrush(Color.FromRgb((byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255)));
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
            var item = e.Parameter as FavoriteModuleItem;
            if (item != null)
            {
                var info = item.ModuleInfo;
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
                for (int i = 0; i < mListFavoriteItems.Count; i++)
                {
                    var item = mListFavoriteItems[i];
                    var info = item.ModuleInfo;
                    if (info != null)
                    {
                        item.Title = CurrentApp.GetLanguageInfo(string.Format("FO{0}", info.ModuleID), info.Title);
                    }
                    string strTip = string.Empty;
                    strTip += string.Format("{0}:{1}\r\n", CurrentApp.GetLanguageInfo("1206001", "Title"), item.Title);
                    strTip += string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("1206002", "Count"), item.UseCount);
                    item.Tip = strTip;
                }

                CreateModuleUsageItems();
            }
            catch { }
        }

        #endregion
    }
}
